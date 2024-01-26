//SlapChickenGames
//2024
//Main weapon controller supporting multiple firing types

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KovSoft.RagdollTemplate.Scripts.Charachter;
using GTAWeaponWheel.Scripts;
using TMPro;

namespace scgGTAController
{
    [RequireComponent(typeof(AudioSource))]
    public class GunController : Weapon
    {
        [HideInInspector] public bool reloading = false;
        [HideInInspector] public bool firing = false;
        [HideInInspector] public bool recoilAuto = false;
        [HideInInspector] public bool recoilSemi = false;
        [HideInInspector] public bool aiming = false;
        [HideInInspector] public bool throwing = false;
        [HideInInspector] public bool cycling = false;

        public enum WeaponTypes { Rifle, Pistol };
        public enum ShootTypes { SemiAuto, FullAuto, BoltAction };
        public enum HoldType { Normal, Dual};

        [Header("Input")]
        InputActions input;
        private bool firePressed;
        private bool aimPressed;
        private bool reloadPressed;

        [Header("WeaponType")]
        public WeaponTypes Weapon;

        [Header("ShootType")]
        public ShootTypes shootType;

        [Header("Hold Type")]
        public HoldType holdType;

        [Header("Animations")]
        public float rifleLayerWeight;
        public float pistolLayerWeight;
        public float rifleArmsLayerWeight;
        public float pistolArmsLayerWeight;
        public Transform leftHandHoldPoint;

        [Header("Camera")]
        private GameObject mainCam;
        Vector3 originalCamPos;

        [Header("Shooting")]
        public ParticleSystem[] muzzleFlashes;
        public GameObject ejectionPoint;
        public GameObject magDropPoint;
        public GameObject Bullet;
        public GameObject Shell;
        public GameObject Mag;
        public float bulletVelocity;
        public float bulletDespawnTime;
        public float shellVelocity;
        public float magVelocity;
        public float shellDespawnTime;
        public float magDespawnTime;
        public float cycleTimeBoltAction;
        public float cycleTimeSemiAuto;   

        [Header("Timing")]
        public float reloadTime;
        public float grenadeTime;
        public float fireRate;

        [Header("Damage")]
        public int Damage;

        [Header("Aiming")]
        public Vector3 aimPosition;
        public Vector3 holdingHandOffsetRot;
        public Transform mainHandTransform;
        Vector3 originalAimRot;
        Vector3 originalAimOffsetCamPos;
        public float aimTime;
        public float zoomInAmount;
        float originalCamFov;
        public float aimInOutDuration;
        float aimTimeElapsed;
        [HideInInspector] public bool aimFinished = true;
        [HideInInspector] public bool swapping;
        [HideInInspector] public bool aimPosSet;

