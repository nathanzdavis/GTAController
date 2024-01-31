//SlapChickenGames
//2024
//Spine orientation control

using KovSoft.RagdollTemplate.Scripts.Charachter;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public Transform flipTarget;
        private bool flipped;
        [HideInInspector] public Transform lookTarget;

        private float negScaleX;

        private void Start()
        {
            negScaleX = -transform.root.localScale.x;
        }

        private void OnDisable()
        {
            if (flipTarget)
            {
                if (flipped)
                {
                    transform.root.GetComponent<HandIK>().enabled = true;
                    flipped = false;
                }
                flipTarget.localScale = new Vector3(Mathf.Abs(transform.root.localScale.x), transform.root.localScale.y, transform.root.localScale.z);

            }
        }

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

                    if (transform.root.GetComponent<ThirdPersonControl>().moveInput.y < 0 && transform.root.GetComponent<ThirdPersonControl>().moveInput.x < 0)
                    {
                        // Subtract 30 degrees from the y-axis rotation
                        Vector3 newRotation = transform.eulerAngles - new Vector3(0f, 100f, 0f);
                        transform.eulerAngles = newRotation;
                        flipTarget.localScale = new Vector3(negScaleX, transform.root.localScale.y, transform.root.localScale.z);
                        transform.root.GetComponent<HandIK>().enabled = false;
                        flipped = true;
                    }
                    else if (transform.root.GetComponent<ThirdPersonControl>().moveInput.y < 0)
                    {
                        // Subtract 30 degrees from the y-axis rotation
                        Vector3 newRotation = transform.eulerAngles + new Vector3(0f, 25f, 0f);
                        transform.eulerAngles = newRotation;
                        if (flipTarget.localScale.x < 0)
                        {
                            flipTarget.localScale = new Vector3(Mathf.Abs(transform.root.localScale.x), transform.root.localScale.y, transform.root.localScale.z);
                            transform.root.GetComponent<HandIK>().enabled = true;
                        }
                    }
                }
                else
                {
                    if (flipped)
                    {
                        transform.root.GetComponent<HandIK>().enabled = true;
                        flipped = false;
                    }
                    flipTarget.localScale = new Vector3(Mathf.Abs(transform.root.localScale.x), transform.root.localScale.y, transform.root.localScale.z);   
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
