using scgGTAController;
using TMPro;
using UnityEngine;

namespace GTAWeaponWheel.Scripts
{
    public class AmmoDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        public bool displayCurrentWeaponAmmo = false; // If true, then displays current weapon ammo

        [SerializeField] private Weapon m_Weapon;
    
        // Start is called before the first frame update
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if (displayCurrentWeaponAmmo)
            {
                if(_text == null || WeaponManager.instance == null || WeaponManager.instance.CurrentWeapon == null)
                    return;

                _text.text = WeaponManager.instance.CurrentWeapon.bulletsInMag + "/<color=#6B6C64>" +
                             WeaponManager.instance.CurrentWeapon.totalBullets;

                if (WeaponManager.instance.CurrentWeapon.bulletsInMag > 0)
                {
                    var ammoPercentage = (float)WeaponManager.instance.CurrentWeapon.bulletsInMag / (float)WeaponManager.instance.CurrentWeapon.bulletsPerMag;
                    HudController.instance.uiBullets.value = ammoPercentage;
                }
                else
                {
                    HudController.instance.uiBullets.value = 0;
                }

            }
            else
            {
                if(_text == null || m_Weapon == null)
                    return;

                _text.text = m_Weapon.bulletsInMag + "/" +
                             m_Weapon.totalBullets;
            }
        }
    }
}
