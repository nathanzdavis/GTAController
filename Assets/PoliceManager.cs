using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PoliceManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> policeNPCs = new List<GameObject>();
    public static PoliceManager instance;
    public Transform[] policeCarSpawns;
    public GameObject policeCarPrefab; // Reference to the police car prefab
    public GameObject policeSteeringAIPrefab; // Reference to the police steering AI prefab
    public GameObject policeNPCPrefab;
    public float carSpawnCooldown;
    private bool spawnedCars;

    void Start()
    {
        instance = this;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("NPC"))
        {
            if (go.GetComponent<PoliceAIBehavior>())
            {
                policeNPCs.Add(go);
            }
        }
    }

    public void CheckAlertAllNearbyCops()
    {
        if (!spawnedCars)
        {
            StartCoroutine(SpawnPoliceCarsAfterDelay(5));
            spawnedCars = true;
        }

        foreach (GameObject go in policeNPCs)
        {
            if (go)
                go.GetComponent<PoliceAIBehavior>().CheckForAlert();
        }
    }

    System.Collections.IEnumerator SpawnPoliceCarsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < 3; i++)
        {
            SpawnPoliceCar();
            yield return new WaitForSeconds(5f); // Delay between spawning each police car
        }

        // Don't spawn cars for a set cooldown
        yield return new WaitForSeconds(carSpawnCooldown);
        spawnedCars = false;
    }

    private void SpawnPoliceCar()
    {
        int randomSpawnIndex = Random.Range(0, policeCarSpawns.Length);
        Transform spawnPoint = policeCarSpawns[randomSpawnIndex];

        GameObject policeCar = Instantiate(policeCarPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject policeSteeringAI = Instantiate(policeSteeringAIPrefab, spawnPoint.position, spawnPoint.rotation);
        policeSteeringAI.GetComponent<SteeringAI>().linkedCarTransform = policeCar.transform;

        // Set the targetTransform on AICarController to the transform of the spawned PoliceSteeringAI
        AICarController carController = policeCar.GetComponent<AICarController>();
        if (carController)
        {
            carController.policeSirenSource.Play();
            carController.enabled = true;
            carController.targetTransform = policeSteeringAI.transform;
        }
    }

    public void SpawnPolice(Transform spawnPoint)
    {
        GameObject policeNPC = Instantiate(policeNPCPrefab, spawnPoint.position, spawnPoint.rotation);
        policeNPC.GetComponent<PoliceAIBehavior>().CheckForAlert();
    }
}
