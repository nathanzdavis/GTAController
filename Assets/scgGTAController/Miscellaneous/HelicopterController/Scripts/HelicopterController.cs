using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    InputActions input;
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 3f;
    public float LookTurnForce = 1500f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
 
    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    private bool speedUpPressed;
    private bool speedDownPressed;
    private bool leftTurnActive;
    public bool IsOnGround = true;
    public bool IsInSeat;
    public LayerMask groundLayer;  // Add this variable to set the layer mask for the ground
    public float raycastDistance;
    public float rotationSpeed = 5f;
    public float rotationDamping = 1f;
    private Quaternion targetRotation;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        float tempY = 0;
        float tempX = 0;

        input = new InputActions();

        input.Player.Enable();

        //Movement
        input.Player.Move.performed += ctx =>
        {
            if (!IsOnGround)
            {
                hMove = ctx.ReadValue<Vector2>();

                // stable forward
                if (hMove.y > 0)
                {
                    tempY = Time.fixedDeltaTime;

                    if (speedDownPressed)
                    {
                        tempY = -Time.fixedDeltaTime;
                    }
                }
                else if (hMove.y < 0)
                {
                    tempY = Time.fixedDeltaTime;

                    if (speedDownPressed)
                    {
                        tempY = -Time.fixedDeltaTime;
                    }
                }

                // stable lurn
                if (hMove.x > 0)
                {
                    tempX = -Time.fixedDeltaTime;

                    if (speedDownPressed)
                    {
                        tempX = Time.fixedDeltaTime;
                    }
                }
                else if (hMove.x < 0)
                {
                    tempX = Time.fixedDeltaTime;

                    if (speedDownPressed)
                    {
                        tempX = -Time.fixedDeltaTime;
                    }
                }

                hMove.x += tempX;
                hMove.x = Mathf.Clamp(hMove.x, -1, 1);

                hMove.y += tempY;
                hMove.y = Mathf.Clamp(hMove.y, -1, 1);
            }

        };

        input.Player.Jump.performed += ctx =>
        {
            speedUpPressed = true;
        };

        input.Player.Jump.canceled += ctx =>
        {
            speedUpPressed = false;
        };

        input.Player.Crouch.performed += ctx =>
        {
            speedDownPressed = true;
        };

        input.Player.Crouch.canceled += ctx =>
        {
            speedDownPressed = false;
        };
    }

    private void FixedUpdate()
    {
        if (IsInSeat)
        {
            LiftProcess();
            MoveProcess();
            TiltProcess();
            CheckGround();
        }

        //Turning
        if (!IsOnGround)
        {
            // Calculate the direction from the object to the camera
            Vector3 directionToCamera = mainCam.transform.forward;

            // Project the direction onto the XZ plane (ignoring the vertical component)
            directionToCamera.y = 0;

            // Calculate the rotation to align with the camera's forward vector
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

            // Calculate the torque needed to reach the target rotation
            Vector3 torque = CalculateTorqueToReachRotation(targetRotation);

            HelicopterModel.AddRelativeTorque(torque * LookTurnForce);
        }
    }

    Vector3 CalculateTorqueToReachRotation(Quaternion targetRotation)
    {
        // Calculate the rotation needed to align with the target rotation
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);

        // Convert the delta rotation to an axis-angle representation
        float angle;
        Vector3 axis;
        deltaRotation.ToAngleAxis(out angle, out axis);

        // Ensure the angle is within the range [-180, 180]
        if (angle > 180f)
        {
            angle -= 360f;
        }

        // Calculate the torque needed to reach the target rotation
        Vector3 torque = axis * angle * Mathf.Deg2Rad;

        return torque;
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void Update()
    {
        if (speedUpPressed)
        {
            EngineForce += 0.1f;
        }
        else if (speedDownPressed)
        {
            EngineForce -= 0.12f;
            if (EngineForce < 0) EngineForce = 0;
        }
    }

    private void CheckGround()
    {
        raycastDistance = 1f;

        // Cast a ray downward
        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance, groundLayer))
        {
            IsOnGround = true;
        }
        else
        {
            IsOnGround = false;
        }
    }
}