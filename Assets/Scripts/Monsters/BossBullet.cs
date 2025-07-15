using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float lifetime = 5f; // Despawn after 5 seconds

    void Start()
    {
        Destroy(gameObject, lifetime); // Auto-despawn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assumes Player script with TakeDamage method
            collision.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}