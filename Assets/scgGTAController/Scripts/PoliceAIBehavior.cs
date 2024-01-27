using UnityEngine;
using System.Collections;
using scgGTAController;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PoliceAIBehavior : MonoBehaviour
{
    [HideInInspector] public Transform player;
    public float alertDistance = 25f;
    public float shootingDistance = 10f;
    public float initialShootDelay;
    public float reloadTime = 5f;
    public int roundsPerMagazine = 10;
    public Transform shootingPoint;
    public GameObject projectilePrefab;
    public float shootingForce = 500f;
    public AudioClip shootSound;
    public GameObject gunModel;
    public int damage;

    private bool reloading = false;
    private int remainingRounds;
    private bool shootDelayApplied = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<AIController>().anim = GetComponent<Animator>();
        remainingRounds = roundsPerMagazine;
    }

    void Update()
    {
        if (GetComponent<AIController>().alerted && player)
        {
            RotateToPlayer(); // Rotate to face the player when within shooting distance

            if (Vector3.Distance(transform.position, player.position) < shootingDistance)
            {
                if (!shootDelayApplied)
                {
                    StartCoroutine(ShootWithDelay());
                    shootDelayApplied = true;
                }
            }
            else
            {
                // Reset the shoot delay flag when player is out of shooting distance
                shootDelayApplied = false;
            }

            if (reloading)
            {
                // Wait for the reload coroutine to complete
                return;
            }
        }
    }

    void RotateToPlayer()
    {
        if (player)
        {
            // Rotate towards the player
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPosition);
        }
    }

    public void CheckForAlert()
    {
        if (!GetComponent<AIController>().alerted)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < alertDistance)
            {
                // Reset reloading when player is not in range
                reloading = false;
                // Reset animation parameters when not alerted

                // Reset the shoot delay flag when player is out of alert distance
                shootDelayApplied = false;
                // Reset the offsetRotation to the spine
                GetComponentInChildren<OffsetRotation>().enabled = false;
                // Hide the gun model
                gunModel.SetActive(true);

                // Set the offsetRotation to the spine
                GetComponentInChildren<OffsetRotation>().enabled = true;
                // Show the gun model
                gunModel.SetActive(true);

                // Set the cost of the road layer to 1 so agents use it as wellnow
                NavMesh.SetAreaCost(3, 1);

                GetComponent<AIController>().alerted = true;
            }
        }
    }

    IEnumerator ShootWithDelay()
    {
        // Wait for the specified shooting delay
        yield return new WaitForSeconds(initialShootDelay);

        while (GetComponent<AIController>().alerted && player) // Keep shooting as long as the cop is alerted
        {
            // Rotate towards the player
            RotateToPlayer();

            // Check if it's time to reload
            if (remainingRounds <= 0 && !reloading)
            {
                GetComponent<AIController>().anim.SetTrigger("Reload"); // Trigger the "Reload" animation trigger
                reloading = true;
                StartCoroutine(Reload());
                shootDelayApplied = false; // Reset the shoot delay flag after reloading
                yield break;
            }

            // Check if the player is still within shooting distance
            if (Vector3.Distance(transform.position, player.position) <= shootingDistance)
            {
                // Trigger the "Shoot" animation trigger
                GetComponent<AIController>().anim.SetTrigger("Shoot");

                // Instantiate the projectile prefab at the shooting point
                GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
                projectilePrefab.GetComponent<RegisterHit>().damage = damage;

                var offsetPosition = new Vector3(player.position.x, player.position.y + 1f, player.position.z);

                // Calculate the direction to shoot
                Vector3 shootDirection = (offsetPosition - shootingPoint.position).normalized;

                // Set the projectile's direction and apply force
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
                projectileRb.AddForce(shootDirection * shootingForce, ForceMode.Impulse);

                // Play shoot sound
                GetComponent<AudioSource>().PlayOneShot(shootSound);

                // Decrement remainingRounds
                remainingRounds--;

                // Wait for the next shot (you can adjust the delay if needed)
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                // Reset the shoot delay flag when player is out of shooting distance
                shootDelayApplied = false;
                yield break;
            }
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        remainingRounds = roundsPerMagazine;
        reloading = false;
    }
}
