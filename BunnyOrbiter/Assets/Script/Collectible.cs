using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Carrot, Cabbage, Coin }
    public Type type;
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Minimal event - let GameManager handlee logic
        GameManager.Instance.OnCollectibleGathered(this);
        Destroy(gameObject);
    }
}
