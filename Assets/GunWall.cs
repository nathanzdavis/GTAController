using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GunWall : MonoBehaviour
{
    InputActions input;

    public Transform[] weapons;
    public float navigationDelay = 0.2f;
    public float zoomFOV;

    private int selectedWeapon = 0;
    private bool isNavigating = false;

    private CinemachineVirtualCamera playerCamera;
    private Transform previousFollow;
    private float previousFOV;

    private bool isPoppingOut = false;
    private bool isPoppingIn = false;
    private Vector3[] originalWeaponPositions;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered trigger zone
            StartNavigation();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player exited trigger zone
            StopNavigation();
        }
    }

    private void Start()
    {
        input = new InputActions();

        input.Player.Enable();

        input.Player.Move.performed += ctx =>
        {
            if (isNavigating)
            {
                float horizontalInput = ctx.ReadValue<Vector2>().x;
                float verticalInput = ctx.ReadValue<Vector2>().y;

                if (Mathf.Abs(horizontalInput) > 0.5f || Mathf.Abs(verticalInput) > 0.5f)
                {
                    // Reset the position of the previously selected weapon when moving to a different one
                    ResetWeaponPosition(selectedWeapon);
                }

                if (horizontalInput > 0)
                {
                    SelectNextWeapon();
                }
                else if (horizontalInput < 0)
                {
                    SelectPreviousWeapon();
                }
                else if (verticalInput > 0)
                {
                    SelectPreviousWeapon();
                }
                else if (verticalInput < 0)
                {
                    SelectNextWeapon();
                }
            }
        };

        playerCamera = GameObject.FindGameObjectWithTag("playerCamera").GetComponent<CinemachineVirtualCamera>();
        previousFollow = playerCamera.Follow;
        previousFOV = playerCamera.m_Lens.FieldOfView;

        // Save the original positions of all weapons
        SaveOriginalWeaponPositions();
    }

    void SaveOriginalWeaponPositions()
    {
        originalWeaponPositions = new Vector3[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            originalWeaponPositions[i] = weapons[i].position;
        }
    }

    void ResetWeaponPosition(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            weapons[weaponIndex].position = originalWeaponPositions[weaponIndex];
        }
    }

    void SelectNextWeapon()
    {
        ResetWeaponPosition(selectedWeapon);
        selectedWeapon = (selectedWeapon + 1) % weapons.Length;
        UpdateWeaponSelection();
        StartCoroutine(PopInOutWeapon());
    }

    void SelectPreviousWeapon()
    {
        ResetWeaponPosition(selectedWeapon);
        selectedWeapon = (selectedWeapon - 1 + weapons.Length) % weapons.Length;
        UpdateWeaponSelection();
        StartCoroutine(PopInOutWeapon());
    }

    void UpdateWeaponSelection()
    {
        playerCamera.LookAt = weapons[selectedWeapon];
    }

    // Coroutine to smoothly pop in and out the selected weapon
    IEnumerator PopInOutWeapon()
    {
        if (isPoppingOut || isPoppingIn) yield break; // Prevent starting multiple coroutines

        isPoppingOut = true;

        Transform selectedWeaponTransform = weapons[selectedWeapon];
        Vector3 targetPosition = selectedWeaponTransform.position + new Vector3(5f, 0f, 0f);

        float elapsedTime = 0f;
        float popDuration = 0.5f; // Adjust as needed

        Vector3 startPopPosition = selectedWeaponTransform.position;

        // Pop out
        while (elapsedTime < popDuration)
        {
            selectedWeaponTransform.position = Vector3.Lerp(startPopPosition, targetPosition, elapsedTime / popDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectedWeaponTransform.position = targetPosition;

        isPoppingOut = false;

        // Wait for a delay before popping in
        yield return new WaitForSeconds(navigationDelay);

        isPoppingIn = true;

        // Pop in
        elapsedTime = 0f;
        targetPosition = originalWeaponPositions[selectedWeapon];

        while (elapsedTime < popDuration)
        {
            selectedWeaponTransform.position = Vector3.Lerp(targetPosition, startPopPosition, elapsedTime / popDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectedWeaponTransform.position = startPopPosition;

        isPoppingIn = false;
    }

    void StartNavigation()
    {
        isNavigating = true;
        playerCamera.Follow = null;
        playerCamera.LookAt = weapons[selectedWeapon];
        playerCamera.m_Lens.FieldOfView = zoomFOV;
    }

    void StopNavigation()
    {
        isNavigating = false;
        playerCamera.Follow = previousFollow;
        playerCamera.LookAt = null;
        playerCamera.m_Lens.FieldOfView = previousFOV;
    }
}
