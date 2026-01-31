using UnityEngine;
using System.Collections;

public class PlayerHealthHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer[] playerRenderers; 
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;
    
    private float staminaRegenTimer;
    private Color[] originalColors;
    private float lastStaminaDebug; // Untuk membatasi spam debug stamina

    void Awake()
    {
        stats.currentHP = stats.maxHP;
        stats.currentStamina = stats.maxStamina;

        originalColors = new Color[playerRenderers.Length];
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            originalColors[i] = playerRenderers[i].color;
        }
        
        Debug.Log($"<color=green>Game Started:</color> HP: {stats.currentHP}, Stamina: {stats.currentStamina}");
    }

    void Update()
    {
        HandleStaminaRegen();
    }

    public void TakeDamage(float amount)
    {
        stats.currentHP -= amount;
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);

        // DEBUG HP
        Debug.Log($"<color=red>Damage Taken:</color> -{amount} | Current HP: {stats.currentHP}/{stats.maxHP}");

        StopAllCoroutines();
        StartCoroutine(DamageFlash());

        if (stats.currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        stats.currentHP += amount;
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);
        
        Debug.Log($"<color=cyan>Healed:</color> +{amount} | Current HP: {stats.currentHP}");
    }

    public bool UseStamina(float amount)
    {
        if (stats.currentStamina >= amount)
        {
            stats.currentStamina -= amount;
            staminaRegenTimer = 0f;
            
            // Debug Stamina (Dibulatkan agar tidak spam setiap frame)
            if (Mathf.Abs(lastStaminaDebug - stats.currentStamina) > 5f) 
            {
                Debug.Log($"<color=yellow>Stamina Used:</color> {Mathf.Round(stats.currentStamina)}/{stats.maxStamina}");
                lastStaminaDebug = stats.currentStamina;
            }
            return true;
        }
        return false;
    }

    private void HandleStaminaRegen()
    {
        if (stats.currentStamina < stats.maxStamina)
        {
            float previousStamina = stats.currentStamina;
            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= stats.regenDelay)
            {
                stats.currentStamina += stats.staminaRegenRate * Time.deltaTime;
                stats.currentStamina = Mathf.Clamp(stats.currentStamina, 0, stats.maxStamina);

                // Debug saat Full
                if (stats.currentStamina >= stats.maxStamina && previousStamina < stats.maxStamina)
                {
                    Debug.Log("<color=yellow>Stamina:</color> Fully Recovered!");
                }
            }
        }
    }

    private void Die()
    {
        Debug.Log("<color=black><b>PLAYER DIED!</b></color>");
    }

    IEnumerator DamageFlash()
    {
        foreach (var sr in playerRenderers) sr.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            playerRenderers[i].color = originalColors[i];
        }
    }
}