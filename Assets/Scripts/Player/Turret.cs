using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Stats")]
    public float damage = 15f; // Sát thương theo tài liệu
    public float range = 6f; // Phạm vi 6 ô
    public float attackSpeed = 1f; // Tốc độ bắn (1 giây/lần)
    private float nextAttackTime;
    public GameObject bulletPrefab; // Prefab đạn

    [Header("References")]
    private Transform target; // Quái mục tiêu
    private Animator animator; // Thêm Animator

    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, 12f); 
        nextAttackTime = Time.time;
    }

    void Update()
    {
        // Tìm quái gần nhất trong phạm vi
        FindTarget();
        if (target != null && Time.time >= nextAttackTime)
        {
            Shoot();
            nextAttackTime = Time.time + attackSpeed;
        }
    }

    void FindTarget()
    {
        // Logic tìm quái (giả sử có tag "Enemy")
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && target != null && animator != null)
        {
            // Kích hoạt animation bắn
            animator.SetBool("Shoot", true);

            Vector2 direction = (target.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            TurretBullet bulletScript = bullet.GetComponent<TurretBullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(damage, direction);
            }
            Debug.Log("Turret shoots at enemy!");
        }
    }

    // Gọi từ animation event khi animation Shoot kết thúc
    void OnShootAnimationEnd()
    {
        if (animator != null)
        {
            animator.SetBool("Shoot", false); // Tắt animation sau khi kết thúc
        }
    }
}