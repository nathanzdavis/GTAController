using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GTAWeaponWheel.Scripts
{
    public class NameDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        
    
        // Start is called before the first frame update
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(_text == null || WeaponManager.instance == null || WeaponManager.instance.CurrentWeapon == null)
                    return;

            _text.text = WeaponManager.instance.CurrentWeapon.name;

        }
    }
}

