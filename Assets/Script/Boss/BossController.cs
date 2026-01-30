using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform player;

    public enum BossState
    {
        Inactive,
        Idle,
        Chase,
        Attack,
        Retreat
    }

    [Header("State")]
    public BossState currentState = BossState.Inactive;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float retreatSpeed = 4f;

    [Header("Radius")]
    public float detectRadius = 6f;
    public float attackRadius = 2.5f;

    [Header("Attack")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.25f;
    public float chargeTime = 0.4f;

    private Rigidbody2D rb;
    private Vector2 arenaCenter;
    private bool isBusy;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // SIMPAN POSISI TENGAH ARENA
        arenaCenter = transform.position;
    }

    void Update()
    {
        if (player == null || isBusy) return;

        FacePlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Inactive:
                rb.linearVelocity = Vector2.zero;
                break;

            case BossState.Idle:
                rb.linearVelocity = Vector2.zero;
                if (distanceToPlayer <= detectRadius)
                    currentState = BossState.Chase;
                break;

            case BossState.Chase:
                if (distanceToPlayer <= attackRadius)
                {
                    StartCoroutine(AttackRoutine());
                    return;
                }

                MoveTowards(player.position, moveSpeed);
                break;
        }
    }

    // =========================
    // ACTIVATE FROM TRIGGER
    // =========================
    public void ActivateBoss()
    {
        currentState = BossState.Idle;
    }

    // =========================
    // ATTACK
    // =========================
    IEnumerator AttackRoutine()
    {
        currentState = BossState.Attack;
        isBusy = true;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeTime);

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(RetreatRoutine());
    }

    // =========================
    // RETREAT (BALIK KE TENGAH)
    // =========================
    IEnumerator RetreatRoutine()
    {
        currentState = BossState.Retreat;

        while (Vector2.Distance(transform.position, arenaCenter) > 0.1f)
        {
            MoveTowards(arenaCenter, retreatSpeed);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isBusy = false;
        currentState = BossState.Idle;
    }

    // =========================
    // MOVE HELPER
    // =========================
    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    // =========================
    // FACE PLAYER
    // =========================
    void FacePlayer()
    {
        float dirX = player.position.x - transform.position.x;
        if (dirX != 0)
            transform.localScale = new Vector3(Mathf.Sign(dirX), 1, 1);
    }
}
