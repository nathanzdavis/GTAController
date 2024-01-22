//SlapChickenGames
//2024
//Simple hud referencer

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace scgGTAController
{
    public class HudController : MonoBehaviour
    {
        //Simple references to the HUD for other scripts to access and modify
        public Slider uiHealth;
        public Slider uiStamina;
        public Slider uiBullets;
        public GameObject crosshair;
        public GameObject deathText;

        public static HudController instance;

        private void Start()
        {
            instance = this;
        }
    }
}