        [Header("Sounds")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        Coroutine lastRoutine = null;

        [Header("Orienting")]
        public Transform lookTarget;

        bool coroutineRunning;

        private void OnEnable()
        {
            if (Weapon == WeaponTypes.Rifle)
            {
                anim.SetLayerWeight(1, rifleLayerWeight);
                orot.rifle = true;
                orot.pistol = false;
            }
            else if (Weapon == WeaponTypes.Pistol)
            {
                anim.SetLayerWeight(1, pistolLayerWeight);
                orot.pistol = true;
                orot.rifle = false;
            }
            else
            {
                anim.SetLayerWeight(1, 0);
                anim.SetLayerWeight(2, 0);
            }

            originalCamFov = Camera.main.GetComponent<Camera>().fieldOfView;

            transform.root.gameObject.GetComponent<HandIK>().targetPoint = leftHandHoldPoint;
        }

        private void OnDisable()
        {
            transform.root.gameObject.GetComponent<HandIK>().targetPoint = null;
        }

        private void Start()
        {
            input = new InputActions();

            input.Player.Enable();

            //Attack input
            input.Player.Attack.performed += ctx =>
            {
                firePressed = true;
            };

            input.Player.Attack.canceled += ctx =>
            {
                firePressed = false;
            };

            //Aim input
            input.Player.Aim.performed += ctx =>
            {
                aimPressed = true;
            };

            input.Player.Aim.canceled += ctx =>
            {
                aimPressed = false;
            };

            //Reload input
            input.Player.Reload.performed += ctx =>
            {
                reloadPressed = true;
            };

            input.Player.Reload.canceled += ctx =>
            {
                reloadPressed = false;
            };

            mainCam = Camera.main.gameObject;
            lookTarget = GameObject.FindGameObjectWithTag("playerFollowObj").transform.GetChild(0);

            //Set the ammo count
            bulletsInMag = bulletsPerMag;
            originalCamPos = mainCam.transform.localPosition;
        }

        private void Update()
        {
            //Input and actions for shooting
            if (firePressed && !firing && reloading == false && bulletsInMag > 0 && !cycling && !swapping)
            {
                firing = true;
                foreach (ParticleSystem ps in muzzleFlashes)
                {
                    ps.Play();
                }
                gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
                spawnBullet();
                bulletsInMag--;

                if (shootType == ShootTypes.FullAuto)
                {
                    spawnShell();
                    recoilAuto = true;
                    recoilSemi = false;
                    lastRoutine = StartCoroutine(shootBullet());
                }
                else if (shootType == ShootTypes.SemiAuto)
                {
                    spawnShell();
                    recoilAuto = false;
                    recoilSemi = true;

                    if (Weapon == WeaponTypes.Rifle)
                    {
                        Invoke("fireCancel", .25f);
                    }
                    Invoke("cycleFire", cycleTimeSemiAuto);
                    cycling = true;
                }
                else if (shootType == ShootTypes.BoltAction)
                {
                    recoilAuto = false;
                    recoilSemi = true;

                    if (Weapon == WeaponTypes.Rifle)
                    {
                        Invoke("fireCancel", .25f);
                        Invoke("cycleFire", cycleTimeBoltAction);
                        Invoke("ejectShellBoltAction", cycleTimeBoltAction / 2);
                        cycling = true;
                        gameObject.GetComponent<Animator>().SetBool("cycle", true);
                    }
                }
                if (!aiming)
                {
                    anim.SetBool("aim", true);
                    orot.enabled = true;
                }
                orientPlayer();

                //Alert nearby cops when shooting
                PoliceManager.instance.CheckAlertAllNearbyCops();
                NPCManager.instance.CheckAlert(transform);
            }

            if (!firePressed || bulletsInMag == 0)
            {
                firing = false;
                recoilSemi = false;
                recoilAuto = false;
                if (shootType == ShootTypes.FullAuto && coroutineRunning)
                {
                    StopCoroutine(lastRoutine);
                    coroutineRunning = false;
                }
                if (!aiming)
                {
                    anim.SetBool("aim", false);
                    orot.enabled = false;
                }
            }

            /*
            if (Input.GetButtonDown("Grenade"))
            {
                anim.SetTrigger("Grenade");
                throwing = true;
                Invoke("throwingCancel", grenadeTime);
            }
            */

            if (reloadPressed && !reloading && !firing && bulletsInMag < bulletsPerMag && totalBullets > 0)
            {
                anim.SetBool("reload", true);
                reloading = true;
                gameObject.GetComponent<AudioSource>().PlayOneShot(reloadSound);
                Invoke("reloadFinished", reloadTime);
                spawnMag();
            }

            //Anims
            anim.SetBool("Fire", firing);
        }

        void cycleFire()
        {
            cycling = false;

            if (shootType == ShootTypes.BoltAction)
                gameObject.GetComponent<Animator>().SetBool("cycle", false);
        }

        void ejectShellBoltAction()
        {
            spawnShell();
        }

        void fireCancel()
        {
            firing = false;
        }

        void LateUpdate()
        {
            if (aimPressed && aimFinished && !swapping)
            {
                aimIn();
            }
            else if (!aimPressed && aiming && !swapping)
            {
                aimOut();
            }

            if (aiming && !aimFinished)
            {
                LerpAimIn();
    
                orot.lookTarget = lookTarget.transform;
            }
            else if (!aiming && !aimFinished)
            {
                LerpAimOut();
            }

            if (firing && !aiming)
            {
                orot.lookTarget = lookTarget.transform;
            }
        }

        void orientPlayer()
        {
            if (!transform.root.gameObject.GetComponent<ThirdPersonBase>().moving)
            {
                Vector3 targetPostition = new Vector3(lookTarget.position.x,
                transform.root.position.y,
                lookTarget.position.z);
                transform.root.LookAt(targetPostition);
            }
        }
        void aimIn()
        {
            originalAimOffsetCamPos = aimPosition;
            aimPosition += originalCamPos;
            holdingHandOffsetRot += originalAimRot;
            aimTimeElapsed = 0;
            aiming = true;
            aimFinished = false;
            anim.SetBool("aim", true);
            orot.enabled = true;
            orientPlayer();
        }

        void aimOut()
        {
            aiming = false;
            aimTimeElapsed = 0;
            Invoke("aimingOutFinished", aimInOutDuration);
            anim.SetBool("aim", false);
            orot.enabled = false;
        }

        void aimingOutFinished()
        {
            mainCam.GetComponent<Camera>().fieldOfView = originalCamFov;
            mainCam.transform.localPosition = originalCamPos;
            aimPosition = originalAimOffsetCamPos;
            aimFinished = true;
        }

        void LerpAimIn()
        {
            if (aimTimeElapsed < aimInOutDuration)
            {
                mainCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(originalCamFov, zoomInAmount, aimTimeElapsed / aimInOutDuration);
                mainCam.transform.localPosition = Vector3.Lerp(originalCamPos, aimPosition, aimTimeElapsed / aimInOutDuration);
                aimTimeElapsed += Time.deltaTime;
            }
            else
            {
                mainCam.GetComponent<Camera>().nearClipPlane = 0.01f;
                mainCam.GetComponent<Camera>().fieldOfView = zoomInAmount;
                mainCam.transform.localPosition = new Vector3(aimPosition.x, aimPosition.y, aimPosition.z);
            }
        }
        void LerpAimOut()
        {
            if (aimTimeElapsed < aimInOutDuration)
            {
                mainCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(zoomInAmount, originalCamFov, aimTimeElapsed / aimInOutDuration);
                mainCam.transform.localPosition = Vector3.Lerp(aimPosition, originalCamPos, aimTimeElapsed / aimInOutDuration);
                aimTimeElapsed += Time.deltaTime;
            }
            else
            {
                mainCam.GetComponent<Camera>().fieldOfView = originalCamFov;
                mainCam.transform.localPosition = originalCamPos;
            }
        }

        IEnumerator shootBullet()
        {
            while (true)
            {
                coroutineRunning = true;
                yield return new WaitForSeconds(fireRate);
                if (bulletsInMag > 0)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
                    foreach (ParticleSystem ps in muzzleFlashes)
                    {
                        ps.Play();
                    }
                    spawnBullet();
                    spawnShell();
                    bulletsInMag--;
                }
            }
        }

