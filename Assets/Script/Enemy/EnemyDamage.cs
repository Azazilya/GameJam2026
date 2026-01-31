using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private float damageCooldown = 1.0f; // Interval damage dalam detik
    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Cek apakah yang disentuh adalah Player
        if (collision.CompareTag("Player"))
        {
            // Logika interval: Waktu sekarang harus lebih besar dari waktu damage terakhir + cooldown
            if (Time.time >= lastDamageTime + damageCooldown) 
            {
                PlayerHealthHandler playerHealth = collision.GetComponent<PlayerHealthHandler>();
                
                if (playerHealth != null) 
                {
                    // Berikan damage berdasarkan statistik musuh
                    playerHealth.TakeDamage(stats.attackDamage);
                    
                    // Update waktu terakhir terkena damage agar interval berjalan
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    // Optional: Reset timer saat player keluar agar jika masuk lagi langsung terkena damage
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            lastDamageTime = 0; 
        }
    }
}