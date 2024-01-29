using System;
using UnityEngine;
using static DroppedItem;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {        
        InputActions input;
        public enum CarType
        {
            Regular,
            Police
        }

        public CarType carType;
        public GameObject[] headlights;
        public GameObject sirenGroup;
        public bool notInCar = true;
        
        private CarController m_Car; // the car controller we want to use
        private Vector2 moveInput;
        private float handbrake;


        private void OnEnable()
        {
            if (GetComponent<AICarController>())
                GetComponent<AICarController>().enabled = false;
        }
        private void OnDisable()
        {
            GetComponent<CarAudio>().enabled = false;
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private void Start()
        {
            input = new InputActions();

            input.Player.Enable();

            //Movement
            input.Player.Move.performed += ctx =>
            {
                if (ctx.ReadValue<Vector2>().x > 0.5f || ctx.ReadValue<Vector2>().x < -0.5f)
                    moveInput.x = ctx.ReadValue<Vector2>().x;
                else
                    moveInput.x = 0;

                moveInput.y = ctx.ReadValue<Vector2>().y;
            };

            //Handbrake
            input.Player.Jump.performed += ctx =>
            {
                handbrake = 1;
            };

            input.Player.Jump.canceled += ctx =>
            {
                handbrake = 0;
            };

            //Lights
            input.Player.CarLights.performed += ctx =>
            {
                if (!notInCar)
                {
                    foreach (GameObject go in headlights)
                    {
                        go.SetActive(!go.activeSelf);
                    }
                }
            };

            //Auxillary
            input.Player.CarAuxillary.performed += ctx =>
            {
                // Check the car type and perform corresponding action
                switch (carType)
                {
                    case CarType.Police:
                        sirenGroup.SetActive(!sirenGroup.activeSelf);
                        break;
                        // Add more cases for other item types if needed
                }
            };
        }

        private void FixedUpdate()
        {
            if (notInCar)
            {
                moveInput.x = 0;
                moveInput.y = 0;
                handbrake = 0;
            }

#if !MOBILE_INPUT
            
            m_Car.Move(moveInput.x, moveInput.y, moveInput.y, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
