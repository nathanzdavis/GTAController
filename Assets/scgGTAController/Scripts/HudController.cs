//SlapChickenGames
//2024
//Simple hud referencer

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace scgGTAController
{
    public class HudController : MonoBehaviour
    {
        //Simple references to the HUD for other scripts to access and modify
        [Header("General Hud")]
        public Slider uiHealth;
        public Slider uiStamina;
        public Slider uiBullets;
        public GameObject crosshair;
        public GameObject deathText;
        public TextMeshProUGUI totalMoney;
        public TextMeshProUGUI changedMoney;

        [Header("Buy Menu")]
        public GameObject ammoNationMenu;
        public TextMeshProUGUI itemTitle;
        public TextMeshProUGUI itemPrice;
        public TextMeshProUGUI itemDescription;
        public Slider itemDamage;
        public Slider itemFireRate;
        public Slider itemAccuracy;
        public Slider itemRange;

        public static HudController instance;

        private void Awake()
        {
            instance = this;
        }
    }
}
