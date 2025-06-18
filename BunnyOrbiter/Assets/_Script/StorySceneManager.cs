using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq; // Required for .FirstOrDefault()

public class StorySceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image storyImage;
    [SerializeField] private TextMeshProUGUI storyDialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton; // Renamed from closeButton

    [Header("Story Data Sources")]
    [SerializeField] private StoryData[] allStories; // Assign ALL your StoryData ScriptableObjects here

    private StoryData currentStory;
    private int currentPageIndex = 0;
    private const string SelectedStoryIDKey = "SelectedStoryID"; // Key used by HomeSceneManager to pass the story ID
    private const string HasWatchedStoryKey = "HasWatchedStory"; // PlayerPrefs key (from your old script, kept as utility)

    [SerializeField] private string chapter1StoryID = "chapter_1"; // Ensure this ID matches an actual StoryData asset ID

    void Start()
    {
        Debug.Log("[StarterSceneManager] Start method called.");

        // Add debug logs here to confirm execution
        Debug.Log($"[StarterSceneManager] Attempting to set PlayerPref for story ID: {chapter1StoryID}");

        PlayerPrefs.SetString("SelectedStoryID", chapter1StoryID);
        PlayerPrefs.Save(); // This is crucial!

        Debug.Log($"[StarterSceneManager] PlayerPref 'SelectedStoryID' set to: {PlayerPrefs.GetString("SelectedStoryID")}.");
        Debug.Log("[StarterSceneManager] Attempting to load StoryScene.");

        SceneManager.LoadScene("StoryScene");
    }
    
    void Awake()
    {
        // Assign button listeners for Next and Skip
        nextButton?.onClick.AddListener(OnNextPageButtonClicked);
        skipButton?.onClick.AddListener(OnSkipButtonClicked); // Assigned to the new skipButton

        LoadStory(); // Attempt to load the selected story
    }

    private void LoadStory()
    {
        // Retrieve the story ID that was saved by the HomeSceneManager
        string selectedStoryID = PlayerPrefs.GetString(SelectedStoryIDKey, "");

        if (string.IsNullOrEmpty(selectedStoryID))
        {
            Debug.LogError("[StorySceneManager] No story ID found in PlayerPrefs. This shouldn't happen. Returning to HomeScene.");
            SceneManager.LoadScene("HomeScene");
            return;
        }

        // Find the StoryData asset that matches the ID from the list assigned in the Inspector
        currentStory = allStories.FirstOrDefault(s => s.storyID == selectedStoryID);

        if (currentStory == null)
        {
            Debug.LogError($"[StorySceneManager] StoryData with ID '{selectedStoryID}' not found in 'allStories' array. Please ensure all your StoryData assets are assigned in the Inspector. Returning to HomeScene.");
            SceneManager.LoadScene("HomeScene");
            return;
        }

        Debug.Log($"[StorySceneManager] Loaded story '{currentStory.storyTitle}' (ID: {currentStory.storyID}). Total pages: {currentStory.pages?.Count}");

        currentPageIndex = 0; // Always start from the first page when a story is loaded
        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        // Basic checks to prevent errors if story data is missing
        if (currentStory == null || currentStory.pages == null || currentStory.pages.Count == 0)
        {
            storyDialogueText.text = "Error: Story has no pages defined. Check StoryData asset.";
            storyImage.sprite = null;
            storyImage.color = Color.clear; // Hide image if no content
            nextButton.interactable = false; // Disable next button if no pages
            Debug.LogError("[StorySceneManager] Attempted to display a page for a story with no pages.");
            return;
        }

        // Ensure currentPageIndex is within valid bounds for the current story
        if (currentPageIndex < 0) currentPageIndex = 0; // Should not happen with Next-only, but good for safety
        if (currentPageIndex >= currentStory.pages.Count)
        {
            // This means we've gone past the last page
            OnStoryEnd(); // Call end-of-story logic
            return;
        }

        // Get the current page's data
        StoryPage currentPage = currentStory.pages[currentPageIndex];

        // Update UI elements with content from the current page
        storyDialogueText.text = currentPage.dialogueText;
        storyImage.sprite = currentPage.pagePicture;

        // Make sure the image is visible if a sprite is assigned, otherwise hide it
        if (currentPage.pagePicture != null)
        {
            storyImage.color = Color.white; // Make it fully visible
        }
        else
        {
            storyImage.color = Color.clear; // Make it completely transparent
        }

        // Update next button interactivity: disable if on the last page
        nextButton.interactable = (currentPageIndex < currentStory.pages.Count - 1);

        Debug.Log($"[StorySceneManager] Displaying page {currentPageIndex + 1}/{currentStory.pages.Count}");
    }

    public void OnNextPageButtonClicked()
    {
        if (currentStory != null && currentPageIndex < currentStory.pages.Count - 1)
        {
            currentPageIndex++; // Move to the next page
            DisplayCurrentPage(); // Update UI
        }
        else
        {
            Debug.Log("[StorySceneManager] Reached end of story pages (Next button clicked on last page).");
            OnStoryEnd(); // Call end-of-story logic
        }
    }

    private void OnStoryEnd()
    {
        Debug.Log($"[StorySceneManager] Story '{currentStory.storyTitle}' has ended.");
        // Optionally, mark this specific story as watched if needed later:
        // PlayerPrefs.SetInt(HasWatchedStoryKey + currentStory.storyID, 1);
        // PlayerPrefs.Save();
        OnSkipButtonClicked(); // Automatically return to HomeScene
    }

    public void OnSkipButtonClicked()
    {
        Debug.Log("[StorySceneManager] Skip button clicked / Story ended. Returning to HomeScene.");
        SceneManager.LoadScene("HomeScene"); // Instantly return to HomeScene
    }

    // --- Utility method from your old script (optional, for checking if story needs to be shown for first-time play) ---
    public static bool ShouldShowStory()
    {
        // PlayerPrefs.DeleteKey(HasWatchedStoryKey); // Uncomment to test story scene always for initial playthrough
        // Note: This 'HasWatchedStoryKey' is generic. If you need to track specific story completions,
        // you should use a key like HasWatchedStoryKey + storyID.
        return PlayerPrefs.GetInt(HasWatchedStoryKey, 0) == 0; // Returns true if 0 (not watched globally)
    }
}