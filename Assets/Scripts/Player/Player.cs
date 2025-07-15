using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float attackRange = 1f;
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    public float damageReduction = 0.2f;
    public float baseSkillCooldown = 0f;
    public float skillCooldown = 0f;

    [Header("Combat")]
    public float attackSpeed = 0.5f;
    private float nextAttackTime;
    public GameObject bulletPrefab;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    public GameObject slashEffect;
    public GameObject skillEffect;

    [Header("References")]
    public InputActionAsset actionAsset;
    private LevelUpManager levelUpManager;
    private UIManager uiManager;
    public RuntimeAnimatorController bladeAlphaController;
    public RuntimeAnimatorController techGammaController;
    public Sprite bladeAlphaSprite;
    public Sprite techGammaSprite;

    private string selectedClass;
    private int attackCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelUpManager = FindObjectOfType<LevelUpManager>();
        uiManager = FindObjectOfType<UIManager>();
        selectedClass = PlayerPrefs.GetString("SelectedClass", "DefaultClass").ToUpper();
        currentHealth = maxHealth;

        // Cấu hình Rigidbody2D
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Thiết lập class
        if (selectedClass == "BLADE ALPHA")
        {
            maxHealth = 120f;
            damageReduction = 0.1f;
            damage = 20f;
            attackRange = 0.5f;
            attackSpeed = 1f;
            moveSpeed = 0.6f;
            damageReduction = 0.05f;
            baseSkillCooldown = 10f;
            currentHealth = maxHealth;
        }
        else if (selectedClass == "TECH GAMA")
        {
            damage = 15f;
            attackRange = 1.4f;
            attackSpeed = 0.8f;
            moveSpeed = 0.8f;
            baseSkillCooldown = 16f;
        }
        skillCooldown = baseSkillCooldown;

        // Gán Animator Controller và Sprite
        if (animator != null && spriteRenderer != null)
        {
            if (selectedClass == "BLADE ALPHA")
            {
                animator.runtimeAnimatorController = bladeAlphaController;
                spriteRenderer.sprite = bladeAlphaSprite;
            }
            else if (selectedClass == "TECH GAMA")
            {
                animator.runtimeAnimatorController = techGammaController;
                spriteRenderer.sprite = techGammaSprite;
            }
            else
            {
                Debug.LogWarning("Unknown class: " + selectedClass);
            }
        }

        actionAsset.Enable();
        UpdateUI();
        animator.SetBool("Idle", true);
        nextAttackTime = Time.time;

        // Đảm bảo có ít nhất một BoxCollider2D không phải Trigger
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        bool hasNonTriggerCollider = false;
        foreach (var collider in colliders)
        {
            if (!collider.isTrigger)
            {
                hasNonTriggerCollider = true;
                break;
            }
        }
        if (!hasNonTriggerCollider)
        {
            BoxCollider2D physicsCollider = gameObject.AddComponent<BoxCollider2D>();
            physicsCollider.size = new Vector2(0.5f, 0.5f);
            physicsCollider.isTrigger = false;
        }
    }

    private void Update()
    {

        if (skillCooldown > 0)
        {
            skillCooldown -= Time.deltaTime;
            if (skillCooldown < 0) skillCooldown = 0;
            uiManager?.UpdateSkillCooldown(skillCooldown, baseSkillCooldown); // Truyền baseSkillCooldown
        }

        // Kích hoạt kỹ năng
        if (Input.GetKeyDown(KeyCode.E) && skillCooldown <= 0)
        {
            UseSkill();
        }

        // Cập nhật animation di chuyển
        bool isMoving = moveInput.magnitude > 0;
        animator.SetBool("Run", isMoving);
        animator.SetBool("Idle", !isMoving);

        if (spriteRenderer != null && isMoving)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }

        // Tấn công tự động
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackSpeed;
        }

        // Kiểm tra chết
        if (currentHealth <= 0 && animator != null)
        {
            animator.SetBool("Die", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
            rb.velocity = Vector2.zero;
            enabled = false;
            Invoke(nameof(NotifyGameOver), 1f);
        }
    }

    void FixedUpdate()
    {
        InputAction moveAction = actionAsset.FindActionMap("Gameplay")?.FindAction("Movement");
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        else
        {
            Debug.LogWarning("Movement action not found in Gameplay map!", this);
        }

        if (rb != null)
        {
            float speed = moveSpeed * moveInput.magnitude;
            Vector2 movement = moveInput.normalized * speed;
            rb.velocity = movement;
        }

        InputAction skillAction = actionAsset.FindActionMap("Gameplay")?.FindAction("Skill");
        if (skillAction != null && skillAction.WasPressedThisFrame())
        {
            UseSkill();
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (uiManager != null)
        {
            uiManager.UpdateXPBar(currentXP, xpToNextLevel);
        }

        while (currentXP >= xpToNextLevel && levelUpManager != null)
        {
            currentXP -= xpToNextLevel;
            levelUpManager.OnLevelUp();
            xpToNextLevel += 10;
            UpdateUI();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    void Attack()
    {
        if (animator != null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
            // Lọc danh sách mục tiêu hợp lệ
            System.Collections.Generic.List<Collider2D> targets = new System.Collections.Generic.List<Collider2D>();
            foreach (Collider2D hit in hitColliders)
            {
                Monster monster = hit.GetComponent<Monster>();
                Boss boss = hit.GetComponent<Boss>();
                if (monster != null || boss != null)
                {
                    targets.Add(hit);
                }
            }
            if (targets.Count > 0)
            {
                // Chọn ngẫu nhiên một mục tiêu
                int randomIndex = Random.Range(0, targets.Count);
                Collider2D target = targets[randomIndex];
                Monster monsterTarget = target.GetComponent<Monster>();
                Boss bossTarget = target.GetComponent<Boss>();

                if (selectedClass == "BLADE ALPHA")
                {
                    float attackDamage = damage * (1 + attackCount * 0.1f);
                    if (monsterTarget != null)
                    {
                        monsterTarget.TakeDamage(attackDamage);
                        if (slashEffect != null)
                        {
                            Vector3 effectPos = monsterTarget.transform.position + new Vector3(-0.7f, 0, 0);
                            GameObject effect = Instantiate(slashEffect, effectPos, Quaternion.identity);
                            Destroy(effect, 0.2f);
                        }
                        AudioManager.Instance?.PlayBladeSlash(); // Phát sound effect chém thường
                    }
                    else if (bossTarget != null)
                    {
                        bossTarget.TakeDamage(attackDamage);
                        if (slashEffect != null)
                        {
                            Vector3 effectPos = bossTarget.transform.position + new Vector3(-0.7f, 0, 0);
                            GameObject effect = Instantiate(slashEffect, effectPos, Quaternion.identity);
                            Destroy(effect, 0.1f);
                        }
                        AudioManager.Instance?.PlayBladeSlash(); // Phát sound effect chém thường
                    }
                    attackCount = (attackCount + 1) % 3;
                }
                else if (selectedClass == "TECH GAMA")
                {
                    if (bulletPrefab != null && firePoint != null)
                    {
                        Vector3 targetPosition = monsterTarget != null ? monsterTarget.transform.position : bossTarget.transform.position;
                        Vector3 direction = (targetPosition - firePoint.position).normalized;
                        if (direction.magnitude >= 0.1f)
                        {
                            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                            Bullet bulletScript = bullet.GetComponent<Bullet>();
                            if (bulletScript != null)
                            {
                                bulletScript.Initialize((Vector2)direction, damage); // Truyền damage từ Player
                            }
                        }
                        AudioManager.Instance?.PlayTechShoot(); // Phát sound effect bắn thường
                    }
                }
            }
        }
    }

    void UseSkill()
    {
        switch (selectedClass)
        {
            case "BLADE ALPHA":
                skillCooldown = baseSkillCooldown;

                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
                foreach (Collider2D hit in hitColliders)
                {
                    Monster monster = hit.GetComponent<Monster>();
                    Boss boss = hit.GetComponent<Boss>();

                    if (monster != null)
                    {
                        monster.TakeDamage(damage * 2f);

                        if (slashEffect != null)
                        {
                            Vector3 effectPos = monster.transform.position + new Vector3(-0.7f, 0, 0);
                            GameObject effect = Instantiate(slashEffect, effectPos, Quaternion.identity);
                            Destroy(effect, 0.2f);
                        }
                    }
                    else if (boss != null)
                    {
                        boss.TakeDamage(damage * 2f);

                        if (slashEffect != null)
                        {
                            Vector3 effectPos = boss.transform.position + new Vector3(-0.7f, 0, 0);
                            GameObject effect = Instantiate(slashEffect, effectPos, Quaternion.identity);
                            Destroy(effect, 0.1f);
                        }
                    }
                }

                if (skillEffect != null)
                {
                    GameObject effect = Instantiate(skillEffect, transform.position, Quaternion.identity);
                    Destroy(effect, 0.5f);
                }
                AudioManager.Instance?.PlayBladeSkill(); // Phát sound effect kỹ năng
                break;
            case "TECH GAMA":
                skillCooldown = baseSkillCooldown;
                if (turretPrefab != null)
                {
                    Instantiate(turretPrefab, transform.position, Quaternion.identity);
                }
                // Thêm logic bắn một mục tiêu (tùy chọn)
                Collider2D[] techHitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
                System.Collections.Generic.List<Collider2D> techTargets = new System.Collections.Generic.List<Collider2D>();
                foreach (Collider2D hit in techHitColliders)
                {
                    Monster monster = hit.GetComponent<Monster>();
                    Boss boss = hit.GetComponent<Boss>();
                    if (monster != null || boss != null)
                    {
                        techTargets.Add(hit);
                    }
                }
                if (techTargets.Count > 0)
                {
                    int randomIndex = Random.Range(0, techTargets.Count);
                    Collider2D target = techTargets[randomIndex];
                    Vector3 targetPosition = target.transform.position;
                    if (bulletPrefab != null && firePoint != null)
                    {
                        Vector3 direction = (targetPosition - firePoint.position).normalized;
                        if (direction.magnitude >= 0.1f)
                        {
                            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                            Bullet bulletScript = bullet.GetComponent<Bullet>();
                            if (bulletScript != null)
                            {
                                bulletScript.Initialize((Vector2)direction, damage); // Truyền damage từ Player
                            }
                        }
                    }
                }
                AudioManager.Instance?.PlayTechSkill(); // Phát sound effect kỹ năng
                break;
        }
        uiManager?.UpdateSkillCooldown(skillCooldown, baseSkillCooldown);
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (!upgrade.isWeapon)
        {
            ApplyStatUpgrade(upgrade.upgradeName);
        }
        UpdateUI();
    }

    void ApplyStatUpgrade(string statName)
    {
        if (statName == "Health")
        {
            maxHealth += 20f;
            UpdateHealthUI();
            Debug.Log("Health Boosted! Max Health: " + maxHealth);
        }
        else if (statName == "Speed")
        {
            moveSpeed += 0.1f;
            Debug.Log("Speed Increased! New Speed: " + moveSpeed);
        }
        else if (statName == "Armor")
        {
            damageReduction = Mathf.Min(damageReduction + 0.05f, 0.8f);
            Debug.Log("Armor Upgraded! Damage Reduction: " + (damageReduction * 100) + "%");
        }
        else if (statName == "Attack Speed")
        {
            attackSpeed = Mathf.Max(attackSpeed - 0.1f, 0.5f);
            Debug.Log("Attack Speed Increased! New Attack Speed: " + attackSpeed + "s");
        }
        else if (statName == "Attack Range")
        {
            attackRange += 0.1f;
            Debug.Log("Attack Range Increased! New Attack Range: " + attackRange);
        }
        else if (statName == "Damage")
        {
            damage += 5f;
            Debug.Log("Damage Increased! New Damage: " + damage);
        }
        else if (statName == "Cooldown")
        {
            baseSkillCooldown = Mathf.Max(baseSkillCooldown - 1f, 5f);
            if (skillCooldown > 0)
            {
                skillCooldown = Mathf.Max(skillCooldown - 1f, 0f);
            }
            Debug.Log("Cooldown Reduced! New Skill Cooldown: " + skillCooldown + "s");
        }
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = damage * (1f - damageReduction);
        currentHealth -= effectiveDamage;
        UpdateHealthUI();
    }

    void UpdateUI()
    {
        if (uiManager != null && levelUpManager != null)
        {
            uiManager.UpdateHealth(currentHealth, maxHealth);
            uiManager.UpdateXPBar(currentXP, xpToNextLevel);
            uiManager.UpdateLevel(levelUpManager.currentLevel);
        }
    }

    void UpdateHealthUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ExperienceOrb orb = other.GetComponent<ExperienceOrb>();
        if (orb != null)
        {
            AddXP(orb.xpValue);
            Destroy(other.gameObject);
        }
    }

    void NotifyGameOver()
    {
        GameController controller = FindObjectOfType<GameController>();
        if (controller != null)
        {
            controller.SendMessage("ShowGameOver");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}