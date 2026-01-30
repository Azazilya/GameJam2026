using UnityEngine;
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
        isAttacking = true;
        ResetFlicker(); 
        playerController.SetSlowdown(false); // Kembalikan kecepatan normal
        if (attackCollider != null) attackCollider.enabled = true;
    }

    void RotateHand()
    {
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
        if (isAttacking && collision.CompareTag("Enemy"))
        {
            EnemyHealthHandler enemy = collision.GetComponent<EnemyHealthHandler>();
            if (enemy != null)
            {
                PlayerStateData state = playerController.GetCurrentState(); //
                float damageMultiplier = 1f;
                float knockbackMultiplier = 1f;

                // Logika Charge: Makin lama ditahan, knockback bisa makin kuat
                if (chargeTimer >= state.superChargeThreshold)
                {
                    damageMultiplier = state.superChargeMultiplier;
                    knockbackMultiplier = 2f; // Super charge memberi extra knockback
                }
                else if (chargeTimer >= state.chargeThreshold)
                {
                    damageMultiplier = state.chargeDamageMultiplier;
                    knockbackMultiplier = 1.5f;
                }

                // Kalkulasi arah Knockback (dari Player ke Musuh)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                float finalKnockbackForce = state.knockbackForce * knockbackMultiplier;

                // Kirim Damage & Knockback
                enemy.TakeDamage(state.attackDamage * damageMultiplier);
                enemy.ApplyKnockback(knockbackDirection, finalKnockbackForce); // Pastikan fungsi ini ada di script musuh
            }
        }
    }
}