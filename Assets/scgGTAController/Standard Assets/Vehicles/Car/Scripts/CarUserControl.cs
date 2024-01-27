using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        [Header("Input")]
        InputActions input;

        private CarController m_Car; // the car controller we want to use
        public bool notInCar = true;
        private Vector2 moveInput;
        float handbrake;

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

            //Handbrake
            input.Player.Jump.canceled += ctx =>
            {
                handbrake = 0;
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
