using UnityEngine;
using System.Collections;

public class EnemyHealthHandler : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private float currentHP;
    private Color originalColor;

    void Awake() // Menggunakan Awake untuk menyimpan warna asli sekali saja
    {
        if (enemyRenderer != null) originalColor = enemyRenderer.color;
    }

    // Fungsi ini dipanggil setiap kali objek diaktifkan (termasuk saat respawn dari pool)
    void OnEnable() 
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        if (enemyStats != null)
        {
            currentHP = enemyStats.maxHP; // Mengembalikan HP ke kondisi maksimal
            Debug.Log($"<color=green>Enemy Respawned:</color> HP Reset to {currentHP}");
        }
        
        // Memastikan warna musuh kembali normal saat respawn
        if (enemyRenderer != null) enemyRenderer.color = originalColor;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        
        Debug.Log($"<color=orange>Enemy Hit!</color> HP: {currentHP}");

        StopAllCoroutines();
        StartCoroutine(FlashRed());

        if (currentHP <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        enemyRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        enemyRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        gameObject.SetActive(false); // Kembali ke pool
    }
}