using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool notInCar = true;
        float h;
        float v;

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

        private void FixedUpdate()
        {
            float handbrake = Input.GetAxis("Jump");
            if (!notInCar)
            {
                // pass the input to the car!
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
            }
            else
            {
                h = 0;
                v = 0;
                handbrake = 0;
            }

#if !MOBILE_INPUT
            
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
