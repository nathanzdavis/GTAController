//SlapChickenGames
//2024
//Simple hud referencer

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
        public TextMeshProUGUI totalMoney;
        public TextMeshProUGUI changedMoney;

        public static HudController instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
