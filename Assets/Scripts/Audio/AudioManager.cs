using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource mainMenuMusic;
    public AudioSource gameMusic;
    public AudioSource endlessMusic;
    public AudioSource victoryMusic;

    [Header("Sound Effects")]
    public AudioClip bladeSlashEffect; // Chém thường Blade Alpha
    public AudioClip bladeSkillEffect; // Kỹ năng Blade Alpha
    public AudioClip techShootEffect;  // Bắn thường Tech Gamma
    public AudioClip techSkillEffect;  // Kỹ năng Tech Gamma

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f; // Thêm Master Volume
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved settings
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolume();

        // Khởi tạo audio sources
        mainMenuMusic.volume = musicVolume * masterVolume;
        gameMusic.volume = musicVolume * masterVolume;
        endlessMusic.volume = musicVolume * masterVolume;
        victoryMusic.volume = musicVolume * masterVolume;
    }

    void Start()
    {
        UpdateMusicBasedOnScene();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusicBasedOnScene();
    }

    void UpdateMusicBasedOnScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        mainMenuMusic.Stop();
        gameMusic.Stop();
        endlessMusic.Stop();
        victoryMusic.Stop();

        if (currentScene == 0) // Main Menu
        {
            mainMenuMusic.Play();
        }
        else if (currentScene == 1) // Game Scene
        {
            gameMusic.Play();
        }
    }

    public void PlayEndlessMusic()
    {
        gameMusic.Stop();
        endlessMusic.Play();
    }

    public void PlayVictoryMusic()
    {
        gameMusic.Stop();
        endlessMusic.Stop();
        victoryMusic.Play();
    }

    public void PlayBladeSlash()
    {
        if (bladeSlashEffect != null)
            AudioSource.PlayClipAtPoint(bladeSlashEffect, Camera.main.transform.position, sfxVolume * masterVolume);
    }

    public void PlayBladeSkill()
    {
        if (bladeSkillEffect != null)
            AudioSource.PlayClipAtPoint(bladeSkillEffect, Camera.main.transform.position, sfxVolume * masterVolume);
    }

    public void PlayTechShoot()
    {
        if (techShootEffect != null)
            AudioSource.PlayClipAtPoint(techShootEffect, Camera.main.transform.position, sfxVolume * masterVolume);
    }

    public void PlayTechSkill()
    {
        if (techSkillEffect != null)
            AudioSource.PlayClipAtPoint(techSkillEffect, Camera.main.transform.position, sfxVolume * masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolume();
        SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolume();
        SaveSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    void ApplyVolume()
    {
        mainMenuMusic.volume = musicVolume * masterVolume;
        gameMusic.volume = musicVolume * masterVolume;
        endlessMusic.volume = musicVolume * masterVolume;
        victoryMusic.volume = musicVolume * masterVolume;
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public static AudioManager Instance
    {
        get { return instance; }
    }
}