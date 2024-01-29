using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using KovSoft.RagdollTemplate.Scripts.Charachter;
using scgGTAController;
using GTAWeaponWheel.Scripts;
using Cinemachine;
using UnityEngine.InputSystem;
using static UnityStandardAssets.Vehicles.Car.CarUserControl;

public class VehicleDoorTrigger : MonoBehaviour
{
    InputActions input;
    public enum VehicleType
    {
        Car,
        Helicopter
    }

    public VehicleType vehicleType;

    bool inTrigger;
    bool enterToggle;
    GameObject player;
    public float offsetCamCollisionDistance;
    public float carEnableTime;
    public Transform enterCarTransform;
    public Transform carFocusTransform;
    bool lockPlayer;
    public Transform seatPos;
    bool transitioning;
    private CinemachineVirtualCamera carCamera;
    private CinemachineVirtualCamera playerCamera;

    void Start()
    {
        carCamera = GameObject.FindGameObjectWithTag("carCamera").GetComponent<CinemachineVirtualCamera>();
        playerCamera = GameObject.FindGameObjectWithTag("playerCamera").GetComponent<CinemachineVirtualCamera>();
        
        input = new InputActions();

        input.Player.Enable();

        //Interaction input
        input.Player.Interact.performed += ctx =>
        {
            TryEnterVehicle();
        };

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {      
            inTrigger = true;
            player = col.gameObject;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            inTrigger = false;
        }
    }

    private void TryEnterVehicle()
    {
        if (inTrigger && !transitioning)
        {
            transitioning = true;
            enterToggle = !enterToggle;
            WeaponManager.instance.SwitchWeapon(4);
            if (enterToggle)
            {
                lockPlayer = true;
                Invoke("enableVehicle", carEnableTime);

                // Check the vehicle type and perform corresponding action
                switch (vehicleType)
                {
                    case VehicleType.Car:
                        player.GetComponent<ThirdPersonControl>().enabled = false;
                        player.GetComponent<ThirdPersonRigid>().enabled = false;
                        player.GetComponent<Ragdoll>().enabled = false;
                        player.transform.position = enterCarTransform.position;
                        player.transform.eulerAngles = enterCarTransform.eulerAngles;
                        player.GetComponent<Animator>().SetBool("doorOpen", true);
                        transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", true);
                        break;
                    case VehicleType.Helicopter:
                        player.GetComponent<ThirdPersonControl>().enabled = false;
                        player.GetComponent<ThirdPersonRigid>().enabled = false;
                        player.GetComponent<Ragdoll>().enabled = false;
                        player.transform.position = enterCarTransform.position;
                        player.transform.eulerAngles = enterCarTransform.eulerAngles;
                        player.GetComponent<Animator>().SetBool("doorOpen", true);
                        transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", true);
                        break;
                }
            }
            else
            {
                // Check the vehicle type and perform corresponding action
                switch (vehicleType)
                {
                    case VehicleType.Car:
                        Invoke("enablePlayer", carEnableTime);
                        player.transform.parent = null;
                        transform.root.gameObject.GetComponent<CarUserControl>().notInCar = true;
                        player.GetComponent<Rigidbody>().isKinematic = false;
                        player.GetComponent<CapsuleCollider>().enabled = true;
                        player.GetComponent<Animator>().SetBool("doorOpen", false);
                        transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", false);
                        break;
                    case VehicleType.Helicopter:
                        Invoke("enablePlayer", carEnableTime);
                        player.transform.parent = null;
                        transform.root.gameObject.GetComponent<HelicopterController>().IsInSeat = false;
                        player.GetComponent<Rigidbody>().isKinematic = false;
                        player.GetComponent<CapsuleCollider>().enabled = true;
                        player.GetComponent<Animator>().SetBool("doorOpen", false);
                        transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", false);
                        break;
                }

            }
        }
    }

    void Update()
    {
        if (lockPlayer)
        {
            player.transform.position = seatPos.position;
            player.transform.eulerAngles = seatPos.eulerAngles;
        }
    }

    void enableVehicle()
    {
        // Check the vehicle type and perform corresponding action
        switch (vehicleType)
        {
            case VehicleType.Car:
                transform.root.gameObject.GetComponent<CarUserControl>().enabled = true;
                transform.root.gameObject.GetComponent<CarUserControl>().notInCar = false;
                transform.root.gameObject.GetComponent<CarAudio>().enabled = true;
                transform.root.gameObject.GetComponent<CarController>().enabled = true;
                player.GetComponent<Rigidbody>().isKinematic = true;
                player.GetComponent<CapsuleCollider>().enabled = false;
                player.GetComponent<IKFeet>().enabled = false;
                player.GetComponent<ThirdPersonControl>().state = "InCar";
                var carFollowObj = GameObject.FindGameObjectWithTag("carFollowObj");
                carFollowObj.transform.position = carFocusTransform.position;
                carFollowObj.transform.rotation = carFocusTransform.rotation;
                carFollowObj.GetComponent<StickToObject>().target = carFocusTransform;
                transform.root.GetComponent<CameraController>().enabled = true;
                carCamera.Priority = 1;
                playerCamera.Priority = 0;
                transitioning = false;
                break;
            case VehicleType.Helicopter:
                transform.root.gameObject.GetComponent<HelicopterController>().enabled = true;
                transform.root.gameObject.GetComponent<HelicopterController>().IsInSeat = true;
                transform.root.gameObject.GetComponent<HelicopterController>().HelicopterSound.enabled = true;
                player.GetComponent<Rigidbody>().isKinematic = true;
                player.GetComponent<CapsuleCollider>().enabled = false;
                player.GetComponent<IKFeet>().enabled = false;
                player.GetComponent<ThirdPersonControl>().state = "InCar";
                var helicopterFollowObj = GameObject.FindGameObjectWithTag("carFollowObj");
                helicopterFollowObj.transform.position = carFocusTransform.position;
                helicopterFollowObj.transform.rotation = carFocusTransform.rotation;
                helicopterFollowObj.GetComponent<StickToObject>().target = carFocusTransform;
                transform.root.GetComponent<CameraController>().enabled = true;
                carCamera.Priority = 1;
                playerCamera.Priority = 0;
                transitioning = false;
                break;
        }

    }

    void enablePlayer()
    {
        // Check the vehicle type and perform corresponding action
        switch (vehicleType)
        {
            case VehicleType.Car:
                transform.root.gameObject.GetComponent<CarUserControl>().enabled = false;
                transform.root.GetComponent<CameraController>().enabled = false;
                lockPlayer = false;
                player.GetComponent<ThirdPersonControl>().enabled = true;
                player.GetComponent<IKFeet>().enabled = true;
                player.GetComponent<ThirdPersonRigid>().enabled = true;
                player.GetComponent<Ragdoll>().enabled = true;
                player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
                player.GetComponent<ThirdPersonControl>().state = "";
                carCamera.Priority = 0;
                playerCamera.Priority = 1;
                transitioning = false;
                break;
            case VehicleType.Helicopter:
                transform.root.gameObject.GetComponent<HelicopterController>().enabled = false;
                transform.root.GetComponent<CameraController>().enabled = false;
                lockPlayer = false;
                player.GetComponent<ThirdPersonControl>().enabled = true;
                player.GetComponent<IKFeet>().enabled = true;
                player.GetComponent<ThirdPersonRigid>().enabled = true;
                player.GetComponent<Ragdoll>().enabled = true;
                player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
                player.GetComponent<ThirdPersonControl>().state = "";
                carCamera.Priority = 0;
                playerCamera.Priority = 1;
                transitioning = false;
                break;
        }

    }
}
