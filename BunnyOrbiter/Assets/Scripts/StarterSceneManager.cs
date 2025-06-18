using UnityEngine;
using UnityEngine.UI;

public class StarterSceneManager : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button settingsButton;
    public Button informationButton;
    public GameObject settingsPopup;
    public Button settingsCloseButton;
    public GameObject floatingDebrisParent;
    
    [Header("Debris State Images")]
    public GameObject debrisFirstStatePNG;   // Lots of debris (Level 0-4)
    public GameObject debrisMiddleStatePNG;  // Medium debris (Level 5-14)
    public GameObject debrisLastStatePNG;    // Thin debris (Level 15-24)
    // Level 25+ = No debris floating
    
    [Header("Floating Animation")]
    public float debrisSpeed = 1f;
    public float debrisRange = 2f;
    
    private void Start()
    {
        SetupUI();
        UpdateDebrisBasedOnLevel();
        StartFloatingAnimation();
    }
    
    private void SetupUI()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        informationButton.onClick.AddListener(OnInformationClicked);
        settingsCloseButton.onClick.AddListener(CloseSettingsPanel);
        
        // Ensure settings popup is closed initially
        settingsPopup.SetActive(false);
        
        // Apply theme
        UIManager.Instance?.ApplyTheme();
    }
    
    private void OnPlayClicked()
    {
        if (DataManager.Instance.playerData.firstTimePlay)
        {
            GameManager.Instance.LoadScene("StoryScene");
        }
        else
        {
            GameManager.Instance.LoadScene("HomeScene");
        }
    }
    
    private void OnSettingsClicked()
    {
        settingsPopup.SetActive(true);
    }
    
    private void OnInformationClicked()
    {
        Debug.Log("Information details - Version 1.0, Developer: [Your Name]");
        // Future: Open website link
    }
    
    public void CloseSettingsPanel()
    {
        settingsPopup.SetActive(false);
        // Save settings when closing panel
        GameManager.Instance.SaveSettings();
    }
    
    private void UpdateDebrisBasedOnLevel()
    {
        int playerLevel = DataManager.Instance.playerData.level;
        
        // Deactivate all debris states first
        debrisFirstStatePNG.SetActive(false);
        debrisMiddleStatePNG.SetActive(false);
        debrisLastStatePNG.SetActive(false);
        
        // Activate appropriate debris state based on level
        if (playerLevel < 5)
        {
            // Level 0-4: Lots of debris
            debrisFirstStatePNG.SetActive(true);
        }
        else if (playerLevel < 15)
        {
            // Level 5-14: Medium quantity of debris
            debrisMiddleStatePNG.SetActive(true);
        }
        else if (playerLevel < 25)
        {
            // Level 15-24: Thin debris
            debrisLastStatePNG.SetActive(true);
        }
        // Level 25+: No debris (all remain inactive)
        
        Debug.Log($"Player Level: {playerLevel}, Debris State Updated");
    }
    
    private void StartFloatingAnimation()
    {
        // Animate floating PNG debris images for storage efficiency
        if (floatingDebrisParent != null)
        {
            foreach (Transform debris in floatingDebrisParent.transform)
            {
                // Only animate active debris states
                if (debris.gameObject.activeInHierarchy)
                {
                    StartCoroutine(FloatDebris(debris));
                }
            }
        }
    }
    
    private System.Collections.IEnumerator FloatDebris(Transform debris)
    {
        Vector3 startPos = debris.localPosition;
        float randomOffset = Random.Range(0f, Mathf.PI * 2f);
        float randomSpeed = Random.Range(0.5f, 1.5f);
        
        while (debris.gameObject.activeInHierarchy)
        {
            // Simple left-right floating motion for PNG images
            float x = Mathf.Sin(Time.time * debrisSpeed * randomSpeed + randomOffset) * debrisRange;
            float y = Mathf.Cos(Time.time * debrisSpeed * randomSpeed * 0.7f + randomOffset) * (debrisRange * 0.3f);
            
            debris.localPosition = startPos + new Vector3(x, y, 0f);
            yield return null;
        }
    }
    
    // Call this method when player levels up (from other scenes)
    public void RefreshDebrisState()
    {
        UpdateDebrisBasedOnLevel();
        
        // Stop all current animations
        StopAllCoroutines();
        
        // Restart animations for active debris
        StartFloatingAnimation();
    }
}