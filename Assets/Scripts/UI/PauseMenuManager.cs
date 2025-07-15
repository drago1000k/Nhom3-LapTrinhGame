using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI For Background")]
    public GameObject backGroundUI;

    [Header("Buttons in Pause Menu")]
    public Button resumeButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Buttons in Options")]
    public Button audioButton;
    public Button graphicButton;
    public Button controlsButton;

    [Header("Back Buttons in Panels")]
    public Button backButtonOptions;
    public Button backButtonAudio;
    public Button backButtonGraphic;
    public Button backButtonControls;

    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject optionsPanel;
    public GameObject audioPanel;
    public GameObject graphicPanel;
    public GameObject controlsPanel;

    [Header("UI Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private bool isPaused = false;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in scene!");
        }
        backGroundUI.SetActive(false);
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        StartCoroutine(HideGraphicPanel());
    }

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        optionsButton.onClick.AddListener(ShowOptions);
        quitButton.onClick.AddListener(QuitGame);
        audioButton.onClick.AddListener(ShowAudio);
        graphicButton.onClick.AddListener(ShowGraphic);
        controlsButton.onClick.AddListener(ShowControls);
        backButtonOptions.onClick.AddListener(BackToPauseMenu);
        backButtonAudio.onClick.AddListener(BackToOptions);
        backButtonGraphic.onClick.AddListener(BackToOptions);
        backButtonControls.onClick.AddListener(BackToOptions);

        // Khởi tạo và liên kết slider
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = audioManager != null ? audioManager.masterVolume : 1f;
            masterVolumeSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = audioManager != null ? audioManager.musicVolume : 1f;
            musicVolumeSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = audioManager != null ? audioManager.sfxVolume : 1f;
            sfxVolumeSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        backGroundUI.SetActive(true);
        pauseMenuPanel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục game
        pauseMenuPanel.SetActive(false);
        backGroundUI.SetActive(false);
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        graphicPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    void ShowOptions()
    {
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void ShowAudio()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(true);
    }

    void ShowGraphic()
    {
        optionsPanel.SetActive(false);
        graphicPanel.SetActive(true);
    }

    void ShowControls()
    {
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    void BackToPauseMenu()
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    void BackToOptions()
    {
        audioPanel.SetActive(false);
        graphicPanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void QuitGame()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian chạy lại bình thường trước khi thoát
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator HideGraphicPanel()
    {
        yield return new WaitForSeconds(0.2f);
        graphicPanel.SetActive(false);
    }
}