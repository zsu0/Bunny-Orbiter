using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Volume Controls")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    
    [Header("Theme Control")]
    public Toggle darkModeToggle;
    public Text darkModeLabel;
    
    [Header("Control Settings")]
    public Toggle virtualJoystickToggle;
    public Text controlsLabel;
    
    [Header("Panel References")]
    public Button closeButton;
    public Button outsideClickArea;
    
    private void Start()
    {
        SetupPanel();
        LoadCurrentSettings();
    }
    
    private void SetupPanel()
    {
        // Setup volume sliders
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        // Setup theme toggle
        darkModeToggle.onValueChanged.AddListener(OnDarkModeToggled);
        
        // Setup controls toggle
        virtualJoystickToggle.onValueChanged.AddListener(OnControlsToggled);
        
        // Setup close functionality
        closeButton.onClick.AddListener(ClosePanel);
        outsideClickArea.onClick.AddListener(ClosePanel);
    }
    
    private void LoadCurrentSettings()
    {
        var gameManager = GameManager.Instance;
        
        masterVolumeSlider.value = gameManager.masterVolume;
        sfxVolumeSlider.value = gameManager.sfxVolume;
        musicVolumeSlider.value = gameManager.musicVolume;
        darkModeToggle.isOn = gameManager.isDarkMode;
        
        // Load virtual joystick preference
        virtualJoystickToggle.isOn = PlayerPrefs.GetInt("UseVirtualJoystick", 0) == 1;
        
        UpdateLabels();
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        GameManager.Instance.masterVolume = value;
        AudioListener.volume = value;
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        GameManager.Instance.sfxVolume = value;
        // Apply to SFX audio sources
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        GameManager.Instance.musicVolume = value;
        // Apply to music audio sources
    }
    
    private void OnDarkModeToggled(bool isOn)
    {
        GameManager.Instance.isDarkMode = isOn;
        UIManager.Instance?.ApplyTheme();
        UpdateLabels();
    }
    
    private void OnControlsToggled(bool useVirtualJoystick)
    {
        PlayerPrefs.SetInt("UseVirtualJoystick", useVirtualJoystick ? 1 : 0);
        UpdateLabels();
    }
    
    private void UpdateLabels()
    {
        darkModeLabel.text = darkModeToggle.isOn ? "Dark Mode" : "Light Mode";
        controlsLabel.text = virtualJoystickToggle.isOn ? "Virtual Joystick" : "Swipe Controls";
    }
    
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SaveSettings();
    }
    
    // Optional: Close panel when clicking outside
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            RectTransform panelRect = transform.GetChild(1).GetComponent<RectTransform>(); // Settings panel
            
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePos))
            {
                ClosePanel();
            }
        }
    }
}