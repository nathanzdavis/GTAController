using scgGTAController;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GTAWeaponWheel.Scripts
{
    public class Weapon : MonoBehaviour
    {
        public int index;
        public string weaponName;
        public int totalBullets;
        public int bulletsPerMag;
        public int bulletsInMag;
        public int damage;
        public int accuracy;
        public float fireRate;
        public int fireRateStat;
        public int range;
        public int value;
        public string buyTitle;
        public string description;
        public Sprite weaponIcon;
        public GameObject spawnablePrefab;

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

                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(0).gameObject.SetActive(true);
                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = weaponIcon;
            }
        }
    }
}