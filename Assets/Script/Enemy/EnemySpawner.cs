using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public string enemyTag; 
    public float spawnInterval = 3f;
    public float spawnRadius = 5f;

    [Header("Limit Settings")]
    [SerializeField] private bool useLimit = true;
    [SerializeField] private int maxEnemiesInWorld = 10;
    [SerializeField] private float checkInterval = 0.5f; // Optimasi: tidak cek jumlah setiap frame

    private float spawnTimer;
    private float checkTimer;
    private int currentEnemyCount;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        checkTimer += Time.deltaTime;

        // Optimasi: Hanya hitung musuh di dunia setiap 'checkInterval' detik
        if (checkTimer >= checkInterval)
        {
            UpdateEnemyCount();
            checkTimer = 0;
        }

        if (spawnTimer >= spawnInterval)
        {
            if (!useLimit || currentEnemyCount < maxEnemiesInWorld)
            {
                SpawnEnemy();
            }
            spawnTimer = 0;
        }
    }

    void UpdateEnemyCount()
    {
        // Mencari semua objek aktif dengan tag tertentu
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        currentEnemyCount = enemies.Length;
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        
        // Panggil dari pool
        ObjectPooler.Instance.SpawnFromPool(enemyTag, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}