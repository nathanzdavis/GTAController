using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float maxHeight = 10f;  // Set the maximum height in the Unity Inspector
    public float speed = 2f;       // Set the elevator speed in the Unity Inspector

    private bool goingUp = true;   // Flag to determine the elevator direction
    private float initialPosition;  // Store the initial position of the elevator

    void Start()
    {
        initialPosition = transform.position.y;
    }

    void FixedUpdate()
    {
        MoveElevator();
    }

    void MoveElevator()
    {
        float targetHeight = goingUp ? initialPosition + maxHeight : initialPosition;

        // Move the elevator towards the target height
        float step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z), step);

        // Check if the elevator has reached the target height
        if (Mathf.Approximately(transform.position.y, targetHeight))
        {
            // Change direction when reaching the top or bottom
            goingUp = !goingUp;
        }
    }
}