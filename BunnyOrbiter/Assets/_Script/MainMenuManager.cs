using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Reference to the SettingsPanel (if it's a GameObject that needs to be toggled)
    [SerializeField] private GameObject settingsPanel; 

    // --- Button Click Handlers ---

    public void OnPlayButtonClicked()
    {
        Debug.Log("PLAY button clicked!");

        if (StorySceneManager.ShouldShowStory())
        {
            SceneManager.LoadScene("StoryScene"); 
        }
        else
        {
            SceneManager.LoadScene("HomeScene");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
        // Toggle the visibility of the settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Settings Panel not assigned to MainMenuManager!");
        }
    }

    public void OnInformationButtonClicked()
    {
        Debug.Log("INFORMATION button clicked!");
        Debug.Log("Information details: Bunny Orbiter v1.0.0. Developed by [Your Name/Company Name]. All rights reserved.");
        // Later: Link to HTML website
    }

    // A helper method for closing pop-ups, like settings
    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}