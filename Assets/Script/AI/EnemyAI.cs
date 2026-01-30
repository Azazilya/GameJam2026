using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Roaming, Chasing }

    [Header("Data")]
    [SerializeField] private EnemyStats stats;
    [SerializeField] private EnemyState currentState = EnemyState.Roaming;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float stopChasingRadius = 7f; // Agar tidak langsung berhenti saat player di tepi radius

    [Header("Roaming Settings")]
    public float roamRadius = 10f;
    public float waitAtPointTime = 2f;
    
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 roamTarget;
    private float waitTimer;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // NavMeshAgent untuk 2D biasanya butuh penyesuaian sumbu, 
        // pastikan kamu menggunakan NavMeshPlus atau atur sumbu Z agar tetap 0.
        agent.speed = stats.movementSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        SetNewRoamTarget();
    }

    void Update()
    {
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
    }

    void HandleRoaming()
    {
        // Cek jika sudah sampai di tujuan
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitAtPointTime;
            }

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewRoamTarget();
            }
        }
    }

    void HandleChasing()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void SetNewRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        // Mencari titik terdekat di NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
        {
            roamTarget = hit.position;
            agent.SetDestination(roamTarget);
        }
    }

}