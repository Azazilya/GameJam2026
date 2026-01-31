using UnityEngine;

public class BossDamageDealer : MonoBehaviour
{
    [SerializeField] private BossData bossData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang terkena adalah Player menggunakan script yang sudah ada
        if (collision.CompareTag("Player"))
        {
            PlayerHealthHandler playerHealth = collision.GetComponent<PlayerHealthHandler>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(bossData.attackDamage);
                Debug.Log($"Boss melukai Player sebesar: {bossData.attackDamage}");
            }
        }
    }
}