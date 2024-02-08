using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using KovSoft.RagdollTemplate.Scripts.Charachter;

public class GunWall : MonoBehaviour
{
    InputActions input;

    public Transform[] weapons;
    public float zoomFOV;
    public float popInOutTime;
    public float popDistance;

    private int selectedWeapon = 0;
    private bool isNavigating = false;
    private bool isSwitching;

    private CinemachineVirtualCamera playerCamera;
    private Transform previousFollow;
    private float previousFOV;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonControl>().horizontalLocked = true;

            // Player entered trigger zone
            StartNavigation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonControl>().horizontalLocked = false;

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
            if (isNavigating && !isSwitching)
            {
                float horizontalInput = ctx.ReadValue<Vector2>().x;
                float verticalInput = ctx.ReadValue<Vector2>().y;

                if (horizontalInput > 0)
                {
                    SelectNextWeapon();
                }
                else if (horizontalInput < 0)
                {
                    SelectPreviousWeapon();
                }

                // Vertical input for up down navigation (disabled so player can walk backwards out of zone)
                /*
                else if (verticalInput > 0)
                {
                    SelectPreviousWeapon();
                }
                else if (verticalInput < 0)
                {
                    SelectNextWeapon();
                }
                */
            }
        };

        playerCamera = GameObject.FindGameObjectWithTag("playerCamera").GetComponent<CinemachineVirtualCamera>();
        previousFollow = playerCamera.Follow;
        previousFOV = playerCamera.m_Lens.FieldOfView;
    }

    private void SelectNextWeapon()
    {
        StartCoroutine(PopInWeapon("Next"));
    }

    private void SelectPreviousWeapon()
    {
        StartCoroutine(PopInWeapon("Previous"));
    }

    private void UpdateWeaponSelection()
    {
        playerCamera.LookAt = weapons[selectedWeapon];
    }

    // Coroutine to smoothly pop in the selected weapon
    private IEnumerator PopInWeapon(string state)
    {
        isSwitching = true;
        Transform selectedWeaponTransform = weapons[selectedWeapon];
        Vector3 targetPosition = selectedWeaponTransform.position - new Vector3(popDistance, 0f, 0f);

        float elapsedTime = 0f;
        float popDuration = popInOutTime; // Adjust as needed

        Vector3 startPopPosition = selectedWeaponTransform.position;

        // Pop out
        while (elapsedTime < popDuration)
        {
            selectedWeaponTransform.position = Vector3.Lerp(startPopPosition, targetPosition, elapsedTime / popDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectedWeaponTransform.position = targetPosition;

        if (state == "Next")
        {
            selectedWeapon = (selectedWeapon + 1) % weapons.Length;
            UpdateWeaponSelection();

            StartCoroutine(PopOutWeapon());
        }
        else if (state == "Previous")
        {
            selectedWeapon = (selectedWeapon - 1 + weapons.Length) % weapons.Length;
            UpdateWeaponSelection();

            StartCoroutine(PopOutWeapon());
        }
    }

    // Coroutine to smoothly pop out the selected weapon
    private IEnumerator PopOutWeapon()
    {
        Transform selectedWeaponTransform = weapons[selectedWeapon];
        Vector3 targetPosition = selectedWeaponTransform.position + new Vector3(popDistance, 0f, 0f);

        float elapsedTime = 0f;
        float popDuration = popInOutTime; // Adjust as needed

        Vector3 startPopPosition = selectedWeaponTransform.position;

        // Pop out
        while (elapsedTime < popDuration)
        {
            selectedWeaponTransform.position = Vector3.Lerp(startPopPosition, targetPosition, elapsedTime / popDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectedWeaponTransform.position = targetPosition;

        isSwitching = false;
    }

    private void StartNavigation()
    {
        isNavigating = true;
        playerCamera.Follow = null;
        playerCamera.LookAt = weapons[selectedWeapon];
        playerCamera.m_Lens.FieldOfView = zoomFOV;
        StartCoroutine(PopOutWeapon());
    }

    private void StopNavigation()
    {
        isNavigating = false;
        playerCamera.Follow = previousFollow;
        playerCamera.LookAt = null;
        playerCamera.m_Lens.FieldOfView = previousFOV;
        StartCoroutine(PopInWeapon(""));
    }
}
