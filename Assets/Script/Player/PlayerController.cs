using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("State System")]
    [SerializeField] private PlayerStateData[] allStates;
    private int currentStateIndex = 0;
    private PlayerStateData currentState;
    public PlayerStateData GetCurrentState() => currentState;

    [Header("References")]
    [SerializeField] private GameObject playerHolder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer bodyRenderer; 
    [SerializeField] private SpriteRenderer headRenderer; 

    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Dynamic Camera Settings")]
    public float cameraLeadDistance = 2f;
    public float maxCameraDistance = 3f;
    public float cameraFollowSpeed = 5f;
    [Tooltip("Kecepatan transisi arah kamera (Lead Smoothing)")]
    public float cameraDirectionSmoothTime = 0.2f; 

    private Vector3 cameraTargetPos;
    private Vector3 currentLeadVelocity; // Untuk SmoothDamp
    private Vector3 smoothedMoveDir;

    [Header("Animation Sheets (Body)")]
    public Sprite[] spritesDepan;
    public Sprite[] spritesBelakang;
    public Sprite[] spritesSampingKiri;
    
    [Header("Animation Timings")]
    public float animationSpeed = 0.15f;
    public float idleThreshold = 0.05f; 
    
    private float animTimer;
    private float idleTimer;
    private int currentFrame;
    private bool isIdle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (allStates != null && allStates.Length > 0) currentState = allStates[0];
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (Input.GetKeyDown(KeyCode.Q)) SwitchState();

        HandleIdleLogic();
        HandleAnimation(); 
        SyncHeadWithBody(); 
    }

    void SyncHeadWithBody()
    {
        if (currentState == null || headRenderer == null || bodyRenderer == null) return;

        // Sinkronkan flipX dengan badan (untuk arah samping)
        headRenderer.flipX = bodyRenderer.flipX;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float mouseRelY = mousePos.y - transform.position.y;
        float mouseRelX = mousePos.x - transform.position.x;

        Sprite[] targetHeadArray = null;

        // Tentukan Array Kepala berdasarkan posisi kursor (sama seperti logika tubuh)
        if (Mathf.Abs(mouseRelX) > Mathf.Abs(mouseRelY))
        {
            targetHeadArray = currentState.headWalkSamping;
        }
        else
        {
            targetHeadArray = (mouseRelY > 0) ? currentState.headWalkBelakang : currentState.headWalkDepan;
        }

        if (targetHeadArray != null && targetHeadArray.Length > 0)
        {
            // PERBAIKAN: Gunakan currentFrame yang sedang berjalan, baik saat Idle maupun Walk
            // Ini memastikan animasi tidak melompat ke frame 0 secara kasar saat berhenti.
            int frameToUse = currentFrame % targetHeadArray.Length;
            headRenderer.sprite = targetHeadArray[frameToUse];
        }
    }

    void FixedUpdate()
    {
        if (currentState != null) rb.linearVelocity = moveInput * currentState.movementSpeed;
    }

    void LateUpdate() => HandleDynamicCamera();

    void SwitchState()
    {
        if (allStates == null || allStates.Length == 0) return;
        currentStateIndex = (currentStateIndex + 1) % allStates.Length;
        currentState = allStates[currentStateIndex];
    }

    void HandleDynamicCamera()
    {
        if (mainCamera == null) return;

        Vector3 playerPos = playerHolder.transform.position;
        Vector3 rawMoveDir = new Vector3(moveInput.x, moveInput.y, 0);

        // PERBAIKAN KAMERA: Menggunakan SmoothDamp pada vektor arah agar transisi 8-axis mulus
        // Ini menghilangkan "hentakan" saat berpindah antar diagonal atau lurus
        smoothedMoveDir = Vector3.SmoothDamp(
            smoothedMoveDir, 
            rawMoveDir, 
            ref currentLeadVelocity, 
            cameraDirectionSmoothTime
        );

        Vector3 offset = smoothedMoveDir * cameraLeadDistance;
        cameraTargetPos = playerPos + Vector3.ClampMagnitude(offset, maxCameraDistance);
        cameraTargetPos.z = -10f;

        // Lerp posisi kamera akhir
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position, 
            cameraTargetPos, 
            cameraFollowSpeed * Time.deltaTime
        );
    }

    void HandleIdleLogic()
    {
        if (moveInput.magnitude < 0.1f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold) isIdle = true;
        }
        else
        {
            idleTimer = 0f;
            isIdle = false;
        }
    }

    void HandleAnimation()
    {
        Sprite[] targetArray = null;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        float mouseRelX = mousePos.x - transform.position.x;
        float mouseRelY = mousePos.y - transform.position.y;

        if (bodyRenderer != null) bodyRenderer.flipX = (mouseRelX > 0);

        if (Mathf.Abs(mouseRelX) > Mathf.Abs(mouseRelY))
            targetArray = spritesSampingKiri;
        else
            targetArray = (mouseRelY > 0) ? spritesBelakang : spritesDepan;

        if (targetArray == null || targetArray.Length == 0) return;

        if (isIdle)
        {
            currentFrame = 0;
            bodyRenderer.sprite = targetArray[0];
        }
        else
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animationSpeed)
            {
                animTimer = 0;
                if (CheckIfMovingBackward(mouseRelX, mouseRelY))
                    currentFrame = (currentFrame - 1 + targetArray.Length) % targetArray.Length;
                else
                    currentFrame = (currentFrame + 1) % targetArray.Length;

                bodyRenderer.sprite = targetArray[currentFrame % targetArray.Length];
            }
        }
    }

    private bool CheckIfMovingBackward(float relX, float relY)
    {
        if (Mathf.Abs(relX) > Mathf.Abs(relY))
        {
            if (moveInput.x > 0 && relX < 0) return true;
            if (moveInput.x < 0 && relX > 0) return true;
        }
        else
        {
            if (moveInput.y > 0 && relY < 0) return true;
            if (moveInput.y < 0 && relY > 0) return true;
        }
        return false;
    }
}