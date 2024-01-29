using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraController : MonoBehaviour
{
    
    InputActions input;

    [HideInInspector] public GameObject followTransform;
    public Vector2 _look;
    public float lookSense;
    public float aimValue;
    public float fireValue;

    public float rotationPower = 3f;
    public float rotationLerp = 0.5f;

    public float maxVerticalAngle;
    public float minVerticalAngle;

    public bool isCarCameraController;

    private void Start()
    {
        input = new InputActions();

        input.Player.Enable();

        //Look input
        input.Player.Look.performed += ctx =>
        {
            _look = ctx.ReadValue<Vector2>();
        };

        input.Player.Look.canceled += ctx =>
        {
            _look = Vector2.zero;
        };

        if (isCarCameraController)
        {
            followTransform = GameObject.FindGameObjectWithTag("carFollowObj");
        }
        else
        {
            followTransform = GameObject.FindGameObjectWithTag("playerFollowObj");
        }
    }

    private void FixedUpdate()
    {
        #region Player Based Rotation
        
        //Move the player based on the X input on the controller
        //transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);

        #endregion

        #region Follow Transform Rotation

        //Rotate the Follow Target transform based on the input
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);

        #endregion

        #region Vertical Rotation
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < minVerticalAngle)
        {
            angles.x = minVerticalAngle;
        }
        else if(angle < 180 && angle > maxVerticalAngle)
        {
            angles.x = maxVerticalAngle;
        }


        followTransform.transform.localEulerAngles = angles;
        #endregion

        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, angles.y, 0);
    }
}
