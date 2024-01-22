using UnityEngine;
using KovSoft.RagdollTemplate.Scripts.Charachter;

namespace GTAWeaponWheel.Scripts
{
    public class WeaponManager : MonoBehaviour
    {
    
        [SerializeField] private Weapon[] Weapons = new Weapon[8];
    
        private int m_CurrentWeaponIndex = 0;
        private Weapon m_CurrentWeapon;

        public Weapon CurrentWeapon => m_CurrentWeapon;
        public static WeaponManager instance;

        public int equippedWeapon;

        private void Start()
        {
            instance = this;
            m_CurrentWeaponIndex = equippedWeapon;
            m_CurrentWeapon = Weapons[equippedWeapon];
            SwitchWeapon(equippedWeapon);
        }

        public void SwitchWeapon(int index)
        {
            //Sets our current Weapon
            if (index > Weapons.Length)
            {
                Debug.LogError("You are trying to assign the Current weapon to a Non-Existing Weapon!");
                return;
            }
            m_CurrentWeaponIndex = index;
            for (int i = 0; i < Weapons.Length; ++i)
            {
                if (Weapons[i] == null)
                    break;
                if (i != m_CurrentWeaponIndex)
                {
                    //Disable Weapon
                    Weapons[i].gameObject.SetActive(false);
                }
                else
                {
                    //Enable Weapon
                    Weapons[i].gameObject.SetActive(true);
                    m_CurrentWeapon = Weapons[i];
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                SwitchWeapon(0);
            else if(Input.GetKeyDown(KeyCode.Alpha2))
                SwitchWeapon(1);
            else if(Input.GetKeyDown(KeyCode.Alpha3))
                SwitchWeapon(2);
            else if(Input.GetKeyDown(KeyCode.Alpha4))
                SwitchWeapon(3);
            else if(Input.GetKeyDown(KeyCode.Alpha5))
                SwitchWeapon(4);
            else if(Input.GetKeyDown(KeyCode.Alpha6))
                SwitchWeapon(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                SwitchWeapon(6);
            else if(Input.GetKeyDown(KeyCode.Alpha8))
                SwitchWeapon(7);
            else if(Input.GetKeyDown(KeyCode.Alpha9))
                SwitchWeapon(8);
        }
    }
}
