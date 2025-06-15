using UnityEngine;

public class Comet : MonoBehaviour, IPoolable // Add ", IPoolable" here
{
    private float rotationSpeed;

    // === REQUIRED POOLING METHODS ===
    public void OnSpawn()
    {
        rotationSpeed = Random.Range(10f, 30f); // Reset rotation
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Stop movement
    }

    public void OnReturn()
    {
        // Optional cleanup
    }
    // === END POOLING METHODS ===

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjectPoolManager.Instance.ReturnToPool("Comet", gameObject);
        }
    }
}