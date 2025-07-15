using UnityEngine;

public class GameController : MonoBehaviour
{
    public UIManager uiManager;
    public Player player; // Tham chiếu trực tiếp đến Player
    public WaveManager waveManager; // Tham chiếu đến WaveManager
    public int wave = 1;
    public int enemiesKilled = 0;
    public float playTime = 0f;
    private bool isPlaying = true;
    private bool bossDefeated = false; // Theo dõi boss đã bị đánh bại chưa
    private bool isEndlessMode = false; // Theo dõi chế độ Endless

    private const float WAVE_DURATION = 30f; // Mỗi wave kéo dài 30 giây

    void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (player == null) player = FindObjectOfType<Player>();
        if (waveManager == null) waveManager = FindObjectOfType<WaveManager>();
    }

    void Update()
    {
        if (isPlaying)
        {
            playTime += Time.deltaTime;

            // Tính wave dựa trên playTime
            int calculatedWave = Mathf.FloorToInt(playTime / WAVE_DURATION) + 1;
            if (calculatedWave > wave && !isEndlessMode) // Tăng wave tự động chỉ khi không ở chế độ Endless
            {
                wave = calculatedWave;
                if (waveManager != null)
                {
                    waveManager.SetWave(wave); // Đồng bộ wave với WaveManager
                    Debug.Log($"Wave updated to {wave} at {playTime:F2}s");
                }
                CheckWave();
            }
        }
    }


    void CheckWave()
    {
        if (waveManager != null)
        {
            if (wave == 20 && !waveManager.IsBossSpawned())
            {
                waveManager.SpawnBoss();
            }
        }
    }

    public void OnBossDefeated()
    {
        bossDefeated = true;
        if (wave == 20 && !isEndlessMode)
        {
            ShowVictory();
        }
        Debug.Log("Boss defeated!");
    }

    void ShowGameOver()
    {
        string stats = $"Time: {playTime:F2}s\nEnemies Killed: {enemiesKilled}\nWave: {wave}";
        if (uiManager != null) uiManager.ShowGameOver(stats);
        isPlaying = false;
    }

    void ShowVictory()
    {
        string stats = $"Time: {playTime:F2}s\nEnemies Killed: {enemiesKilled}\nWave: {wave}";
        if (uiManager != null) uiManager.ShowVictory(stats);
        isPlaying = false;
    }

    public int GetWave()
    {
        return wave;
    }

    public void StartEndlessMode(int startWave)
    {
        if (!isEndlessMode)
        {
            isEndlessMode = true;
            isPlaying = true; // Đảm bảo gameplay tiếp tục
            wave = startWave; // Nhảy thẳng đến wave 21
            if (waveManager != null)
            {
                waveManager.SetWave(wave); // Cập nhật wave trong WaveManager
            }
            Debug.Log($"Endless Mode Started! Wave: {wave}");

            // Thay đổi nhạc nền sang endlessMusic
            AudioManager.Instance?.PlayEndlessMusic();

            // Tăng wave liên tục trong chế độ Endless
            InvokeRepeating(nameof(IncreaseWave), WAVE_DURATION, WAVE_DURATION);
        }
    }

    private void IncreaseWave()
    {
        if (isEndlessMode)
        {
            wave++;
            if (waveManager != null)
            {
                waveManager.SetWave(wave);
                Debug.Log($"Endless Wave increased to {wave} at {playTime:F2}s");
            }
            // Có thể thêm logic spawn enemy hoặc tăng độ khó ở đây
        }
    }
}