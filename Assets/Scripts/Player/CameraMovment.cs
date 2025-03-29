using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    
    void Update()
    {
        transform.position = target.position + offset;
    }
}
