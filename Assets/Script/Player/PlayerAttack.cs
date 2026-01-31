using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer handRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D attackCollider;
    [SerializeField] private List<SpriteRenderer> allPlayerRenderers;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationOffset = -180f;
    [SerializeField] private bool flipVerticalManual = true;

    private float animTimer;
    private int currentFrame;
    private bool isAttacking;
    private bool isCharging;
    private float chargeTimer;
    private float flickerPhase;
    private bool isRotating; 

    void Update()
    {
        RotateHand();
        HandleInput();
        AnimateHand();
        if (isCharging) ApplyFlicker();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isCharging = true;
            chargeTimer = 0;
            currentFrame = 0; 
            flickerPhase = 0;
            playerController.SetSlowdown(true); // Memperlambat gerak
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            chargeTimer += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ExecuteAttack();
        }
    }

    void ExecuteAttack()
    {
        isCharging = false;
        ResetFlicker(); 
        playerController.SetSlowdown(false);
        
        PlayerStateData state = playerController.GetCurrentState();

        if (state.useRotateAttack)
        {
            // JALANKAN TIPE ROTASI
            StartCoroutine(RotateAttackRoutine(state));
        }
        else
        {
            // JALANKAN TIPE ANIMASI (Logika lama)
            isAttacking = true;
            if (attackCollider != null) attackCollider.enabled = true;
        }

        isAttacking = false;
        isRotating = false;
    }

    private IEnumerator RotateAttackRoutine(PlayerStateData state)
    {
        isAttacking = true; // Tetap true agar trigger damage bekerja
        isRotating = true;
        if (attackCollider != null) attackCollider.enabled = true;

        Quaternion startRotation = transform.localRotation;
        // Tentukan arah ayunan (misal: memutar searah jarum jam)
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, state.rotateAngle);

        float elapsed = 0;
        // 1. Ayunan ke depan
        while (elapsed < state.rotateDuration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / state.rotateDuration);
            yield return null;
        }

        // 2. Jeda sangat singkat (opsional)
        yield return new WaitForSeconds(0.05f);

        // 3. Matikan collider lebih awal agar tidak hit dua kali saat kembali
        if (attackCollider != null) attackCollider.enabled = false;

        // 4. Kembali ke posisi awal
        elapsed = 0;
        while (elapsed < state.rotateDuration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, startRotation, elapsed / state.rotateDuration);
            yield return null;
        }

        isAttacking = false;
        isRotating = false;
    }

    void RotateHand()
    {
        if (mainCamera == null || isRotating) return;
        if (mainCamera == null) return;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        Vector2 dir = mousePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        bool isMouseLeft = mousePos.x < transform.position.x;
        handRenderer.flipY = flipVerticalManual ? !isMouseLeft : isMouseLeft;
    }

    void AnimateHand()
    {
        if (playerController == null || isRotating) return;
        if (playerController == null) return;
        PlayerStateData state = playerController.GetCurrentState(); //
        if (state == null) return;

        Sprite[] targetArray = (isAttacking || isCharging) ? state.handAttack : state.handIdle;
        if (targetArray == null || targetArray.Length == 0) return;

        if (isCharging && chargeTimer >= (state.chargeThreshold * 0.3f))
        {
            currentFrame = Mathf.Min(state.holdFrameIndex, targetArray.Length - 1);
            handRenderer.sprite = targetArray[currentFrame];
            return; 
        }

        animTimer += Time.deltaTime;
        if (animTimer >= playerController.animationSpeed)
        {
            animTimer = 0;
            currentFrame++;

            if (isAttacking && currentFrame >= targetArray.Length)
            {
                isAttacking = false;
                currentFrame = 0;
                if (attackCollider != null) attackCollider.enabled = false;
            }
            
            int frameIdx = currentFrame % targetArray.Length;
            handRenderer.sprite = targetArray[frameIdx];
        }
    }

    void ApplyFlicker()
    {
        PlayerStateData state = playerController.GetCurrentState();
        if (state == null) return;

        float progress = Mathf.Min(chargeTimer / state.superChargeThreshold, 1f);
        float currentFlickerSpeed = Mathf.Lerp(state.baseFlickerSpeed, state.maxFlickerSpeed, progress);

        flickerPhase += Time.deltaTime * currentFlickerSpeed;
        float lerp = Mathf.PingPong(flickerPhase, 1f);
        
        Color finalColor = Color.Lerp(Color.white, state.flickerColor, lerp);

        foreach (var renderer in allPlayerRenderers)
        {
            if (renderer != null) renderer.color = finalColor;
        }
    }

    void ResetFlicker()
    {
        flickerPhase = 0;
        foreach (var renderer in allPlayerRenderers)
        {
            if (renderer != null) renderer.color = Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    // Cek tag musuh atau boss
    if (isAttacking && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
    {
        PlayerStateData state = playerController.GetCurrentState();
        float damageMultiplier = 1f;

        // --- Logika Charge Multiplier (Sesuaikan jika kamu punya logika khusus) ---
        if (chargeTimer >= state.superChargeThreshold) damageMultiplier = 2.5f;
        else if (chargeTimer >= state.chargeThreshold) damageMultiplier = 1.5f;

        float finalDamage = state.attackDamage * damageMultiplier;
        bool hitSuccessful = false;

        // 1. CEK JIKA TARGET ADALAH MUSUH BIASA
        EnemyHealthHandler enemy = collision.GetComponent<EnemyHealthHandler>();
        if (enemy != null)
        {
            enemy.TakeDamage(finalDamage);
            enemy.ApplyKnockback((collision.transform.position - transform.position).normalized, state.knockbackForce);
            hitSuccessful = true;
        }
        // 2. CEK JIKA TARGET ADALAH BOSS
        else 
        {
            BossHealthHandler boss = collision.GetComponent<BossHealthHandler>();
            if (boss != null)
            {
                boss.TakeDamage(finalDamage);
                boss.ApplyKnockback((collision.transform.position - transform.position).normalized, state.knockbackForce);
                hitSuccessful = true;
            }
        }

        // 3. LOGIKA LIFESTEAL (Hanya berjalan jika serangan berhasil mengenai target)
        if (hitSuccessful)
        {
            float lifestealAmount = finalDamage * 0.35f;
            PlayerHealthHandler playerHealth = GetComponentInParent<PlayerHealthHandler>();
            if (playerHealth != null)
            {
                playerHealth.Heal(lifestealAmount);
                Debug.Log($"<color=magenta>Lifesteal!</color> Restored: {lifestealAmount} from {collision.name}");
            }
        }
    }
}
}