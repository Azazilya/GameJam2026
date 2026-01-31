using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("State System")]
    [SerializeField] private PlayerStateData[] allStates;
    private int currentStateIndex = 0;
    private PlayerStateData currentState;
    public PlayerStateData GetCurrentState() => currentState;

    private float originalCameraSize;
    private bool isSwitching = false;
    private Vector2 lastMoveInput; // Untuk menyimpan arah pergerakan terakhir

    [Header("Switch State Effects")]
    [SerializeField] private float zoomInSize = 3f;      // Besar zoom saat ganti topeng
    [SerializeField] private float zoomDuration = 0.2f;  // Durasi transisi zoom
    [SerializeField] private float pauseDuration = 0.5f; // Berapa lama game berhenti
    [SerializeField] private float dashForce = 20f;      // Kekuatan dash setelah ganti
    [SerializeField] private float dashDuration = 0.15f; // Berapa lama dash berlangsung

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
    private bool isSlowed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (allStates != null && allStates.Length > 0) currentState = allStates[0];

        if (mainCamera != null) originalCameraSize = mainCamera.orthographicSize;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isSwitching) return; // Jangan terima input saat sedang transisi ganti topeng

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
        if (moveInput.magnitude > 0.1f)
        {
            moveInput = moveInput.normalized;
            lastMoveInput = moveInput; // Catat arah terakhir untuk dash
        }

        // Ganti fungsi Q menjadi Coroutine
        if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(SwitchStateRoutine());

        HandleIdleLogic();
        HandleAnimation(); 
        SyncHeadWithBody(); 
    }

    private IEnumerator SwitchStateRoutine()
    {
        isSwitching = true;
        
        // 1. ZOOM IN & PAUSE
        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Gunakan unscaled karena nanti Time.timeScale akan 0
            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, zoomInSize, elapsed / zoomDuration);
            yield return null;
        }

        // Berhentikan waktu (Pause Musuh & Player)
        Time.timeScale = 0f; 

        // 2. SWITCH STATE (Ganti Topeng)
        if (allStates != null && allStates.Length > 0)
        {
            currentStateIndex = (currentStateIndex + 1) % allStates.Length;
            currentState = allStates[currentStateIndex];
            Debug.Log("Switched to: " + currentState.name);
        }

        // Tunggu sebentar dalam keadaan pause
        yield return new WaitForSecondsRealtime(pauseDuration);

        // 3. ZOOM OUT & UNPAUSE
        Time.timeScale = 1f;
        elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Lerp(zoomInSize, originalCameraSize, elapsed / zoomDuration);
            yield return null;
        }

        // 4. DASH (Penambahan velocity tiba-tiba)
        if (lastMoveInput.magnitude > 0.1f)
        {
            rb.linearVelocity = lastMoveInput * dashForce;
            yield return new WaitForSeconds(dashDuration);
        }

        isSwitching = false;
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

    public void SetSlowdown(bool slowed) => isSlowed = slowed;

    void FixedUpdate()
    {
        if (isSwitching) return; 

        if (currentState != null)
        {
            float currentSpeed = currentState.movementSpeed;
            if (isSlowed) currentSpeed *= (1f - currentState.chargeMovementPenalty);
            rb.linearVelocity = moveInput * currentSpeed;
        }
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