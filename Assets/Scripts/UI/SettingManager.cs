using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    [Header("Resolution Settings")]
    public Dropdown resolutionDropdown; // Tham chiếu đến Resolution Dropdown
    public Dropdown windowModeDropdown; // Tham chiếu đến Window Mode Dropdown
    public Toggle vSyncToggle; // Tham chiếu đến VSync Toggle

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private Resolution[] resolutions; // Lưu danh sách resolution khả dụng
    private static int selectedResolutionIndex = -1; // Lưu index resolution
    private static int selectedWindowMode = -1; // Lưu Window Mode (0 = Windowed, 1 = Fullscreen)
    private static bool vSyncEnabled = false; // Lưu trạng thái VSync
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in scene!");
        }
        // Giữ đối tượng này qua các scene
        DontDestroyOnLoad(gameObject);

        // Khởi tạo lần đầu hoặc khôi phục từ PlayerPrefs
        if (selectedResolutionIndex == -1)
        {
            selectedResolutionIndex = PlayerPrefs.GetInt("SelectedResolution", -1);
            selectedWindowMode = PlayerPrefs.GetInt("SelectedWindowMode", Screen.fullScreen ? 1 : 0);
            vSyncEnabled = PlayerPrefs.GetInt("VSyncEnabled", QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;
        }

        // Gán sự kiện khi scene thay đổi
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Lấy tất cả resolution khả dụng
        resolutions = Screen.resolutions;
        InitializeUIComponents(); // Khởi tạo hoặc tìm lại UI
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(Wait());
    }

    void InitializeUIComponents()
    {
        // Tìm Dropdown và Toggle trong scene hiện tại
        resolutionDropdown = GameObject.Find("DropdownResolution")?.GetComponent<Dropdown>();
        windowModeDropdown = GameObject.Find("DropdownMode")?.GetComponent<Dropdown>();
        vSyncToggle = GameObject.Find("VSyncToggle")?.GetComponent<Toggle>();

        // Tìm Slider cho audio
        masterVolumeSlider = GameObject.Find("MasterVolumeSlider")?.GetComponent<Slider>();
        musicVolumeSlider = GameObject.Find("MusicVolumeSlider")?.GetComponent<Slider>();
        sfxVolumeSlider = GameObject.Find("SFXVolumeSlider")?.GetComponent<Slider>();

        // Kiểm tra và gán sự kiện
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
        if (windowModeDropdown != null)
        {
            windowModeDropdown.onValueChanged.RemoveAllListeners();
            windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
        }
        if (vSyncToggle != null)
        {
            vSyncToggle.onValueChanged.RemoveAllListeners();
            vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        }
        if (masterVolumeSlider != null && audioManager != null)
        {
            masterVolumeSlider.value = audioManager.masterVolume;
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
        }
        if (musicVolumeSlider != null && audioManager != null)
        {
            musicVolumeSlider.value = audioManager.musicVolume;
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        }
        if (sfxVolumeSlider != null && audioManager != null)
        {
            sfxVolumeSlider.value = audioManager.sfxVolume;
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        }

        // Cập nhật lại UI
        PopulateResolutionDropdown();
        SetInitialValues();
    }

    void PopulateResolutionDropdown()
    {
        if (resolutionDropdown == null) return; // Thoát nếu không tìm thấy
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = (selectedResolutionIndex != -1) ? selectedResolutionIndex : currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void SetInitialValues()
    {
        if (windowModeDropdown != null)
        {
            windowModeDropdown.value = (selectedWindowMode != -1) ? selectedWindowMode : (Screen.fullScreen ? 1 : 0);
            windowModeDropdown.RefreshShownValue();
        }
        if (vSyncToggle != null)
        {
            vSyncToggle.isOn = (vSyncEnabled != false) ? vSyncEnabled : (QualitySettings.vSyncCount > 0);
            ApplyVSync(vSyncToggle.isOn);
        }
        if (audioManager != null)
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = audioManager.masterVolume;
            if (musicVolumeSlider != null) musicVolumeSlider.value = audioManager.musicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = audioManager.sfxVolume;
        }
    }

    void OnResolutionChanged(int index)
    {
        if (resolutionDropdown == null) return;
        selectedResolutionIndex = index;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("SelectedResolution", index);
        Debug.Log("Resolution changed to: " + resolution.width + "x" + resolution.height);
    }

    void OnWindowModeChanged(int index)
    {
        if (windowModeDropdown == null) return;
        selectedWindowMode = index;
        bool isFullscreen = index == 1;
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("SelectedWindowMode", index);
        Debug.Log("Window Mode changed to: " + (isFullscreen ? "Fullscreen" : "Windowed"));
    }

    void OnVSyncChanged(bool value)
    {
        if (vSyncToggle == null) return;
        vSyncEnabled = value;
        ApplyVSync(value);
        PlayerPrefs.SetInt("VSyncEnabled", value ? 1 : 0);
        Debug.Log("VSync changed to: " + (value ? "On" : "Off"));
    }

    void ApplyVSync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    void OnDestroy()
    {
        // Lưu giá trị cuối cùng khi đối tượng bị hủy
        PlayerPrefs.Save();
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        InitializeUIComponents();
    }
}