//SlapChickenGames
//2024
//Hand and Head IK system 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgGTAController
{
    public class Adjuster : MonoBehaviour
    {
        public Transform headBone;
        public Transform holdPoint;
        public Transform handBone;
        public Transform leftArmBone;
        public Transform rightArmBone;
        public Vector3 headOffsetRot = new Vector3(0, 0, 0);
        public Vector3 indexFingerOffsetRot = new Vector3(0, 0, 0);
        public Vector3 leftArmOffset = new Vector3(0, 0, 0);
        public Vector3 rightArmOffset = new Vector3(0, 0, 0);
        public bool adjustIndexFinger;
        public Transform indexFinger;
        public bool isAi;
        public bool useLeftHandIK;
        public bool adjustLeftArm;
        public bool adjustRightArm;
        void LateUpdate()
        {
            //headBone.eulerAngles = new Vector3(headBone.eulerAngles.x + headOffsetRot.x, headBone.eulerAngles.y + headOffsetRot.y, headBone.eulerAngles.z + headOffsetRot.z);

            if (!isAi && useLeftHandIK)
            {
                if (!gameObject.GetComponent<GunController>().reloading && !gameObject.GetComponent<GunController>().throwing)
                {
                    handBone.transform.position = holdPoint.transform.position;
                }
            }


            if (adjustIndexFinger)
            {
                indexFinger.localEulerAngles = new Vector3(indexFinger.localEulerAngles.x + indexFingerOffsetRot.x, indexFinger.localEulerAngles.y + indexFingerOffsetRot.y, indexFinger.localEulerAngles.z + indexFingerOffsetRot.z);
            }

            if (adjustLeftArm)
            {
                leftArmBone.localEulerAngles = new Vector3(leftArmBone.localEulerAngles.x + leftArmOffset.x, leftArmBone.localEulerAngles.y + leftArmOffset.y, leftArmBone.localEulerAngles.z + leftArmOffset.z);
            }

            if (adjustRightArm)
            {
                rightArmBone.localEulerAngles = new Vector3(rightArmBone.localEulerAngles.x + rightArmOffset.x, rightArmBone.localEulerAngles.y + rightArmOffset.y, rightArmBone.localEulerAngles.z + rightArmOffset.z);
            }
        }
    }
}
