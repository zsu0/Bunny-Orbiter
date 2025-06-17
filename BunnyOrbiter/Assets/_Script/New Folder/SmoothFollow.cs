using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float height = 5f;
    public float damping = 5f;

    void LateUpdate()
    {
        Vector3 targetPos = target.position - target.forward * distance + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, targetPos, damping * Time.deltaTime);
        transform.LookAt(target);
    }
}
