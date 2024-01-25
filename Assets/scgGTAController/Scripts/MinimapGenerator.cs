using UnityEngine;

public class MinimapGenerator : MonoBehaviour
{
    // File name for the generated minimap picture
    private const string MinimapFileName = "MinimapPicture.jpg";
    // Tag for the directional light
    private const string DirectionalLightTag = "directionalLight";

    // Unity editor button function
    [UnityEditor.MenuItem("scgGTAController/Generate Minimap Picture")]
    private static void GenerateMinimap()
    {
        // Find the directional light in the scene
        GameObject directionalLightObject = GameObject.FindGameObjectWithTag(DirectionalLightTag);

        // If directional light is not found, alert the user
        if (directionalLightObject == null || directionalLightObject.GetComponent<Light>() == null)
        {
            Debug.LogError("Please add a directional light to the scene and tag it as 'directionalLight'.");
            return;
        }
        var oldDirectionalLightRotation = directionalLightObject.transform.localEulerAngles;
        directionalLightObject.transform.localEulerAngles = new Vector3(90, 0, 0);

        // Save the current shadow type
        ShadowQuality originalShadowType = QualitySettings.shadows;

        // Disable shadows for the capture
        QualitySettings.shadows = ShadowQuality.Disable;

        // Create a new orthographic camera
        Camera minimapCamera = new GameObject("MinimapCamera").AddComponent<Camera>();
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = 50; // Adjust the size as needed
        minimapCamera.aspect = 1.0f; // Set aspect ratio to 1 for a square image
        minimapCamera.clearFlags = CameraClearFlags.SolidColor; // Set clear flags to SolidColor
        minimapCamera.backgroundColor = Color.black; // Set the background color to black
        minimapCamera.transform.position = new Vector3(0, 100, 0); // Set the camera height
        minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // Look down at the world

        // Capture a snapshot of the view
        RenderTexture rt = new RenderTexture(Screen.width, Screen.width, 24);
        minimapCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.width, TextureFormat.RGB24, false);
        minimapCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.width), 0, 0);
        minimapCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        // Save the image to the root assets folder
        byte[] bytes = screenshot.EncodeToJPG();
        System.IO.File.WriteAllBytes("Assets/" + MinimapFileName, bytes);

        // Destroy the temporary camera
        DestroyImmediate(minimapCamera.gameObject); // Use DestroyImmediate to remove it from the hierarchy immediately

        directionalLightObject.transform.localEulerAngles = oldDirectionalLightRotation;

        // Set the shadow type back to the original setting
        QualitySettings.shadows = originalShadowType;

        Debug.Log("Minimap Picture generated and saved as " + MinimapFileName);
    }
}
