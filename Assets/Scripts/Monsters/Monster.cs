using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int tier = 1;
    [SerializeField] private float dropChance = 0.2f;
    [SerializeField] private GameObject healPickUp;
    [SerializeField] private GameObject experienceOrb;

    private Rigidbody2D rg2d;
    private GameObject targetGameobject;
    private Player targetPlayer;

    public float baseHealth;
    public float baseDamage;

    private float currentHealth;
    private float currentDamage;
    private float currentSpeed; // Sử dụng currentSpeed thay vì speed

    public float speed = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    public event System.Action OnDeath;

    private void Awake()
    {
        rg2d = GetComponent<Rigidbody2D>();
        lastAttackTime = -attackCooldown;
    }

    private void Start()
    {
        if (targetGameobject == null)
        {
            targetGameobject = GameObject.FindGameObjectWithTag("Player");
            if (targetGameobject != null)
                targetPlayer = targetGameobject.GetComponent<Player>();
        }

        ApplyTierStats();
        currentHealth = baseHealth;
        currentDamage = baseDamage;
        currentSpeed = speed; // Khởi tạo currentSpeed từ speed ban đầu
    }

    private void FixedUpdate()
    {
        if (targetGameobject != null)
        {
            Vector2 direction = (targetGameobject.transform.position - transform.position).normalized;
            // Sử dụng currentSpeed thay vì speed
            Vector2 targetPosition = (Vector2)transform.position + direction * currentSpeed * Time.fixedDeltaTime;
            rg2d.MovePosition(targetPosition);

            // Flip hướng nhìn
            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(currentDamage);
                lastAttackTime = Time.time;
                Debug.Log($"{gameObject.name} attacked Player for {currentDamage} damage!");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.CeilToInt(damage);
        Debug.Log($"{gameObject.name} took {damage} damage, remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Drop item
        if (healPickUp != null && Random.value <= dropChance)
        {
            Instantiate(healPickUp, transform.position, Quaternion.identity);
        }
        if (experienceOrb != null)
        {
            Instantiate(experienceOrb, transform.position, Quaternion.identity);
        }

        // Gọi sự kiện OnDeath
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void ApplyTierStats()
    {
        switch (tier)
        {
            case 1:
                speed = 0.3f;
                baseHealth = 10;
                baseDamage = 5;
                transform.localScale = Vector3.one;
                break;
            case 2:
                speed = 0.5f;
                baseHealth = 5;
                baseDamage = 3;
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                break;
            case 3:
                speed = 0.1f;
                baseHealth = 50;
                baseDamage = 10;
                transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                break;
            default:
                Debug.LogWarning("Invalid tier: " + tier);
                break;
        }
    }

    public void ApplyScaling(float healthMultiplier, float damageMultiplier, float speedMultiplier)
    {
        ApplyTierStats();
        currentHealth = Mathf.CeilToInt(baseHealth * healthMultiplier);
        currentDamage = Mathf.CeilToInt(baseDamage * damageMultiplier);
        currentSpeed = speed * speedMultiplier; // Sử dụng float để giữ độ chính xác

        Debug.Log($"{gameObject.name} Tier {tier} Stats: HP = {currentHealth}, Damage = {currentDamage}, Speed = {currentSpeed}");
    }

    public void SetTarget(GameObject playerObject)
    {
        targetGameobject = playerObject;
        targetPlayer = playerObject.GetComponent<Player>();
    }
}