using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 1f;
    public float damage;
    public float lifetime = 5f; // Thời gian sống của đạn (giây)

    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>(); // Thêm Rigidbody2D nếu chưa có
            rb.gravityScale = 0; // Tắt trọng lực
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Giữ ổn định xoay
        }
        else
        {
            rb.velocity = Vector2.zero; // Reset velocity trước khi gán mới
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Hủy đạn sau lifetime giây
    }

    void FixedUpdate()
    {
        if (rb != null && direction != Vector2.zero)
        {
            rb.velocity = direction * speed; // Đảm bảo di chuyển
            Debug.Log("Bullet moving with velocity: " + rb.velocity); // Debug
        }
        else
        {
            Debug.LogWarning("Bullet direction is zero or Rigidbody2D is null!");
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

    public void Initialize(float damage, Vector2 direction)
    {
        this.damage = damage;
        this.direction = direction.normalized; // Chuẩn hóa vector
        Debug.Log("Bullet initialized with direction: " + this.direction); // Debug
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Hủy khi ra khỏi tầm nhìn
    }
}