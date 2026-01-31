using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Roaming, Chasing, Dead }

    [Header("Data")]
    [SerializeField] private EnemyStats stats;
    [SerializeField] private EnemyState currentState = EnemyState.Roaming;

    [Header("Animation Settings")]
    [SerializeField] private Sprite[] animationSprites; // Masukkan 2 sprite di Inspector
    [SerializeField] private float frameRate = 0.2f;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float stopChasingRadius = 7f;

    [Header("Roaming Settings")]
    public float roamRadius = 10f;
    public float waitAtPointTime = 2f;
    
    private NavMeshAgent agent;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float animTimer;
    private bool isDead = false;

    private Vector3 roamTarget;
    private float waitTimer;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        agent.speed = stats.movementSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNewRoamTarget();
    }

    void Update()
    {
        if (isDead) return;

        HandleAnimation();

        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

        switch (currentState)
        {
            case EnemyState.Roaming:
                HandleRoaming();
                if (distanceToPlayer <= detectionRadius) currentState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
                HandleChasing();
                if (distanceToPlayer > stopChasingRadius) currentState = EnemyState.Roaming;
                break;
        }

        // Contoh Trigger Mati (Hapus ini jika kamu punya sistem Health sendiri)
        if (Input.GetKeyDown(KeyCode.K)) Die(); 
    }

    void HandleAnimation()
    {
        if (animationSprites.Length < 2) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer = 0;
            currentFrame = (currentFrame + 1) % animationSprites.Length;
            spriteRenderer.sprite = animationSprites[currentFrame];
        }
    }

    // Panggil fungsi ini saat HP musuh habis
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentState = EnemyState.Dead;
        agent.isStopped = true; // Berhenti bergerak
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        Color startColor = spriteRenderer.color;
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    // --- Sisa fungsi Roaming & Chasing tetap sama ---
    void HandleRoaming()
    {
        if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting) { isWaiting = true; waitTimer = waitAtPointTime; }
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0) { isWaiting = false; SetNewRoamTarget(); }
        }
    }

    void HandleChasing()
    {
        if (player != null) agent.SetDestination(player.position);
    }

    void SetNewRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
        {
            roamTarget = hit.position;
            agent.SetDestination(roamTarget);
        }
    }
}