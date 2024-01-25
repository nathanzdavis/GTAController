using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using KovSoft.RagdollTemplate.Scripts.Charachter;
using scgGTAController;
using GTAWeaponWheel.Scripts;
using Cinemachine;

public class CarDoorTrigger : MonoBehaviour
{
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

    void Update()
    {
        if (inTrigger && !transitioning)
        {
            if (Input.GetButtonDown("Action")){
                transitioning = true;
                enterToggle = !enterToggle;
                WeaponManager.instance.SwitchWeapon(4);
                if (enterToggle)
                {
                    lockPlayer = true;
                    Invoke("enableCar", carEnableTime);
                    player.GetComponent<ThirdPersonControl>().enabled = false;                   
                    player.transform.position = enterCarTransform.position;
                    player.transform.eulerAngles = enterCarTransform.eulerAngles;
                    player.GetComponent<Animator>().SetBool("doorOpen", true);
                    transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", true);
                }
                else
                {
                    Invoke("enablePlayer", carEnableTime);
                    player.transform.parent = null;
                    transform.root.gameObject.GetComponent<CarUserControl>().notInCar = true;
                    player.GetComponent<Rigidbody>().isKinematic = false;
                    player.GetComponent<CapsuleCollider>().enabled = true;
                    player.GetComponent<Animator>().SetBool("doorOpen", false);
                    transform.root.gameObject.GetComponent<Animator>().SetBool("doorOpen", false);
                }            
            }
        }

        if (lockPlayer)
        {
            player.transform.position = seatPos.position;
            player.transform.eulerAngles = seatPos.eulerAngles;
        }
    }

    void enableCar()
    {
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
    }

    void enablePlayer()
    {
        transform.root.gameObject.GetComponent<CarUserControl>().enabled = false;
        transform.root.GetComponent<CameraController>().enabled = false;
        lockPlayer = false;
        player.GetComponent<ThirdPersonControl>().enabled = true;
        player.GetComponent<IKFeet>().enabled = true;
        player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        player.GetComponent<ThirdPersonControl>().state = "";
        carCamera.Priority = 0;
        playerCamera.Priority = 1;
        transitioning = false;
    }
}
