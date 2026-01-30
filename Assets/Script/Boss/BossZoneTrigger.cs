using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    public GameObject barrier;
    public BossController boss;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        barrier.SetActive(true);
        boss.ActivateBoss();

        gameObject.SetActive(false); 
    }
}
