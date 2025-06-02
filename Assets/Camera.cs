using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;         // The object to follow
    public Vector3 offset = new Vector3(0, 5, -10); // Offset from the target

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            // Don't rotate or look at target
        }
    }
}
