using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool alerted;
    public float pauseDuration = 2f;
    public float playerAvoidanceDistance = 5f;
    public float avoidanceRotationSpeed = 2f;
    public int randomWaypointInterval = 3;
    public AudioClip[] screams;
    public float chanceToPlayScream;

    private int waypointsVisitedCount = 0;
    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent navMeshAgent;
    private Transform targetWaypoint;
    private int currentWaypointIndex = 0;
    private bool isPaused = false;
    private bool isMoving = false;
    private bool isPlayerNear = false;

    // Reference to PoliceAIBehavior
    private PoliceAIBehavior policeAI;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("waypoint"))
        {
            waypoints.Add(go.transform);
        }

        // Set a random waypoint as the starting point for visited count
        currentWaypointIndex = Random.Range(0, waypoints.Count);

        SetNextWaypoint();

        // Get the PoliceAIBehavior component if it exists
        policeAI = GetComponent<PoliceAIBehavior>();
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (policeAI != null && alerted)
            {
                // If the cop is alerted, move toward the player using PoliceAIBehavior
                MoveTowardsPlayer();
            }
            else if (!alerted)
            {
                Patrol();
            }
        }
        if (!GetComponent<PoliceAIBehavior>())
            anim.SetBool("Alerted", alerted && navMeshAgent.velocity.magnitude >= .5f);
        else
            anim.SetBool("Alerted", alerted);

        anim.SetBool("Moving", isMoving && navMeshAgent.velocity.magnitude >= .5f);
    }

    private void Patrol()
    {
        isMoving = navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;

        if (isMoving && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isMoving = false;
        }

        if (!isMoving)
        {
            StartCoroutine(PauseBetweenWaypoints());
        }

        if (isPlayerNear)
        {
            AvoidPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (policeAI.player)
        {
            // Move toward the player if PoliceAIBehavior is alerted
            navMeshAgent.SetDestination(policeAI.player.transform.position);

            isMoving = true;
        }
    }

    private IEnumerator PauseBetweenWaypoints()
    {
        isPaused = true;

        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(pauseDuration);
        navMeshAgent.isStopped = false;

        isPaused = false;

        waypointsVisitedCount++;

        if (waypointsVisitedCount >= randomWaypointInterval)
        {
            waypointsVisitedCount = 0;
            SetRandomWaypoint();
        }
        else
        {
            SetNextWaypoint();
        }
    }

    private void SetNextWaypoint()
    {
        targetWaypoint = waypoints[currentWaypointIndex];
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        navMeshAgent.SetDestination(targetWaypoint.position);
    }

    private void SetRandomWaypoint()
    {
        currentWaypointIndex = Random.Range(0, waypoints.Count);
        targetWaypoint = waypoints[currentWaypointIndex];
        navMeshAgent.SetDestination(targetWaypoint.position);
        StartCoroutine(ResetSpeedAfterRandomWaypoint());
    }

    private void SetRandomDirection()
    {
        // Generate a random direction
        Vector3 randomDirection = Random.insideUnitSphere.normalized * 25f;

        // Set the distance to at least 25 meters
        randomDirection.y = 0f;

        // Calculate the target position
        Vector3 targetPosition = transform.position + randomDirection;

        // Set the NavMeshAgent destination directly
        navMeshAgent.SetDestination(targetPosition);

        navMeshAgent.speed = 3f;

        // Optionally, reset speed after reaching the random waypoint
        StartCoroutine(ResetSpeedAfterRandomWaypoint());
    }

    private IEnumerator ResetSpeedAfterRandomWaypoint()
    {
        if (!GetComponent<PoliceAIBehavior>()){
            yield return new WaitForSeconds(15f);
            // Wait until the AI reaches the random waypoint
            navMeshAgent.speed = 1.75f; // Set the speed back to 1.75 after reaching the random waypoint

            alerted = false;
        }
    }

    public void Alert()
    {
        if (!GetComponent<PoliceAIBehavior>() && !alerted)
        {
            alerted = true;
            navMeshAgent.isStopped = false;
            isPaused = false;

            StopAllCoroutines();

            // Set the cost of the road layer to 1 so agents use it as wellnow
            NavMesh.SetAreaCost(3, 1);

            if (Random.Range(0, 100) < chanceToPlayScream)
            {
                StartCoroutine(PlayScreamSound());
            }

            SetRandomDirection();
        }
    }

    private IEnumerator PlayScreamSound()
    {
        yield return new WaitForSeconds(Random.Range((float)0, (float)3));
        GetComponent<AudioSource>().PlayOneShot(screams[Random.Range(0, screams.Length - 1)]);
    }

    public void AvoidPlayer()
    {
        anim.SetTrigger("PlayerReaction");
        StartCoroutine(AvoidanceDelay());
    }

    private IEnumerator AvoidanceDelay()
    {
        isPaused = true;

        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(3f);

        Vector3 directionToPlayer = (transform.position - targetWaypoint.position).normalized;
        Vector3 avoidanceRotation = Quaternion.Euler(0f, 90f, 0f) * directionToPlayer;

        while (isPlayerNear)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + avoidanceRotation, avoidanceRotationSpeed * Time.deltaTime);
            yield return null;
        }

        navMeshAgent.isStopped = false;

        isPaused = false;
        SetNextWaypoint();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
