using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Player progess
    public int coins;
    public int carrots;
    public int cabbages;

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

    public void OnCollectibleGathered(Collectible collectible)
    {
        switch (collectible.type)
        {
            case Collectible.Type.Carrot: carrots += collectible.value; break;
            case Collectible.Type.Cabbage: cabbages += collectible.value; break;
            case Collectible.Type.Coin: coins += collectible.value; break;
        }
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Carrots", carrots);
        PlayerPrefs.SetInt("Cabbages", cabbages);
    }
    
    private void LoadData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        carrots = PlayerPrefs.GetInt("Carrots", 0);
        cabbages = PlayerPrefs.GetInt("Cabbages", 0);
    }
}
