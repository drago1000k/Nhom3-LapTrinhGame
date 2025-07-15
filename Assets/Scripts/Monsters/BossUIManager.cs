using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    public GameObject healthBarUI;
    public Slider healthBar;
    public Text healthText;
    private Boss currentBoss;

    private void Start()
    {
        healthBarUI.SetActive(false);    
    }

    public void SetBoss(Boss boss)
    {
        currentBoss = boss;
        if (healthBar != null)
        {
            healthBar.maxValue = boss.baseHealth;
            healthBar.value = boss.baseHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(boss.baseHealth)} / {boss.baseHealth}";
        }
        gameObject.SetActive(true);
    }

    public void UpdateHealth(float currentHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {healthBar.maxValue}";
        }
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(false);
        currentBoss = null;
    }

    public void ShowHealthBarUI()
    {
        healthBarUI.SetActive(true);
    }
}