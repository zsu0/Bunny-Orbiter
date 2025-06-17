using UnityEngine;

public class Carrot : MonoBehaviour, IPoolable // Add ", IPoolable" here
{
    public int value = 1;

    // === REQUIRED POOLING METHODS ===
    public void OnSpawn()
    {
        GetComponent<Collider>().enabled = true; // Reset collider
        GetComponent<Renderer>().enabled = true; // Make visible
    }

    public void OnReturn()
    {
        // Optional cleanup (leave empty if not needed)
    }
    // === END POOLING METHODS ===

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Give player points
            GameManager.Instance.CollectItem(Collectible.Type.Carrot, value);
            
            // 2. Return to pool instead of destroying
            ObjectPoolManager.Instance.ReturnToPool("Carrot", gameObject);
        }
    }
}