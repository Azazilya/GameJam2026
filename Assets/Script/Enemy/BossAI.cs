using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, Chase, Attack, Retreat }

    [Header("References")]
    [SerializeField] private BossData bossData;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer handRenderer;
    [SerializeField] private Collider2D attackCollider;

    [Header("Body Sheets (Wajib diisi di Inspector)")]
    public Sprite[] spritesDepan;
    public Sprite[] spritesBelakang;
    public Sprite[] spritesSamping;

    [Header("AI Config")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2.2f;
    [SerializeField] private float retreatDuration = 2f;
    [SerializeField] private float retreatDistance = 5f;

    private BossState currentState = BossState.Idle;
    private NavMeshAgent agent;
    private float animTimer;
    private int currentFrame;
    private bool isAttacking;
    private int attacksRemaining = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Setup NavMeshAgent agar tidak merusak visual 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = bossData.movementSpeed;

        if (playerTransform == null) 
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (attackCollider != null) attackCollider.enabled = false;
        
        // Pastikan posisi Z nol
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void Update()
    {
        if (playerTransform == null || bossData == null) return;

        UpdateBrain();
        HandleVisuals();
        
        // Kunci posisi Z agar tidak hilang tertelan background
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void UpdateBrain()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case BossState.Idle:
                agent.isStopped = true;
                if (distanceToPlayer <= detectionRadius) 
                {
                    // Saat pertama kali melihat player, tentukan jumlah serangan (1-3)
                    attacksRemaining = Random.Range(1, 4); 
                    currentState = BossState.Chase;
                    agent.isStopped = false;
                }
                break;

            case BossState.Chase:
                agent.SetDestination(playerTransform.position);
                
                // Hanya menyerang jika jarak cukup dan masih punya jatah serangan
                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    StartCoroutine(AttackSequence());
                }
                break;

            case BossState.Attack:
                break;

            case BossState.Retreat:
                Vector2 dirAway = (transform.position - playerTransform.position).normalized;
                Vector3 retreatPos = transform.position + (Vector3)dirAway * retreatDistance;
                agent.SetDestination(retreatPos);
                break;
        }
    }

    private void HandleVisuals()
    {
        // Arah pandang berdasarkan posisi player (seperti kursor)
        float relX = playerTransform.position.x - transform.position.x;
        float relY = playerTransform.position.y - transform.position.y;

        bodyRenderer.flipX = (relX > 0);
        if (headRenderer) headRenderer.flipX = (relX > 0);

        // Pilih Array (Sinkron dengan logic PlayerController)
        Sprite[] targetBody;
        Sprite[] targetHead;

        if (Mathf.Abs(relX) > Mathf.Abs(relY))
        {
            targetBody = spritesSamping;
            targetHead = bossData.headWalkSamping;
        }
        else
        {
            targetBody = (relY > 0) ? spritesBelakang : spritesDepan;
            targetHead = (relY > 0) ? bossData.headWalkBelakang : bossData.headWalkDepan;
        }

        if (targetBody == null || targetBody.Length == 0) return;

        // Jalankan Animasi (Sederhana: selalu loop jika tidak idle)
        bool isMoving = agent.velocity.magnitude > 0.1f;

        if (!isMoving && currentState == BossState.Idle)
        {
            currentFrame = 0;
            bodyRenderer.sprite = targetBody[0];
            if (bossData.headIdle.Length > 0) headRenderer.sprite = bossData.headIdle[0];
        }
        else
        {
            animTimer += Time.deltaTime;
            if (animTimer >= 0.15f)
            {
                animTimer = 0;
                currentFrame = (currentFrame + 1) % targetBody.Length;
                
                bodyRenderer.sprite = targetBody[currentFrame];
                if (targetHead != null && targetHead.Length > 0)
                    headRenderer.sprite = targetHead[currentFrame % targetHead.Length];
            }
        }

        // Animasi & Rotasi Tangan
        HandleHandLogic(relX, relY);
    }

    private void HandleHandLogic(float relX, float relY)
    {
        if (handRenderer == null) return;

        // 1. Rotasi Tangan Menatap Player
        Vector2 dir = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        handRenderer.transform.rotation = Quaternion.Euler(0, 0, angle - 180f);
        
        // 2. Mirror Vertikal (Invert Logika FlipY)
        // Sebelumnya: (relX < 0), diubah menjadi (relX > 0) untuk membalikkan orientasi vertikalnya
        handRenderer.flipY = (relX > 0);

        // 3. Sprite Tangan (Attack vs Idle)
        Sprite[] handSprites = isAttacking ? bossData.handAttack : bossData.handIdle;
        if (handSprites != null && handSprites.Length > 0)
        {
            handRenderer.sprite = handSprites[currentFrame % handSprites.Length];
        }
    }

    private IEnumerator AttackSequence()
    {
        currentState = BossState.Attack;
        agent.isStopped = true;
        isAttacking = true;
        
        if (attackCollider) attackCollider.enabled = true;

        // Durasi serangan
        yield return new WaitForSeconds(1f / bossData.attackSpeed);

        if (attackCollider) attackCollider.enabled = false;
        isAttacking = false;
        
        attacksRemaining--; // Kurangi jatah serangan

        if (attacksRemaining <= 0)
        {
            // Jika jatah habis, baru mundur
            StartCoroutine(RetreatRoutine());
        }
        else
        {
            // Jika masih ada jatah, kembali mengejar player untuk serangan berikutnya
            currentState = BossState.Chase;
            agent.isStopped = false;
        }
    }

    private IEnumerator RetreatRoutine()
    {
        currentState = BossState.Retreat;
        agent.isStopped = false;
        
        yield return new WaitForSeconds(retreatDuration);
        
        // Setelah selesai mundur, tentukan lagi jumlah serangan untuk fase berikutnya
        attacksRemaining = Random.Range(1, 4); 
        currentState = BossState.Chase;
    }
}