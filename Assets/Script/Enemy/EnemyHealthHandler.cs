using UnityEngine;
using System.Collections;

public class EnemyHealthHandler : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private Rigidbody2D rb;
    private float currentHP;
    private bool isKnockedBack; // Flag agar AI tahu kapan harus berhenti
    private Color originalColor;

    void Awake() // Menggunakan Awake untuk menyimpan warna asli sekali saja
    {
        rb = GetComponent<Rigidbody2D>();
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

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (gameObject.activeSelf)
        {
            StopCoroutine("KnockbackRoutine"); // Reset jika terkena hit lagi
            StartCoroutine(KnockbackRoutine(direction, force));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        isKnockedBack = true;
        
        // Berikan dorongan instan menggunakan Impulse
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Musuh terpental selama durasi ini (bisa disesuaikan, permintaan: 1 detik)
        yield return new WaitForSeconds(1.0f);

        // Hentikan paksa momentum agar musuh tidak meluncur selamanya
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
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