using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HomeSceneManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject storyCollectionPanel;
    [SerializeField] private GameObject achievementsPanel;
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject explorePlanetSelectPanel;
    [SerializeField] private GameObject myRabbitPanel;

    [Header("Overlay Panel")]
    [SerializeField] private GameObject uiPanelOverlay; // Assign your single UIPanelOverlay here

    [Header("Home Scene UI Elements")]
    [SerializeField] private TextMeshProUGUI levelDisplayTMP;

    [Header("Story Collection UI")]
    [SerializeField] private GameObject storyDialogDisplayScrollViewGO;
    [SerializeField] private TextMeshProUGUI storyDisplayDialogText; // The TextMeshProUGUI inside StoryDialogDisplayScrollView
    [SerializeField] private GameObject storyEntryPrefab; // Your StoryEntryPrefab
    [SerializeField] private Transform storyListContentParent; // The "Content" GameObject inside StoryListScrollView
    [SerializeField] private Sprite lockedStoryPlaceholderIcon; // Your "?" or locked icon

    [Header("Story Data")]
    [SerializeField] private StoryData[] allStories; // Assign all your StoryData ScriptableObjects here

    // A list to keep track of unlocked story IDs (PlayerPrefs will store this)
    private List<string> unlockedStoryIDs = new List<string>();
    private const string UnlockedStoriesKey = "UnlockedStories"; // PlayerPrefs key

    // Keep track of the currently active pop-up panel
    private GameObject currentActivePanel = null;

    void Start()
    {
        // Ensure all pop-up panels and the overlay are hidden at start
        settingsPanel?.SetActive(false);
        storyCollectionPanel?.SetActive(false);
        achievementsPanel?.SetActive(false);
        bagPanel?.SetActive(false);
        levelPanel?.SetActive(false);
        explorePlanetSelectPanel?.SetActive(false);
        myRabbitPanel?.SetActive(false);
        uiPanelOverlay?.SetActive(false); // Make sure overlay is off

        UpdateLevelDisplay(1);
        LoadUnlockedStories(); // Load unlocked stories when HomeScene starts
        PopulateStoryCollection(); // Populate the UI based on loaded data

        // Unlock a default story for testing if none are unlocked by default
        // This is just for initial testing, remove or modify later.
        if (allStories.Length > 0 && !unlockedStoryIDs.Contains(allStories[0].storyID) && !allStories[0].isUnlockedByDefault)
        {
            UnlockStory(allStories[0].storyID);
        }
    }

    // --- Public methods for button clicks ---

    public void OnExitButtonClicked()
    {
        Debug.Log("EXIT button clicked! Returning to StarterScene.");
        SceneManager.LoadScene("StarterScene");
    }

    public void OnExploreButtonClicked()
    {
        Debug.Log("EXPLORE button clicked! Opening Planet Selection.");
        OpenPanel(explorePlanetSelectPanel);
    }

    public void OnLevelButtonClicked()
    {
        Debug.Log("LEVEL button clicked!");
        OpenPanel(levelPanel);
        // Potentially update LevelPanel content here
    }

    public void OnStoryCollectionButtonClicked()
    {
        Debug.Log("STORY COLLECTION button clicked!");
        OpenPanel(storyCollectionPanel);

        // Ensure the dialogue display is hidden and cleared when the main panel opens
        if (storyDisplayDialogText != null)
        {
            storyDisplayDialogText.text = "Select a story to view its full dialogue.";
        }
        if (storyDialogDisplayScrollViewGO != null) // <--- NEW: Ensure it's hidden
        {
            storyDialogDisplayScrollViewGO.SetActive(false);
        }
    }

    public void OnAchievementsButtonClicked()
    {
        Debug.Log("ACHIEVEMENTS button clicked!");
        OpenPanel(achievementsPanel);
    }

    public void OnBagButtonClicked()
    {
        Debug.Log("BAG button clicked!");
        OpenPanel(bagPanel);
    }

    public void OnMarketButtonClicked()
    {
        Debug.Log("MARKET button clicked!");
        SceneManager.LoadScene("MarketScene");
    }

    public void OnMyRabbitButtonClicked()
    {
        Debug.Log("MY RABBIT button clicked!");
        OpenPanel(myRabbitPanel);
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
        OpenPanel(settingsPanel);
    }

    // --- Story Management Logic ---

    private void LoadUnlockedStories()
    {
        unlockedStoryIDs.Clear();
        string savedUnlockedStories = PlayerPrefs.GetString(UnlockedStoriesKey, "");
        if (!string.IsNullOrEmpty(savedUnlockedStories))
        {
            // Split the string by comma to get individual IDs
            string[] ids = savedUnlockedStories.Split(',');
            foreach (string id in ids)
            {
                unlockedStoryIDs.Add(id);
            }
        }
        Debug.Log($"[HomeSceneManager] Loaded unlocked stories: {string.Join(", ", unlockedStoryIDs)}");
    }

    private void SaveUnlockedStories()
    {
        PlayerPrefs.SetString(UnlockedStoriesKey, string.Join(",", unlockedStoryIDs));
        PlayerPrefs.Save();
        Debug.Log($"[HomeSceneManager] Saved unlocked stories: {string.Join(", ", unlockedStoryIDs)}");
    }

    // Call this when a story is unlocked (e.g., via level up, achievement completion)
    public void UnlockStory(string storyID)
    {
        if (!unlockedStoryIDs.Contains(storyID))
        {
            unlockedStoryIDs.Add(storyID);
            SaveUnlockedStories(); // Save immediately after unlocking
            Debug.Log($"[HomeSceneManager] Story '{storyID}' UNLOCKED!");
            PopulateStoryCollection(); // Refresh UI after unlocking
        }
        else
        {
            Debug.Log($"[HomeSceneManager] Story '{storyID}' was already unlocked.");
        }
    }

    // Populates the scroll view with story entries based on unlock status
    private void PopulateStoryCollection()
    {
        // Clear existing entries to avoid duplicates on refresh
        foreach (Transform child in storyListContentParent)
        {
            Destroy(child.gameObject);
        }

        if (storyEntryPrefab == null)
        {
            Debug.LogError("[PopulateStoryCollection] Story Entry Prefab is not assigned! Cannot populate collection.");
            return;
        }

        if (allStories == null || allStories.Length == 0)
        {
            Debug.LogWarning("[PopulateStoryCollection] No StoryData assets assigned to 'allStories' in HomeSceneManager.");
            return;
        }

        foreach (StoryData story in allStories)
        {
            GameObject entryGO = Instantiate(storyEntryPrefab, storyListContentParent);

            // Try to get components, providing specific error if not found
            Image storyIcon = entryGO.transform.Find("StoryIcon")?.GetComponent<Image>();
            TextMeshProUGUI storyTitleText = entryGO.transform.Find("StoryTitleButton/Text (TMP)")?.GetComponent<TextMeshProUGUI>();
            Button storyButton = entryGO.transform.Find("StoryTitleButton")?.GetComponent<Button>();
            CanvasGroup canvasGroup = entryGO.GetComponent<CanvasGroup>();

            if (storyIcon == null) { Debug.LogError($"[PopulateStoryCollection] StoryEntryPrefab missing 'StoryIcon' Image in '{entryGO.name}'."); continue; }
            if (storyTitleText == null) { Debug.LogError($"[PopulateStoryCollection] StoryEntryPrefab missing 'Text (TMP)' for title in '{entryGO.name}'."); continue; }
            if (storyButton == null) { Debug.LogError($"[PopulateStoryCollection] StoryEntryPrefab missing 'StoryTitleButton' Button in '{entryGO.name}'."); continue; }
            if (canvasGroup == null) { Debug.LogError($"[PopulateStoryCollection] StoryEntryPrefab missing 'CanvasGroup' in '{entryGO.name}'."); continue; }

            bool isUnlocked = story.isUnlockedByDefault || unlockedStoryIDs.Contains(story.storyID);

            // --- CRUCIAL DEBUG LOGS FOR DIAGNOSIS ---
            Debug.Log($"[PopulateDebug] Processing story: {story.storyTitle} (ID: {story.storyID}). Is Unlocked: {isUnlocked}");

            // Set icon, title, and interactivity based on unlock status
            if (isUnlocked)
            {
                storyIcon.sprite = story.storyIcon;
                storyIcon.color = Color.white; // Ensure it's not faded
                storyTitleText.text = story.storyTitle;
                canvasGroup.alpha = 1f; // Fully visible
                storyButton.interactable = true; // Clickable

                // Assign click event to the button for unlocked stories
                storyButton.onClick.AddListener(() => DisplayStoryDialog(story));
                Debug.Log($"[PopulateDebug] Added listener for UNLOCKED story: {story.storyTitle}");
            }
            else
            {
                // Faded if locked
                storyIcon.sprite = lockedStoryPlaceholderIcon; // Use the "?" placeholder
                storyIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Faded grey
                storyTitleText.text = "? ? ?";
                canvasGroup.alpha = 0.5f; // Partially transparent
                storyButton.interactable = false; // Not clickable

                Debug.Log($"[PopulateDebug] Story is LOCKED: {story.storyTitle}. Listener NOT added.");
            }
        }
    }

    // Displays the combined dialogue of a selected story in the dialogue panel
    private void DisplayStoryDialog(StoryData story)
    {
        // --- CRUCIAL DEBUG LOGS FOR DIAGNOSIS ---
        Debug.Log($"[DisplayStoryDebug] Attempting to display story: {story.storyTitle} (ID: {story.storyID})");

        if (storyDisplayDialogText != null && storyDialogDisplayScrollViewGO != null) // <--- Added GO check
        {
            storyDialogDisplayScrollViewGO.SetActive(true); // <--- NEW: Activate the dialogue display GO

            // Use the CombinedDialogueForCollection property from the StoryData asset
            if (story.pages == null || story.pages.Count == 0 || string.IsNullOrEmpty(story.CombinedDialogueForCollection))
            {
                storyDisplayDialogText.text = "This story has no dialogue pages defined or its combined dialogue is empty.";
                Debug.LogWarning($"[DisplayStoryDebug] Story '{story.storyTitle}' has no combined dialogue text or pages defined.");
            }
            else
            {
                storyDisplayDialogText.text = story.CombinedDialogueForCollection;
                Debug.Log($"[DisplayStoryDebug] Dialogue successfully set for '{story.storyTitle}'. Text length: {story.CombinedDialogueForCollection.Length}");
            }
        }
        else
        {
            Debug.LogError("[DisplayStoryDebug] storyDisplayDialogText is NOT assigned in HomeSceneManager! Cannot display dialogue.");
        }
    }

    // Method to clear the displayed story dialogue
    public void ClearStoryDialogueDisplay()
    {
        if (storyDisplayDialogText != null)
        {
            storyDisplayDialogText.text = "Select a story to view its full dialogue."; // Reset to a default message
            Debug.Log("[HomeSceneManager] Story dialogue display cleared.");
        }
        if (storyDialogDisplayScrollViewGO != null) // <--- NEW: Deactivate the GO
        {
            storyDialogDisplayScrollViewGO.SetActive(false);
        }
    }


    // --- Centralized Panel Management Methods ---

    // Called when any main button wants to open a panel
    private void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen == null)
        {
            Debug.LogWarning("Attempted to open a null panel.");
            return;
        }

        // If there's an active panel, close it first
        if (currentActivePanel != null && currentActivePanel != panelToOpen)
        {
            currentActivePanel.SetActive(false);
        }

        // Set the new panel as active
        panelToOpen.SetActive(true);
        currentActivePanel = panelToOpen;

        // Activate the overlay
        if (uiPanelOverlay != null)
        {
            uiPanelOverlay.SetActive(true);
        }
    }

    // Called by the "X" button on pop-ups, or by the overlay click
    public void CloseCurrentPanel()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }

        // Deactivate the overlay
        if (uiPanelOverlay != null)
        {
            uiPanelOverlay.SetActive(false);
        }
    }

    // Helper to update level display
    public void UpdateLevelDisplay(int level)
    {
        if (levelDisplayTMP != null)
        {
            levelDisplayTMP.text = "LEVEL " + level;
        }
    }

    // StoryScene
    public void LaunchStoryScene(string storyIDToLoad)
    {
        Debug.Log($"[HomeSceneManager] Launching StoryScene for Story ID: {storyIDToLoad}");

        // Store the ID of the story to be shown page-by-page in PlayerPrefs
        PlayerPrefs.SetString("SelectedStoryID", storyIDToLoad); // Use the same key as StorySceneManager
        PlayerPrefs.Save();

        // Load the StoryScene
        SceneManager.LoadScene("StoryScene");
    }
}