        void reloadFinished()
        {
            reloading = false;
            anim.SetBool("reload", false);
            int bulletsToRemove = (bulletsPerMag - bulletsInMag);
            if (totalBullets >= bulletsPerMag)
            {
                bulletsInMag = bulletsPerMag;
                totalBullets -= bulletsToRemove;
            }
            else if (bulletsToRemove <= totalBullets)
            {
                bulletsInMag += bulletsToRemove;
                totalBullets -= bulletsToRemove;
            }
            else
            {
                bulletsInMag += totalBullets;
                totalBullets -= totalBullets;
            }
        }

        void throwingCancel()
        {
            throwing = false;
        }

        void spawnBullet()
        {
            GameObject tempBullet;

            //Spawn bullet from the shoot point position, the true tip of the gun
            tempBullet = Instantiate(Bullet, mainCam.transform.GetChild(0).transform.position, mainCam.transform.GetChild(0).transform.rotation) as GameObject;
            tempBullet.GetComponent<RegisterHit>().damage = Damage;

            //Orient it
            tempBullet.transform.Rotate(Vector3.left * 90);

            //Add forward force based on where camera is pointing
            Rigidbody tempRigidBody;
            tempRigidBody = tempBullet.GetComponent<Rigidbody>();
            tempRigidBody.AddForce(mainCam.transform.GetChild(0).transform.forward * bulletVelocity);

            //Destroy after time
            Destroy(tempBullet, bulletDespawnTime);
        }

        void spawnShell()
        {
            if (transform.root.GetComponent<ThirdPersonControl>().state != "InCar")
            {
                //Spawn bullet
                GameObject tempShell;
                tempShell = Instantiate(Shell, ejectionPoint.transform.position, ejectionPoint.transform.rotation) as GameObject;

                //Orient it
                tempShell.transform.Rotate(Vector3.left * 90);

                //Add forward force based on where ejection point is pointing (blue axis)
                Rigidbody tempRigidBody;
                tempRigidBody = tempShell.GetComponent<Rigidbody>();
                tempRigidBody.AddForce(ejectionPoint.transform.forward * shellVelocity);

                //Destroy after time
                Destroy(tempShell, shellDespawnTime);
            }
        }

        void spawnMag()
        {
            if (transform.root.GetComponent<ThirdPersonControl>().state != "InCar")
            {
                //Spawn bullet
                GameObject tempMag;
                tempMag = Instantiate(Mag, magDropPoint.transform.position, magDropPoint.transform.rotation) as GameObject;

                //Orient it
                tempMag.transform.Rotate(Vector3.left * 90);

                //Add forward force based on where ejection point is pointing (blue axis)
                Rigidbody tempRigidBody;
                tempRigidBody = tempMag.GetComponent<Rigidbody>();
                tempRigidBody.AddForce(magDropPoint.transform.forward * magVelocity);

                //Destroy after time
                Destroy(tempMag, magDespawnTime);
            }
        }
    }
}
