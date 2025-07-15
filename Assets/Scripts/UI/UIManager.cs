using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Health and XP")]
    public Slider healthBar;
    public Slider xpBar;
    public Text levelText;

    [Header("Additional Stats")]
    public Text enemiesKilledText;
    public Text playTimeText;
    public Text waveText;

    [Header("Skill Cooldown")]
    public Image bladeAlphaCooldownImage; // UI cho Blade Alpha
    public Text bladeAlphaCooldownText;
    public Image techGammaCooldownImage;  // UI cho Tech Gamma
    public Text techGammaCooldownText;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Text victoryText;
    public Text statsText;
    public Button endlessButton;
    public Button endGameButton;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public Text endGameText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Text gameOverStatsText;
    public Button restartButton;
    public Button backToMainMenuButton;

    private Player player;
    private LevelUpManager levelUpManager;
    private GameController gameController;
    private BossUIManager bossUIManager;

    void Start()
    {
        player = FindObjectOfType<Player>();
        levelUpManager = FindObjectOfType<LevelUpManager>();
        gameController = FindObjectOfType<GameController>();
        bossUIManager = FindObjectOfType<BossUIManager>();

        if (player == null) Debug.LogError("Player not found in scene!");
        else if (levelUpManager == null) Debug.LogError("LevelUpManager not found in scene!");
        else if (gameController == null) Debug.LogError("GameController not found in scene!");
        else if (bossUIManager == null) Debug.LogWarning("BossUIManager not found in scene!");
        else
        {
            UpdateHealth(player.currentHealth, player.maxHealth);
            UpdateXPBar(player.currentXP, player.xpToNextLevel);
            UpdateLevel(levelUpManager.currentLevel);
            UpdateEnemiesKilled(gameController.enemiesKilled);
            UpdatePlayTime(gameController.playTime);
            UpdateWave(gameController.wave);
            UpdateSkillCooldown(0f, player.baseSkillCooldown);
        }

        // Gán sự kiện cho các button
        if (endlessButton != null) endlessButton.onClick.AddListener(OnEndlessClicked);
        if (endGameButton != null) endGameButton.onClick.AddListener(OnEndGameClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (backToMainMenuButton != null) backToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);

        // Chỉ bật UI của class được chọn, tắt các UI khác
        string className = PlayerPrefs.GetString("SelectedClass", "DefaultClass").ToUpper();
        if (bladeAlphaCooldownImage != null) bladeAlphaCooldownImage.gameObject.SetActive(className == "BLADE ALPHA");
        if (bladeAlphaCooldownText != null) bladeAlphaCooldownText.gameObject.SetActive(className == "BLADE ALPHA");
        if (techGammaCooldownImage != null) techGammaCooldownImage.gameObject.SetActive(className == "TECH GAMA");
        if (techGammaCooldownText != null) techGammaCooldownText.gameObject.SetActive(className == "TECH GAMA");
    }

    void Update()
    {
        if (player != null && levelUpManager != null && gameController != null &&
            healthBar != null && xpBar != null && levelText != null &&
            enemiesKilledText != null && playTimeText != null &&
            (bladeAlphaCooldownImage != null || techGammaCooldownImage != null) &&
            (bladeAlphaCooldownText != null || techGammaCooldownText != null))
        {
            UpdateHealth(player.currentHealth, player.maxHealth);
            UpdateXPBar(player.currentXP, player.xpToNextLevel);
            UpdateLevel(levelUpManager.currentLevel);
            UpdateEnemiesKilled(gameController.enemiesKilled);
            UpdatePlayTime(gameController.playTime);
            UpdateWave(gameController.wave);
            UpdateSkillCooldown(player.skillCooldown, player.baseSkillCooldown);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void UpdateXPBar(float currentXP, float xpToNextLevel)
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }

    public void UpdateLevel(int currentLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel}";
        }
    }

    public void UpdateEnemiesKilled(int enemiesKilled)
    {
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = $"Kill: {enemiesKilled}";
        }
    }

    public void UpdatePlayTime(float playTime)
    {
        if (playTimeText != null)
        {
            playTimeText.text = $"Time: {playTime:F2}s";
        }
    }

    public void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {wave}";
            if (wave == 5 || wave == 10 || wave == 15 || wave == 20)
                waveText.color = Color.red;
            else
                waveText.color = Color.green;
        }
    }

    public void UpdateSkillCooldown(float cooldownTime, float maxCooldown)
    {
        string className = PlayerPrefs.GetString("SelectedClass", "DefaultClass").ToUpper();

        float fill = cooldownTime > 0 ? 1f - (cooldownTime / maxCooldown) : 1f;
        string text = cooldownTime > 0 ? cooldownTime.ToString("F1") + "s" : "";

        if (className == "BLADE ALPHA")
        {
            if (bladeAlphaCooldownImage != null) bladeAlphaCooldownImage.fillAmount = fill;
            if (bladeAlphaCooldownText != null) bladeAlphaCooldownText.text = text;
        }
        else if (className == "TECH GAMA")
        {
            if (techGammaCooldownImage != null) techGammaCooldownImage.fillAmount = fill;
            if (techGammaCooldownText != null) techGammaCooldownText.text = text;
        }
    }

    public void ShowVictory(string stats)
    {
        if (victoryPanel != null && victoryText != null && statsText != null)
        {
            Time.timeScale = 0f;
            victoryPanel.SetActive(true);
            victoryText.text = "Victory!";
            statsText.text = stats;
        }
    }

    public void ShowGameOver(string stats)
    {
        if (gameOverPanel != null && gameOverText != null && gameOverStatsText != null)
        {
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
            gameOverText.text = "Game Over";
            gameOverStatsText.text = stats;
        }
    }

    void OnEndlessClicked()
    {
        if (victoryPanel != null && gameController != null && bossUIManager != null)
        {
            victoryPanel.SetActive(false);
            Time.timeScale = 1f; // Khôi phục thời gian để tiếp tục gameplay
            bossUIManager.healthBarUI.SetActive(false); // Ẩn thanh máu boss
            gameController.StartEndlessMode(21); // Gọi phương thức với wave 21
            Debug.Log("Starting Endless Mode at Wave 21");
        }
    }

    void OnEndGameClicked()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        ShowEnding();
        AudioManager.Instance.PlayVictoryMusic();
    }

    void OnRestartClicked()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        SceneManager.LoadScene(1);
    }

    void OnBackToMainMenuClicked()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        SceneManager.LoadScene(0);
        Debug.Log("Back to Main Menu");
    }

    void ShowEnding()
    {
        string selectedClass = PlayerPrefs.GetString("SelectedClass", "DefaultClass");
        string endingText = GetEndingText(selectedClass);
        if (victoryText != null && victoryPanel != null)
        {
            endGamePanel.SetActive(true);
            endGameText.text = endingText;
        }
    }

    string GetEndingText(string className)
    {
        switch (className)
        {
            case "BLADE ALPHA": // Blade Alpha
                return "Blade Alpha bước ra ánh sáng, trả thù thành công cho đồng đội.";
            case "TECH GAMA": // Tech Gamma
                return "Tech Gamma sửa chữa hệ thống, ngăn virus lan rộng và thoát ra.";
            default:
                return "Một chiến thắng không rõ danh tính...";
        }
    }
}