using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Carrot, Cabbage, Coin }
    public Type type;
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Notify GameManager
        GameManager.Instance.HandleCollection(this);
        
        // Visual/audio feedback would go here
        Destroy(gameObject);
    }
}
