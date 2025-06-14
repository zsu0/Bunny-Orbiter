using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpacePlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float maxSpeed = 8f;
    public float rotationSpeed = 5f;

    [Header("Crash")]
    public float crashForce = 15f;
    public float crashStunTime = 2f;

    [Header("References")]
    public Transform visualRoot;
    public ParticleSystem crashParticles;

    private Rigidbody rb;
    private Vector2 input;
    private bool isStunned;
    private float currentStunTime;

    private PlayerInput playerInput;
    private InputAction moveAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"]; // Make sure you have a "Move" action defined
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (isStunned)
        {
            currentStunTime -= Time.deltaTime;
            if (currentStunTime <= 0) isStunned = false;
            return;
        }

        // Get input value (-1 to 1)
        Vector2 input = moveAction.ReadValue<Vector2>();
        
        // Visual tilt
        if (input.x != 0)
        {
            visualRoot.localRotation = Quaternion.Lerp(
                visualRoot.localRotation,
                Quaternion.Euler(0, 0, -25 * input.x),
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            visualRoot.localRotation = Quaternion.Lerp(
                visualRoot.localRotation,
                Quaternion.identity,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void FixedUpdate()
    {
        if (isStunned) return;

        // Movement in camera space
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 forceDirection = (cameraRight * input.x + cameraForward * input.y);
        
        if (forceDirection.magnitude > 0.1f && rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(forceDirection * moveForce, ForceMode.Force);
        }
    }

    public void HandleCrash(Vector3 collisionPoint)
    {
        if (isStunned) return;

        // Physics reaction
        Vector3 dir = (transform.position - collisionPoint).normalized;
        rb.AddForce(dir * crashForce, ForceMode.Impulse);
        
        // Visual feedback
        crashParticles.Play();
        isStunned = true;
        currentStunTime = crashStunTime;

        // Game over sequence
        Invoke("TriggerGameOver", crashStunTime);
    }

    void TriggerGameOver()
    {
        GameManager.Instance.PlayerCrashed();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}