using System.Collections;
using UnityEngine;

public class StrobeLight : MonoBehaviour
{
    public Light strobeLight;
    public float onTime = 0.1f;     // Time the light is on during each cycle
    public float offTime = 0.1f;    // Time the light is off during each cycle

    void OnEnable()
    {
        if (strobeLight == null)
        {
            strobeLight = GetComponent<Light>();
        }

        if (strobeLight == null)
        {
            Debug.LogError("StrobeLight script requires a Light component. Attach a Light component to the same GameObject or assign it through the inspector.");
            enabled = false;
            return;
        }

        // Start the strobe coroutine
        StartCoroutine(StrobeCoroutine());
    }

    IEnumerator StrobeCoroutine()
    {
        while (true)
        {
            // Toggle the light on
            strobeLight.enabled = true;
            yield return new WaitForSeconds(onTime);

            // Toggle the light off
            strobeLight.enabled = false;
            yield return new WaitForSeconds(offTime);
        }
    }
}
