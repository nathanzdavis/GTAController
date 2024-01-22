using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public float pauseDuration = 2f;
    public float playerAvoidanceDistance = 5f;
    public float avoidanceRotationSpeed = 2f;

    public int randomWaypointInterval = 3;
    private int waypointsVisitedCount = 0;

    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent navMeshAgent;
    private Transform targetWaypoint;
    private int currentWaypointIndex = 0;
    private bool isPaused = false;
    private bool isMoving = false;
    private bool isPlayerNear = false;
    private Animator anim;

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
            // If the cop is alerted, move toward the player using PoliceAIBehavior
            if (policeAI != null && policeAI.alerted)
            {
                MoveTowardsPlayer();
            }
            else
            {
                Patrol();
            }
        }

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
