using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int coins = 0;
    public int carrots = 0;
    public int cabbages = 0;
    public float bestTime = 0f;
    public List<string> unlockedStories = new List<string>();
    public List<string> unlockedAchievements = new List<string>();
    public List<string> unlockedCosmetics = new List<string>();
    public string currentHat = "";
    public string currentTrail = "";
    public int currentColorIndex = 0;
    public bool firstTimePlay = true;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public PlayerData playerData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }
    
    public void LoadData()
    {
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string json = PlayerPrefs.GetString("PlayerData");
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
        }
    }
    
    public void AddCoins(int amount)
    {
        playerData.coins += amount;
        SaveData();
    }
    
    public void AddCarrots(int amount)
    {
        playerData.carrots += amount;
        SaveData();
    }
    
    public void AddCabbages(int amount)
    {
        playerData.cabbages += amount;
        SaveData();
    }
    
    public bool CanAfford(int cost)
    {
        return playerData.coins >= cost;
    }
    
    public bool SpendCoins(int amount)
    {
        if (CanAfford(amount))
        {
            playerData.coins -= amount;
            SaveData();
            return true;
        }
        return false;
    }

    // For level progression tracking use in StarterScene
    public void CheckLevelProgression()
    {
        int newLevel = CalculateLevel();
        
        if (newLevel != playerData.level)
        {
            int oldLevel = playerData.level;
            playerData.level = newLevel;
            
            // Check if debris state should change (levels 5, 15, 25)
            if ((oldLevel < 5 && newLevel >= 5) || 
                (oldLevel < 15 && newLevel >= 15) || 
                (oldLevel < 25 && newLevel >= 25))
            {
                // Notify StarterScene to update debris when player returns
                PlayerPrefs.SetInt("DebrisStateChanged", 1);
            }
            
            SaveData();
            Debug.Log($"Level Up! Old: {oldLevel}, New: {newLevel}");
        }
    }

    private int CalculateLevel()
    {
        // Example: Level based on total experience/achievements
        int totalExperience = playerData.coins + (playerData.carrots * 2) + (playerData.cabbages * 3);
        return Mathf.FloorToInt(totalExperience / 100f) + 1; // 100 exp per level
    }
}