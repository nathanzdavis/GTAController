using scgGTAController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GTAWeaponWheel.Scripts
{
    public class WeaponWheel : MonoBehaviour
    {
        InputActions input;
        public static WeaponWheel instance;
        [SerializeField] private GameObject wheelParent;
        [SerializeField] private GameObject blur;
        
        private bool m_WheelEnabled;
        private Camera playerCamera;
        private GameObject player;
        [SerializeField] private float targetTimeScale = 0.3f, timeToGoToTargetTimeScale = 0.1f;  //Adds A Slow Motion Effect when weapon wheel is enabled
        private float m_TimeV;
        
        [Serializable]
        public class Wheel
        {
            public Sprite highlightSprite;
            private Sprite m_NormalSprite;
            public Image wheel;

            public Sprite NormalSprite
            {
                get => m_NormalSprite;
                set => m_NormalSprite = value;
            }
        }

        public Wheel[] wheels = new Wheel[8];

        
        [Header("Dots & Lines")]
        [SerializeField] private Transform[] dots = new Transform[9];
        private Vector2[] pos = new Vector2[9];
        private Vector3 start, end;

        [Header("Selection")]
        private Vector2 selectionPos;

        public bool WheelEnabled => m_WheelEnabled;

        private void OnDrawGizmosSelected()
        {
            start.x = pos[0].x;
            start.y = pos[0].y;
            start.z = dots[0].position.z;
            for (int i = 0; i < pos.Length; ++i)
            {
                end.x = pos[i].x;
                end.y = pos[i].y;
                end.z = dots[i].position.z;
                Debug.DrawLine(start, end, Color.red);
            }
            for (int i = 0; i < pos.Length - 1; ++i)
            {
                start.x = pos[i].x;
                start.y = pos[i].y;
                start.z = dots[i].position.z;
                end.x = pos[i+1].x;
                end.y = pos[i+1].y;
                end.z = dots[i+1].position.z;
                Debug.DrawLine(start, end, Color.red);
            }
            //For the Last Triangle
            start.x = pos[8].x;
            start.y = pos[8].y;
            start.z = dots[8].position.z;
            end.x = pos[1].x;
            end.y = pos[1].y;
            end.z = dots[1].position.z;
            Debug.DrawLine(start, end, Color.red);
            
        }


        private float Area(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return Mathf.Abs((v1.x *(v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) / 2f);
        }

        private bool IsInside(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v)
        {
            float A = Area(v1, v2, v3);
            float A1 = Area(v1, v2, v);
            float A2 = Area(v1, v, v3);
            float A3 = Area(v, v2, v3);

            return (Mathf.Abs(A1 + A2 + A3 - A) < 1f);
        }
        private void EnableHighlight(int index)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null && wheels[i].highlightSprite != null)
                {
                    if(i == index)
                    {
                        wheels[i].wheel.sprite = wheels[i].highlightSprite;
                    }
                    else
                    {
                        wheels[i].wheel.sprite = wheels[i].NormalSprite;

                    }
                }
            }
        }
        
        private void DisableAllHighlight()
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null)
                    wheels[i].wheel.sprite = wheels[i].NormalSprite;
            }
        }

        private void CheckForCurrentWeapon()
        {
            if(playerCamera == null)
                return;

            for (int i = 0; i < pos.Length; i++)
            {
                //Changing World coordinates to screen coordinates
                pos[i] = playerCamera.WorldToScreenPoint(dots[i].position);
            }

            if (selectionPos.magnitude > 0.5f)
            {
                SelectWeapon(selectionPos);
            }
        }

        void SelectWeapon(Vector2 stickInput)
        {
            float angle = Mathf.Atan2(stickInput.x, -stickInput.y) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360;

            // Calculate the sector size based on the number of items on the wheel
            float sectorSize = 360f / 8f;

            // Calculate the selected sector
            int selectedSector = Mathf.FloorToInt((angle + sectorSize / 2) / sectorSize) % 8;

            // Highlight the selected slot and switch the weapon
            EnableHighlight(selectedSector);
            WeaponManager.instance.SwitchWeapon(selectedSector);

            Debug.Log("Selected Weapon: " + selectedSector);
        }

        // Start is called before the first frame update
        private void Start()
        {
            input = new InputActions();

            input.Player.Enable();

            //Joystick look value
            input.Player.Look.performed += ctx =>
            {
                selectionPos = ctx.ReadValue<Vector2>();
            };

            //Interaction
            input.Player.InteractionWheel.performed += ctx =>
            {
                ShowWheel();
            };

            input.Player.InteractionWheel.canceled += ctx =>
            {
                HideWheel();
            };

            player = GameObject.FindGameObjectWithTag("Player");
            prevSense = player.GetComponent<CameraController>().lookSense;

            instance = this;
            DisableWheel();

            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null)
                {
                    wheels[i].NormalSprite = wheels[i].wheel.sprite;
                }
            }
            //Initializing the position vectors
            for (int i = 0; i < dots.Length; i++)
            {
                pos[i].x = dots[i].position.x;
                pos[i].y = dots[i].position.y;
            }

            playerCamera = Camera.main;
        }

        private void EnableWheel()
        {
            if(wheelParent != null)
                wheelParent.SetActive(true);
            m_WheelEnabled = true;
            if (blur != null)
                blur.SetActive(true);
        }

        private void DisableWheel()
        {
            if(wheelParent != null)
                wheelParent.SetActive(false);
            m_WheelEnabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (blur != null)
                blur.SetActive(false);
        }

        float prevSense;

        // Update is called once per frame
        void Update()
        {
            if (player)
            {
                if (m_WheelEnabled)
                {
                    CheckForCurrentWeapon();
                    Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref m_TimeV, timeToGoToTargetTimeScale);

                }
                else if (player.GetComponent<HealthController>().health > 0)
                    Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f, ref m_TimeV, timeToGoToTargetTimeScale);

            }
        }

        private void ShowWheel()
        {
            //Enable Wheel Mode
            EnableWheel();
            player.GetComponent<CameraController>().lookSense = 0;
        }

        private void HideWheel()
        {
            //Disable Wheel Mode
            DisableWheel();
            player.GetComponent<CameraController>().lookSense = prevSense;
        }
    }
}
