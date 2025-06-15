using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class StorySceneManager : MonoBehaviour
{
    [Header("Story Pages")]
    [SerializeField] private GameObject[] storyPages; // Assign your StoryPage1, StoryPage2, etc. here
    [SerializeField] private TextMeshProUGUI storyNarrativeText; // Assign the TextMeshProUGUI component

    [Header("Story Content")]
    // Array of story text for each page
    [TextArea(3, 10)] // Makes the text field bigger in the Inspector
    [SerializeField] private string[] pageTexts; 

    private int currentPageIndex = 0;

    private const string HasWatchedStoryKey = "HasWatchedStory"; // PlayerPrefs key

    void Start()
    {
        // Deactivate all story pages initially
        foreach (GameObject page in storyPages)
        {
            if (page != null)
            {
                page.SetActive(false);
            }
        }

        // Display the first page
        if (storyPages.Length > 0 && storyPages[0] != null)
        {
            storyPages[0].SetActive(true);
            DisplayCurrentPageText();
        }
    }

    // Called by a "Next" button (if you add one, or simply by clicking the screen)
    public void NextPage()
    {
        if (currentPageIndex < storyPages.Length - 1)
        {
            // Deactivate current page
            if (storyPages[currentPageIndex] != null)
            {
                storyPages[currentPageIndex].SetActive(false);
            }

            currentPageIndex++;

            // Activate next page
            if (storyPages[currentPageIndex] != null)
            {
                storyPages[currentPageIndex].SetActive(true);
                DisplayCurrentPageText();
            }
        }
        else
        {
            // Last page, transition to HomeScene
            EndStory();
        }
    }

    private void DisplayCurrentPageText()
    {
        if (storyNarrativeText != null && currentPageIndex < pageTexts.Length)
        {
            storyNarrativeText.text = pageTexts[currentPageIndex];
        }
    }

    // Called by the Skip Button or when story ends
    public void EndStory()
    {
        Debug.Log("Story Skipped or Ended. Going to HomeScene.");
        PlayerPrefs.SetInt(HasWatchedStoryKey, 1); // Mark story as watched
        PlayerPrefs.Save(); // Save PlayerPrefs immediately
        SceneManager.LoadScene("HomeScene");
    }

    // --- Static method to check if story needs to be shown ---
    public static bool ShouldShowStory()
    {
        // PlayerPrefs.DeleteKey(HasWatchedStoryKey); // Uncomment to test story scene always
        return PlayerPrefs.GetInt(HasWatchedStoryKey, 0) == 0; // Returns true if 0 (not watched)
    }
}