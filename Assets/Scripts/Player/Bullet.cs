using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 15f; // Sát thương của đạn, có thể được cập nhật từ Player
    public float lifetime = 2f; // Thời gian sống của đạn
    public float speed = 10f; // Tốc độ di chuyển

    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0; // Tắt trọng lực
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Giữ ổn định xoay
        rb.bodyType = RigidbodyType2D.Dynamic; // Đảm bảo động
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Hủy đạn sau lifetime giây
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            if (direction != Vector2.zero)
            {
                rb.velocity = direction * speed; // Di chuyển đạn
                Debug.Log("Bullet moving with velocity: " + rb.velocity + " at position: " + transform.position);
            }
            else
            {
                Debug.LogWarning("Bullet direction is zero! Position: " + transform.position);
            }
        }
        else
        {
            Debug.LogError("Rigidbody2D is null! Position: " + transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Monster monster = other.GetComponent<Monster>();
        Boss boss = other.GetComponent<Boss>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            Destroy(gameObject); // Hủy đạn khi trúng
        }
        else if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject); // Hủy đạn khi trúng
        }
    }

    public void Initialize(Vector2 direction, float damage)
    {
        this.direction = direction; // Sử dụng direction đã chuẩn hóa
        this.damage = damage; // Cập nhật damage từ Player
    }
}