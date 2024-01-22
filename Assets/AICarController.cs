using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class AICarController : MonoBehaviour
    {
        private CarController m_Car;
        public bool notInCar = true;

        public Transform targetTransform; // Specify the target transform here

        private bool reversing;
        private bool checkingReversing;
        private Coroutine reverseCoroutine;

        float handbrake = 0;
        float h = 0;
        float v = 0;

        private void OnEnable()
        {
            GetComponent<CarUserControl>().enabled = false;
            GetComponent<CarAudio>().enabled = true;
        }

        private void OnDisable()
        {
            GetComponent<CarUserControl>().enabled = true;
        }

        private void Awake()
        {
            m_Car = GetComponent<CarController>();
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            if (!notInCar && targetTransform != null)
            {
                // Get the current direction of the car
                Vector3 carDirection = transform.forward;

                // Calculate the target direction
                Vector3 targetDirection = targetTransform.position - transform.position;

                // Project the target direction onto the car's local space
                Vector3 localTargetDirection = transform.InverseTransformDirection(targetDirection);

                // Calculate the angle between current and target direction
                float angle = Mathf.Atan2(localTargetDirection.x, localTargetDirection.z) * Mathf.Rad2Deg;

                // Adjust input based on the angle
                h = Mathf.Clamp(angle / 180f, -1f, 1f);

                if (Vector3.Distance(targetTransform.position, transform.position) > 5)
                {
                    handbrake = 0f;
                    if (m_Car.CurrentSpeed < 10f && !reversing)
                    {
                        print("moving forwards");
                        v = 1f; // Set forward input only if not reversing
                    }   
                    else if (reversing)
                    {
                        v = -1f;
                        h = -h;
                        if (m_Car.CurrentSpeed > 10f)
                        {
                            v = 0;
                        }
                    }
                    else
                    {
                        v = 0;
                    }

                    if (v > 0 && m_Car.CurrentSpeed < 0.1f && handbrake == 0 && !checkingReversing)
                    {
                        StartCoroutine(ReverseAndWait());
                    }
                }
                else
                {
                    h = 0;
                    v = 0;
                    handbrake = 1f;
                }

            }

            m_Car.Move(h, v, v, handbrake);

            //print(m_Car.CurrentSpeed);
        }

        private IEnumerator ReverseAndWait()
        {
            checkingReversing = true;
            yield return new WaitForSeconds(2f);
            if (m_Car.CurrentSpeed < 0.1f)
            {
                reversing = true;
                yield return new WaitForSeconds(3f); // Wait for 2 seconds while reversing
                reversing = false;
            }
            checkingReversing = false;
        }
    }
}
