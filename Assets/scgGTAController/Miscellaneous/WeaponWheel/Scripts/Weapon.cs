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
        public Sprite weaponIcon;

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
            else if (!string.IsNullOrEmpty(weaponName))
            {
                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(0).gameObject.SetActive(true);
                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = weaponIcon;
                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(1).gameObject.SetActive(true);
                WeaponWheel.instance.wheels[index].wheel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = totalBullets.ToString();
            }
        }
    }
}