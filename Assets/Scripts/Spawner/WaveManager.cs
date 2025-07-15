using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Monster Prefabs")]
    public GameObject zombiePrefab;
    public GameObject poisonPrefab;
    public GameObject tankPrefab;
    public GameObject bossPrefab;

    [Header("Default Stats")]
    public int zombieBaseHP = 50;
    public int zombieBaseDamage = 2;
    public int poisonBaseHP = 30;
    public int poisonBaseDamage = 4;
    public int tankBaseHP = 200;
    public int tankBaseDamage = 30;
    public int bossBaseHP = 2000;
    public int bossBaseDamage = 50;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private float maxSpawnInterval = 0.5f;
    [SerializeField] private int maxMonstersPerWave = 10;
    [SerializeField] private Vector2 mapSize = new Vector2(10f, 10f);

    private int zombieHP;
    private int zombieDamage;
    private int poisonHP;
    private int poisonDamage;
    private int tankHP;
    private int tankDamage;
    private int bossHP;
    private int bossDamage;

    private int currentWave = 1;
    private float baseHealthMultiplier = 1f;
    private float baseDamageMultiplier = 1f;
    private float baseSpeedMultiplier = 1f;
    private int currentMonsterCount = 0;
    private bool bossSpawned = false; // Theo dõi boss đã spawn chưa
    private bool isBossWaveActive = false; // Theo dõi wave 20 đang xử lý boss

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.wave = currentWave;
        }
        ApplyScaling(baseHealthMultiplier, baseDamageMultiplier);
        StartCoroutine(SpawnMonstersContinuously());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            SkipWave();
            if (currentWave == 20) // Kiểm tra nếu đã đến wave 20
            {
                CheckSpecialWave(); // Kích hoạt spawn boss ngay
            }
        }
    }

    IEnumerator SpawnMonstersContinuously()
    {
        while (true)
        {
            int maxMonsters = Mathf.Min(maxMonstersPerWave + (currentWave - 1) * 5, 50);

            // Spawn quái trong 30 giây cho mọi wave
            float waveTimer = 0f;
            while (waveTimer < 30f)
            {
                if (currentWave < 20 || !isBossWaveActive) // Chỉ spawn quái thông thường nếu không phải wave 20 với boss
                {
                    SpawnMonster();
                }
                float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(spawnInterval);
                waveTimer += spawnInterval;
            }

            // Chuyển wave sau 30 giây
            UpdateWave();
            ApplyScaling(baseHealthMultiplier, baseDamageMultiplier);

            yield return null;
        }
    }

    void UpdateWave()
    {
        currentWave++;
        if (gameController != null)
        {
            gameController.wave = currentWave;
        }
        UpdateDifficultyScaling();
        CheckSpecialWave();
    }

    void CheckSpecialWave()
    {
        if (currentWave == 20)
        {
            isBossWaveActive = true;
            SpawnBoss();
        }
    }

    public void SkipWave()
    {
        currentWave += 1;
        if (gameController != null)
        {
            gameController.wave = currentWave;
        }
    }

    void ApplyScaling(float healthMultiplier, float damageMultiplier)
    {
        zombieHP = (int)(zombieBaseHP * healthMultiplier);
        zombieDamage = (int)(zombieBaseDamage * damageMultiplier);
        poisonHP = (int)(poisonBaseHP * healthMultiplier);
        poisonDamage = (int)(poisonBaseDamage * damageMultiplier);
        tankHP = (int)(tankBaseHP * healthMultiplier);
        tankDamage = (int)(tankBaseDamage * damageMultiplier);
        bossHP = (int)(bossBaseHP * healthMultiplier);
        bossDamage = (int)(bossBaseDamage * damageMultiplier);
    }

    void SpawnMonster()
    {
        float zombieWeight = 100f;
        float poisonWeight = 0f;
        float tankWeight = 0f;

        if (currentWave > 5)
        {
            poisonWeight += 10f;
            zombieWeight -= 10f;
        }
        if (currentWave > 10)
        {
            tankWeight += 10f;
            poisonWeight += 10f;
            zombieWeight -= 20f;
        }
        if (currentWave > 15)
        {
            tankWeight += 10f;
            zombieWeight -= 10f;
        }

        float roll = Random.Range(0f, 100f);
        GameObject monsterToSpawn = null;
        if (roll < zombieWeight) monsterToSpawn = zombiePrefab;
        else if (roll < zombieWeight + poisonWeight) monsterToSpawn = poisonPrefab;
        else monsterToSpawn = tankPrefab;

        if (monsterToSpawn != null)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            GameObject monster = Instantiate(monsterToSpawn, spawnPos, Quaternion.identity);
            currentMonsterCount++;

            Monster stats = monster.GetComponent<Monster>();
            if (stats != null)
            {
                stats.ApplyScaling(baseHealthMultiplier, baseDamageMultiplier, baseSpeedMultiplier);
                stats.SetTarget(GameObject.FindGameObjectWithTag("Player"));
                stats.OnDeath += OnMonsterDeath;
            }
        }
        else
        {
            Debug.LogWarning("Monster prefab chưa được gán!");
        }
    }

    public void SpawnBoss()
    {
        if (bossPrefab != null && !bossSpawned)
        {
            Vector2 spawnPos = new Vector2(1.9f, -1.61f); // Vị trí cố định
            GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            currentMonsterCount++;
            bossSpawned = true;

            // Lấy component Monster
            Monster monsterStats = boss.GetComponent<Monster>();
            if (monsterStats != null)
            {
                monsterStats.ApplyScaling(baseHealthMultiplier, baseDamageMultiplier, baseSpeedMultiplier);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    monsterStats.SetTarget(player);
                }
                else
                {
                    Debug.LogWarning("Player not found when setting target for Boss!");
                }
                monsterStats.OnDeath += OnMonsterDeath;
            }

            // Lấy component Boss
            Boss bossScript = boss.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.ApplyScaling(baseHealthMultiplier, baseDamageMultiplier);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    bossScript.SetTarget(player); // Đảm bảo target cho Boss
                }
                BossUIManager bossUIManager = FindObjectOfType<BossUIManager>();
                if (bossUIManager != null)
                {
                    bossUIManager.ShowHealthBarUI();
                    bossUIManager.SetBoss(bossScript); // Hiển thị thanh máu
                }
                else
                {
                    Debug.LogWarning("BossUIManager not found in Scene!");
                }
            }
            else
            {
                Debug.LogWarning("Boss script not found on bossPrefab!");
            }
        }
        else
        {
            Debug.LogWarning("BossPrefab chưa được gán hoặc đã spawn!");
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        if (mapSize.x <= 0f || mapSize.y <= 0f)
        {
            return transform.position; // Fallback nếu mapSize không hợp lệ
        }

        float x = Random.Range(-mapSize.x / 2f, mapSize.x / 2f) + transform.position.x;
        float y = Random.Range(-mapSize.y / 2f, mapSize.y / 2f) + transform.position.y;
        Vector2 spawnPos = new Vector2(x, y);

        int maxAttempts = 10;
        int attempts = 0;
        while (Physics2D.OverlapCircle(spawnPos, 0.5f) != null && attempts < maxAttempts)
        {
            x = Random.Range(-mapSize.x / 2f, mapSize.x / 2f) + transform.position.x;
            y = Random.Range(-mapSize.y / 2f, mapSize.y / 2f) + transform.position.y;
            spawnPos = new Vector2(x, y);
            attempts++;
        }

        return spawnPos;
    }

    void OnMonsterDeath()
    {
        currentMonsterCount--;
    }

    void UpdateDifficultyScaling()
    {
        baseHealthMultiplier *= 1.1f;
        baseDamageMultiplier *= 1.05f;
        baseSpeedMultiplier *= 1.05f;

        if (currentWave >= 21 && currentWave % 10 == 0)
        {
            baseHealthMultiplier *= 1.5f;
            baseDamageMultiplier *= 1.5f;
            baseSpeedMultiplier *= 1.1f; 
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public bool IsBossSpawned()
    {
        return bossSpawned;
    }

    public void SetWave(int newWave)
    {
        currentWave = newWave;
    }
}