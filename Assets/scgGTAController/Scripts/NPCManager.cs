using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    private void Start()
    {
        instance = this;
    }

    public void CheckAlert(Transform origin)
    {
        // Check for nearby NPCs within 15 meters and alert them
        Collider[] colliders = Physics.OverlapSphere(origin.position, 15f);
        foreach (Collider collider in colliders)
        {
            // Check if the collider has an AIController component
            AIController aiController = collider.GetComponent<AIController>();
            if (aiController != null)
            {
                // Call the Alert() function on the AIController
                aiController.Alert();
            }
        }
    }
}
