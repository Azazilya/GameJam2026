using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer handRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera mainCamera;

    [Header("Rotation & Orientation")]
    [Tooltip("Gunakan -180 jika arah rotasi sebelumnya terbalik.")]
    [SerializeField] private float rotationOffset = -180f;
    [Tooltip("Centang ini jika gambar tangan terlihat terbalik (atas di bawah).")]
    [SerializeField] private bool flipVerticalManual = true;

    private float animTimer;
    private int currentFrame;
    private bool isAttacking;

    void Update()
    {
        RotateHand();
        if (Input.GetMouseButtonDown(0) && !isAttacking) StartAttack();
        AnimateHand();
    }

    void RotateHand()
    {
        if (mainCamera == null) return;

        // Ambil posisi mouse
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        Vector3 dir = mousePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Terapkan rotasi dengan offset -180
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        // LOGIKA FLIP:
        // Jika mouse di kiri, kita flip agar tangan tidak terbalik secara visual saat rotasi ekstrem
        bool isMouseLeft = mousePos.x < transform.position.x;

        if (flipVerticalManual)
        {
            // Jika tangan terbalik saat offset -180, kita balikkan logika FlipY-nya
            handRenderer.flipY = !isMouseLeft; 
        }
        else
        {
            handRenderer.flipY = isMouseLeft;
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        currentFrame = 0;
        animTimer = 0;
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
                if (isAttacking) isAttacking = false;
                currentFrame = 0;
            }
        }
        handRenderer.sprite = targetArray[currentFrame % targetArray.Length];
    }
}