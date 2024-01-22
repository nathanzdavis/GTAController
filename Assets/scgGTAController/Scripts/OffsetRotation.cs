//SlapChickenGames
//2024
//Spine orientation control

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace scgGTAController
{
    public class OffsetRotation : MonoBehaviour
    {
        public float offsetRotationRifle;
        public float offsetRotationPistol;
        public float offsetRotationNormal;
        public bool rifle;
        public bool pistol;
        [HideInInspector] public Transform lookTarget;

        void LateUpdate()
        {
            if (rifle)
            {
                if (lookTarget)
                {
                    // Look at the target
                    transform.LookAt(lookTarget);

                    // Add yOffset to the y rotation
                    transform.Rotate(Vector3.up, offsetRotationRifle);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + offsetRotationRifle, 0);
                }
            }
            else if (pistol)
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + offsetRotationPistol, 0);
            else
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + offsetRotationNormal, 0);
        }
    }
}
