using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer handRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D attackCollider; // Tarik collider senjata ke sini

    [Header("Rotation Settings")]
    [SerializeField] private float rotationOffset = -180f;
    [SerializeField] private bool flipVerticalManual = true;

    private float animTimer;
    private int currentFrame;
    private bool isAttacking;

    void Start()
    {
        if (attackCollider != null) attackCollider.enabled = false;
    }

    void Update()
    {
        RotateHand();
        if (Input.GetMouseButtonDown(0) && !isAttacking) StartAttack();
        AnimateHand();
    }

    void RotateHand()
    {
        if (mainCamera == null) return;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        Vector3 dir = mousePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        bool isMouseLeft = mousePos.x < transform.position.x;
        handRenderer.flipY = flipVerticalManual ? !isMouseLeft : isMouseLeft;
    }

    void StartAttack()
    {
        isAttacking = true;
        currentFrame = 0;
        animTimer = 0;
        if (attackCollider != null) attackCollider.enabled = true; // Aktifkan collider
    }

    void AnimateHand()
    {
        if (playerController == null) return;
        PlayerStateData state = playerController.GetCurrentState();
        if (state == null) return;

        Sprite[] targetArray = isAttacking ? state.handAttack : state.handIdle;
        if (targetArray == null || targetArray.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= playerController.animationSpeed)
        {
            animTimer = 0;
            currentFrame++;
            if (currentFrame >= targetArray.Length)
            {
                isAttacking = false;
                currentFrame = 0;
                if (attackCollider != null) attackCollider.enabled = false; // Matikan collider setelah animasi selesai
            }
            handRenderer.sprite = targetArray[currentFrame % targetArray.Length];
        }
    }

    // Fungsi untuk memberikan damage saat collider mengenai musuh
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy"))
        {
            EnemyHealthHandler enemy = collision.GetComponent<EnemyHealthHandler>();
            if (enemy != null)
            {
                PlayerStateData state = playerController.GetCurrentState();
                enemy.TakeDamage(state.attackDamage);
            }
        }
    }
}