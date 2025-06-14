using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class OrbitalMovement : MonoBehaviour
{
    [Header("Orbital Settings")]
    public Transform orbitCenter;
    public float orbitRadius = 5f;
    public float orbitSpeed = 2f;
    public float laneWidth = 2f;
    public float laneChangeSpeed = 10f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private float currentAngle;
    private float targetLane;  // -1=left, 0=center, 1=right

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        // Read input value ( -1 for left, 1 for right)
        float moveInput = moveAction.ReadValue<Vector2>().x;

        // Snap to lane positions
        if (moveInput < -0.5f) targetLane = -1;
        else if (moveInput > 0.5f) targetLane = 1;
    }

    private void FixedUpdate()
    {
        // Orbit progression
        currentAngle += orbitSpeed * Time.fixedDeltaTime;
        
        // Calculate base orbit position
        Vector3 orbitPos = orbitCenter.position + 
            Quaternion.Euler(0, currentAngle * Mathf.Rad2Deg, 0) * 
            Vector3.forward * orbitRadius;
        
        // Apply lane offset (smoothed)
        float laneOffset = Mathf.Lerp(
            transform.localPosition.x, 
            targetLane * laneWidth, 
            laneChangeSpeed * Time.fixedDeltaTime
        );
        
        rb.MovePosition(new Vector3(
            orbitPos.x + laneOffset,
            orbitPos.y,
            orbitPos.z
        ));
    }
}