using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public float baseHealth = 1000f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float shootMultiCooldown = 6f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private GameObject experienceOrb;
    [SerializeField] private GameObject healPickUp;
    [SerializeField] private float healDropChance = 0.5f;

    private Animator animator;
    private float currentHealth;
    private Transform player;
    private float shootMultiTimer = 0f;
    private float attackTimer = 0f;
    private bool isAlive = true;
    private GameController gameController;
    private Rigidbody2D rb2d; // Thêm Rigidbody2D

    private BossUIManager bossUIManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
        }

        gameController = FindObjectOfType<GameController>();
        shootMultiTimer = shootMultiCooldown;
        attackTimer = attackCooldown;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on Boss!");
        }

        rb2d = GetComponent<Rigidbody2D>(); // Lấy Rigidbody2D
        if (rb2d == null)
        {
            Debug.LogWarning("Rigidbody2D not found on Boss!");
        }

        currentHealth = baseHealth;
        bossUIManager = FindObjectOfType<BossUIManager>();
        if (bossUIManager != null)
        {
            bossUIManager.SetBoss(this);
        }
    }

    void FixedUpdate() // Sử dụng FixedUpdate cho di chuyển với Rigidbody2D
    {
        if (!isAlive || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        if (rb2d != null)
        {
            Vector2 targetPosition = (Vector2)transform.position + direction * speed * Time.fixedDeltaTime;
            rb2d.MovePosition(targetPosition);
        }

        Vector2 currentScale = transform.localScale;
        if (direction.x > 0) currentScale.x = Mathf.Abs(currentScale.x);
        else if (direction.x < 0) currentScale.x = -Mathf.Abs(currentScale.x);
        transform.localScale = currentScale;

        shootMultiTimer -= Time.fixedDeltaTime;
        attackTimer -= Time.fixedDeltaTime;

        if (shootMultiTimer <= 0f)
        {
            ShootMultiDirectional();
            shootMultiTimer = shootMultiCooldown;
        }

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    void ShootMultiDirectional()
    {
        float[] angles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        foreach (float angle in angles)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, angle));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = direction * bulletSpeed;
        }
    }

    void Attack()
    {
        Vector2 playerDirection = (player.position - transform.position).normalized;
        float playerAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        GameObject targetedBullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, playerAngle));
        Rigidbody2D targetedRb = targetedBullet.GetComponent<Rigidbody2D>();
        if (targetedRb != null) targetedRb.velocity = playerDirection * bulletSpeed;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (bossUIManager != null) bossUIManager.UpdateHealth(currentHealth);
        Debug.Log($"Boss took {damage} damage, remaining health: {currentHealth}");

        if (currentHealth <= 0f)
        {
            isAlive = false;
            if (animator != null)
            {
                animator.SetTrigger("Die");
                StartCoroutine(DieAfterAnimation());
            }
            else
            {
                DieImmediately();
            }
        }
    }

    private void DieImmediately()
    {
        DropItems();
        NotifyBossDefeated();
        if (bossUIManager != null) bossUIManager.HideHealthBar();
        gameObject.SetActive(false);
    }

    private IEnumerator DieAfterAnimation()
    {
        enabled = false;
        if (rb2d != null) rb2d.velocity = Vector2.zero;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float dieAnimationLength = stateInfo.length > 0 ? stateInfo.length : 1f;

        yield return new WaitForSeconds(dieAnimationLength);

        DropItems();
        NotifyBossDefeated();
        if (bossUIManager != null) bossUIManager.HideHealthBar();
        gameObject.SetActive(false);
    }

    private void DropItems()
    {
        if (experienceOrb != null && UnityEngine.Random.value <= 0.7f)
        {
            Transform e = Instantiate(experienceOrb).transform;
            e.position = transform.position;
        }
        if (healPickUp != null && UnityEngine.Random.value <= healDropChance)
        {
            Transform h = Instantiate(healPickUp).transform;
            h.position = transform.position;
        }
    }

    private void NotifyBossDefeated()
    {
        if (gameController != null) gameController.OnBossDefeated();
    }

    public void ApplyScaling(float healthMultiplier, float damageMultiplier)
    {
        currentHealth = baseHealth * healthMultiplier;
        if (bossUIManager != null) bossUIManager.UpdateHealth(currentHealth);
        // Có thể scale thêm speed hoặc bulletSpeed nếu cần
    }

    public void SetTarget(GameObject playerObject)
    {
        player = playerObject.transform;
    }
}