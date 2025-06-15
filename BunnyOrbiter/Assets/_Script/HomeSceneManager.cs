using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshProUI components if any

public class HomeSceneManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject storyCollectionPanel;
    [SerializeField] private GameObject achievementsPanel;
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private GameObject myRabbitPanel; // For MyRabbit Menu

    void Start()
    {
        // Ensure all pop-up panels are hidden at start
        settingsPanel?.SetActive(false);
        storyCollectionPanel?.SetActive(false);
        achievementsPanel?.SetActive(false);
        bagPanel?.SetActive(false);
        marketPanel?.SetActive(false);
        myRabbitPanel?.SetActive(false);
    }

    // --- Navigation Buttons ---
    public void OnExitButtonClicked()
    {
        Debug.Log("EXIT button clicked! Returning to StarterScene.");
        SceneManager.LoadScene("StarterScene");
    }

    public void OnExploreButtonClicked()
    {
        Debug.Log("EXPLORE button clicked! Starting GamePlayScene.");
        SceneManager.LoadScene("GamePlayScene"); // Will create this later
    }

    // --- Pop-up Buttons ---
    public void OnStoryCollectionButtonClicked()
    {
        Debug.Log("STORY COLLECTION button clicked!");
        TogglePanel(storyCollectionPanel);
    }

    public void OnAchievementsButtonClicked()
    {
        Debug.Log("ACHIEVEMENTS button clicked!");
        TogglePanel(achievementsPanel);
    }

    public void OnBagButtonClicked()
    {
        Debug.Log("BAG button clicked!");
        TogglePanel(bagPanel);
    }

    public void OnMarketButtonClicked()
    {
        Debug.Log("MARKET button clicked!");
        // Market is full-screen, so it's a scene transition, not just a panel toggle
        SceneManager.LoadScene("MarketScene"); // Will create this later
    }

    public void OnMyRabbitButtonClicked()
    {
        Debug.Log("MY RABBIT button clicked!");
        TogglePanel(myRabbitPanel);
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("SETTINGS button clicked!");
        TogglePanel(settingsPanel);
    }


    // --- General Panel Toggle/Close Method ---
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Panel not assigned: " + panel?.name);
        }
    }

    public void ClosePanel(GameObject panel) // For explicit "X" buttons
    {
         if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}