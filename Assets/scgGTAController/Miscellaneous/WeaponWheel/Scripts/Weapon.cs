using scgGTAController;
using System.Collections;
using UnityEngine;

namespace GTAWeaponWheel.Scripts
{
    public class Weapon : MonoBehaviour
    {
        public int index;
        public string weaponName;
        public int totalBullets;
        public int bulletsPerMag;
        public int bulletsInMag;

        [HideInInspector] public Animator anim;
        [HideInInspector] public OffsetRotation orot;

        private void Awake()
        {
            anim = transform.root.GetComponent<Animator>();
            orot = transform.root.GetComponentInChildren<OffsetRotation>();
        }

        private void OnEnable()
        {
            if (weaponName == "Fists")
            {
                anim.SetLayerWeight(1, 0);
                orot.enabled = false;
            }
        }
    }
}