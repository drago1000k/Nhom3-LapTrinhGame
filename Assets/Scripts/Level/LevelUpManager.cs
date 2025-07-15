using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpPanel; // Panel UI khi lên cấp
    public Button[] optionButtons; // Mảng 3 Button cho 3 tùy chọn
    public Text[] optionTexts; // Text trên mỗi Button
    public Image[] optionIcons; // Image để hiển thị icon
    public Text[] optionDescriptions; // Thêm mảng Text cho description
    public List<UpgradeData> availableUpgrades; // Danh sách nâng cấp có thể chọn

    private Player player; // Tham chiếu đến Player
    public int currentLevel = 1;

    void Start()
    {
        player = FindObjectOfType<Player>(); // Tìm Player trong scene
        levelUpPanel.SetActive(false); // Ẩn panel ban đầu
    }

    // Hàm gọi khi thu thập đủ XP để lên cấp
    public void OnLevelUp()
    {
        currentLevel++;
        ShowLevelUpOptions();
    }

    void ShowLevelUpOptions()
    {
        levelUpPanel.SetActive(true); // Hiển thị panel
        Time.timeScale = 0f; // Tạm dừng game giống Vampire Survivors

        // Lấy 3 nâng cấp ngẫu nhiên
        List<UpgradeData> eligibleUpgrades = new List<UpgradeData>();
        foreach (var upgrade in availableUpgrades)
        {
            if (upgrade.levelRequirement <= currentLevel)
                eligibleUpgrades.Add(upgrade);
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (eligibleUpgrades.Count > 0)
            {
                int randomIndex = Random.Range(0, eligibleUpgrades.Count);
                UpgradeData selectedUpgrade = eligibleUpgrades[randomIndex];
                optionTexts[i].text = selectedUpgrade.upgradeName;
                optionIcons[i].sprite = selectedUpgrade.icon; // Gán icon từ UpgradeData
                optionIcons[i].enabled = true; // Bật Image để hiển thị icon
                optionDescriptions[i].text = selectedUpgrade.description; // Hiển thị description
                optionDescriptions[i].enabled = true; // Bật Text để hiển thị description
                int index = i; // Capture index for closure
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectUpgrade(selectedUpgrade, index));
                eligibleUpgrades.RemoveAt(randomIndex); // Loại bỏ để không lặp lại
            }
            else
            {
                optionTexts[i].text = "No Upgrade";
                optionIcons[i].enabled = false; // Tắt Image nếu không có nâng cấp
                optionDescriptions[i].text = ""; // Xóa description nếu không có nâng cấp
                optionDescriptions[i].enabled = false; // Tắt Text nếu không có nâng cấp
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].interactable = false;
            }
        }
    }

    void SelectUpgrade(UpgradeData upgrade, int buttonIndex)
    {
        // Áp dụng nâng cấp (ví dụ: tăng chỉ số hoặc thêm vũ khí)
        if (upgrade.isWeapon)
            Debug.Log("Added weapon: " + upgrade.upgradeName);
        else
            Debug.Log("Upgraded stat: " + upgrade.upgradeName);

        // Ẩn panel và tiếp tục game
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;

        // Cập nhật Player (cần thêm logic tùy chỉnh)
        if (player != null)
            player.ApplyUpgrade(upgrade);
    }
}