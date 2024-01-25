using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SteeringAI : MonoBehaviour
{
    [HideInInspector] public Transform linkedCarTransform; // Add a reference to the linked car's transform
    public float catchUpSpeed = 2f;
    public float catchUpDistance = 3f;
    public float originalSpeed;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Find the player GameObject with the "Player" tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // Check if the player GameObject and linked car GameObject are found
        if (playerObject != null)
        {
            // Get the Transform component of the player GameObject
            playerTransform = playerObject.transform;
            // Set the player as the destination
            navMeshAgent.SetDestination(playerTransform.position);
        }
        else
        {
            Debug.LogError("Player or LinkedCar GameObject not found!");
        }

        StartCoroutine(UpdatePlayerLocation());
    }

    IEnumerator UpdatePlayerLocation()
    {
        //Every 5 seconds update the player destination if we havent reached them yet
        yield return new WaitForSeconds(5f);

        navMeshAgent.SetDestination(playerTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform)
        {
            // Check if the AI has reached the player
            if (Vector3.Distance(transform.position, playerTransform.position) < 10f)
            {
                // Stop further movement
                Debug.Log("Reached the player!");
                navMeshAgent.isStopped = true;
            }
            else
            {
                // Check if the linked car is more than 3m away
                float distanceToLinkedCar = Vector3.Distance(transform.position, linkedCarTransform.position);

                if (distanceToLinkedCar > catchUpDistance)
                {
                    // Slow down to catch up speed
                    navMeshAgent.speed = catchUpSpeed;
                }
                else
                {
                    // Set speed back to the original speed
                    navMeshAgent.speed = originalSpeed;
                }
            }
        }
    }
}
