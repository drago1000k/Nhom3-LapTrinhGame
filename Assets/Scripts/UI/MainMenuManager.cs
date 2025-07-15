using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Button in main menu")]
    public Button newGameButton;
    public Button classesButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Button in options")]
    public Button audioButton;
    public Button graphicButton;
    public Button controlsButton;

    [Header("Button in panel")]
    public Button backButtonOptions;
    public Button backButtonAudio;
    public Button backButtonGraphic;
    public Button backButtonControls;
    public Button backButtonClasses;

    [Header("Button in class selection")]
    public Button classAButton;
    public Button classBButton;
    public Button confirmClassButton;

    [Header("Panel")]
    public GameObject mainMenuPanel;
    public GameObject classesPanel;
    public GameObject optionsPanel;
    public GameObject audioPanel;
    public GameObject graphicPanel;
    public GameObject controlsPanel;
    public GameObject classSelectionPanel;

    [Header("Image")]
    public GameObject hudImage;

    [Header("UI Text")]
    public Text selectedClassText; // Thêm Text để hiển thị class

    [Header("UI Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private string selectedClass; // Lưu class được chọn
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in scene!");
        }
        StartCoroutine(HideGraphicPanel());
    }

    void Start()
    {
        newGameButton.onClick.AddListener(ShowClassSelection);
        classesButton.onClick.AddListener(Classes);
        optionsButton.onClick.AddListener(Options);
        quitButton.onClick.AddListener(Quit);
        audioButton.onClick.AddListener(ShowAudio);
        graphicButton.onClick.AddListener(ShowGraphic);
        controlsButton.onClick.AddListener(ShowControls);
        backButtonOptions.onClick.AddListener(BackToMain);
        backButtonClasses.onClick.AddListener(BackToMain);
        backButtonAudio.onClick.AddListener(BackToOptions);
        backButtonGraphic.onClick.AddListener(BackToOptions);
        backButtonControls.onClick.AddListener(BackToOptions);
        classAButton.onClick.AddListener(() => SelectClass("BLADE ALPHA"));
        classBButton.onClick.AddListener(() => SelectClass("TECH GAMA"));
        confirmClassButton.onClick.AddListener(ConfirmClass);

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

    void ShowClassSelection()
    {
        mainMenuPanel.SetActive(false);
        classSelectionPanel.SetActive(true);
        hudImage.SetActive(false);
        selectedClassText.text = "Select a class..."; // Hiển thị mặc định
    }

    void SelectClass(string className)
    {
        selectedClass = className;
        selectedClassText.text = "Selected: " + selectedClass; // Cập nhật Text
        Debug.Log("Selected class: " + selectedClass);
    }

    void ConfirmClass()
    {
        if (string.IsNullOrEmpty(selectedClass))
        {
            selectedClassText.text = "Please select a class first!"; // Cảnh báo
            Debug.LogWarning("Please select a class first!");
            return;
        }
        PlayerPrefs.SetString("SelectedClass", selectedClass); // Lưu class
        SceneManager.LoadScene(1); // Chuyển sang scene chơi
    }

    void Classes()
    {
        mainMenuPanel.SetActive(false);
        classesPanel.SetActive(true);
        hudImage.SetActive(false);
    }

    void Options()
    {
        mainMenuPanel.SetActive(false);
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

    void BackToMain()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        graphicPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        hudImage.SetActive(true);
        classesPanel.SetActive(false);
        classSelectionPanel.SetActive(false);
        selectedClassText.text = "";
    }

    void BackToOptions()
    {
        optionsPanel.SetActive(true);
        audioPanel.SetActive(false);
        graphicPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    void Quit()
    {
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