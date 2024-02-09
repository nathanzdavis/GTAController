using Cinemachine;
using UnityEngine;
using System.Collections;
using KovSoft.RagdollTemplate.Scripts.Charachter;
using scgGTAController;
using GTAWeaponWheel.Scripts;

public class GunWall : MonoBehaviour
{
    InputActions input;

    public Weapon[] weapons;
    public float zoomFOV;
    public float popInOutTime;
    public float popDistance;
    public AudioClip buySound;
    public AudioClip errorSound;

    private int selectedWeapon = 0;
    private bool isNavigating = false;
    private bool isSwitching;

    private CinemachineVirtualCamera playerCamera;
    private Transform previousFollow;
    private float previousFOV;

    private void OnTriggerEnter(Collider other)
    {
        // Player entered buy area
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonControl>().horizontalLocked = true;

            isNavigating = true;
            playerCamera.Follow = null;          
            playerCamera.m_Lens.FieldOfView = zoomFOV;
            HudController.instance.ammoNationMenu.SetActive(true);

            UpdateWeaponSelection();
            StartCoroutine(PopOutWeapon());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player exited buy area
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonControl>().horizontalLocked = false;

            isNavigating = false;
            playerCamera.Follow = previousFollow;
            playerCamera.LookAt = null;
            playerCamera.m_Lens.FieldOfView = previousFOV;
            HudController.instance.ammoNationMenu.SetActive(false);

            StartCoroutine(PopInWeapon(""));
        }
    }

    private void Start()
    {
        input = new InputActions();

        input.Player.Enable();
        input.UI.Enable();

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

        input.UI.Submit.performed += ctx =>
        {
            BuyWeapon();
        };

        playerCamera = GameObject.FindGameObjectWithTag("playerCamera").GetComponent<CinemachineVirtualCamera>();
        previousFollow = playerCamera.Follow;
        previousFOV = playerCamera.m_Lens.FieldOfView;
    }

    private void BuyWeapon()
    {
        if (isNavigating && MoneyManager.instance.totalMoney > weapons[selectedWeapon].value && !WeaponManager.instance.equipHand.Find(weapons[selectedWeapon].spawnablePrefab.name))
        {
            MoneyManager.instance.ChangeMoney(-weapons[selectedWeapon].value);
            WeaponManager.instance.GiveWeapon(weapons[selectedWeapon].spawnablePrefab);

            GetComponent<AudioSource>().PlayOneShot(buySound);
        }
        else if (isNavigating)
        {
            GetComponent<AudioSource>().PlayOneShot(errorSound);
        }
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
        playerCamera.LookAt = weapons[selectedWeapon].transform.transform;

        HudController.instance.itemTitle.text = weapons[selectedWeapon].buyTitle;
        HudController.instance.itemPrice.text = "$" + weapons[selectedWeapon].value;
        HudController.instance.itemDescription.text = weapons[selectedWeapon].description;
        HudController.instance.itemDamage.value = (float)weapons[selectedWeapon].damage / 100;
        HudController.instance.itemFireRate.value = (float)weapons[selectedWeapon].fireRateStat / 100;
        HudController.instance.itemAccuracy.value = (float)weapons[selectedWeapon].accuracy / 100;
        HudController.instance.itemRange.value = (float)weapons[selectedWeapon].range / 100;
    }

    // Coroutine to smoothly pop in the selected weapon
    private IEnumerator PopInWeapon(string state)
    {
        isSwitching = true;
        Transform selectedWeaponTransform = weapons[selectedWeapon].transform;
        Vector3 targetPosition = selectedWeaponTransform.position - new Vector3(popDistance, 0f, 0f);

        float elapsedTime = 0f;
        float popDuration = popInOutTime; // Adjust as needed

        Vector3 startPopPosition = selectedWeaponTransform.position;

        // Pop in
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
        Transform selectedWeaponTransform = weapons[selectedWeapon].transform;
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
}
