using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Gameplay")]
    public int currentScore;
    public int collectedCoins;
    public int collectedFood;
    public float survivalTime;

    // player progress
    public int totalCoins;
    public int totalCarrots;
    public int totalCabbages;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI timeText;
    public TMPro.TextMeshProUGUI foodText;

    private bool isGameOver;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize UI references AFTER canvas is ready
            StartCoroutine(DelayedUIInit());
        }
    }

    IEnumerator DelayedUIInit()
    {
        yield return null; // Wait one frame
        UpdateUI(); // Now safe to access UI elements
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Time.timeScale = 1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            survivalTime += Time.deltaTime;
            UpdateUI();
        }
    }

    public void CollectItem(Collectible.Type type, int value)
    {
        switch (type)
        {
            case Collectible.Type.Carrot:
            case Collectible.Type.Cabbage:
                collectedFood += value;
                break;
            case Collectible.Type.Coin:
                collectedCoins += value;
                currentScore += value;
                break;
        }
    }

    public void PlayerCrashed()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0.5f; // Slow-mo effect
    }

    public void PlayerFellOff()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UpdateUI()
    {
        // Skip if UI elements aren't ready
        if (scoreText == null || timeText == null || foodText == null) 
            return;

        scoreText.text = $"Score: {currentScore}";
        timeText.text = $"Time: {survivalTime.ToString("F1")}s";
        foodText.text = $"Food: {collectedFood}";
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Optional: Add your loading screen logic here if you have one
        // if (loadingScreen != null) loadingScreen.SetActive(true);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            // Optional: Add loading progress updates here
            yield return null;
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void HandleCollection(Collectible collectible)
    {
        switch (collectible.type)
        {
            case Collectible.Type.Carrot:
                totalCarrots += collectible.value;
                break;
            case Collectible.Type.Cabbage:
                totalCabbages += collectible.value;
                break;
            case Collectible.Type.Coin:
                totalCoins += collectible.value;
                break;
        }
        
        Debug.Log($"Collected: {collectible.type} (Total: {GetTotalForType(collectible.type)})");
    }

    private int GetTotalForType(Collectible.Type type)
    {
        return type switch
        {
            Collectible.Type.Carrot => totalCarrots,
            Collectible.Type.Cabbage => totalCabbages,
            Collectible.Type.Coin => totalCoins,
            _ => 0
        };
    }
}