using UnityEngine;
using System.Collections;

public class BossHealthHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private BossData bossData;
    private float currentHP;
    private bool isDead = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer[] allRenderers;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float fadeDuration = 1.5f;

    // Fungsi ini dipanggil otomatis oleh ObjectPooler saat objek aktif
    void OnEnable()
    {
        ResetBoss();
    }

    public void ResetBoss()
    {
        isDead = false;
        
        // Kembalikan HP ke maksimal dari data
        if (bossData != null) currentHP = bossData.maxHP; 

        // Kembalikan visual ke semula (Alpha 1 dan warna Putih)
        foreach (var sr in allRenderers)
        {
            if (sr != null)
            {
                Color c = Color.white;
                c.a = 1f;
                sr.color = c;
            }
        }

        // Aktifkan kembali komponen yang dimatikan saat Die
        if (GetComponent<BossAI>()) GetComponent<BossAI>().enabled = true;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = true;
        
        Debug.Log($"Boss {bossData.bossName} Respawned! HP: {currentHP}");
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        Debug.Log($"Boss {bossData.bossName} hit! HP: {currentHP}/{bossData.maxHP}");

        StopAllCoroutines();
        StartCoroutine(DamageFlash());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red><b>BOSS DEFEATED!</b></color>");

        // Matikan AI dan kontrol agar tidak bergerak saat proses Fade Out
        if (GetComponent<BossAI>()) GetComponent<BossAI>().enabled = false;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator DamageFlash()
    {
        foreach (var sr in allRenderers) sr.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        foreach (var sr in allRenderers) sr.color = Color.white;
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            foreach (var sr in allRenderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = alpha;
                    sr.color = c;
                }
            }
            yield return null;
        }

        // Alih-alih Destroy, kita nonaktifkan agar masuk kembali ke Pool
        gameObject.SetActive(false); 
    }
}