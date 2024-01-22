using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> policeNPCs = new List<GameObject>();

    public static PoliceManager instance;

    // Start is called before the first frame update
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
        foreach (GameObject go in policeNPCs)
        {
            if (go)
                go.GetComponent<PoliceAIBehavior>().CheckForAlert();
        }
    }
}
