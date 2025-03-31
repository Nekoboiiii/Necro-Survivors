using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // Speed of movement
    public float magnitude; // Range of movement
    public Vector3 direction; // Direction of movement
    Vector3 initialPosition;
    Pickup pickup;

    private void Start()
    {
        pickup = GetComponent<Pickup>();

        // Save the starrting position of the game object
        initialPosition = transform.position;
    }

    void Update()
    {
        if (pickup && !pickup.hasBeenCollected)
        {
            // Sin function for smooth bobbing effect
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        }
    }
}
