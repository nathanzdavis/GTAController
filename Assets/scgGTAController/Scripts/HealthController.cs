﻿//SlapChickenGames
//2024
//Health controller

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

namespace scgGTAController
{
    public class HealthController : MonoBehaviour
    {
        //IMPORTANT, this script needs to be on the root transform

        [Header("Basics")]
        public float health;
        float maxHealth;
        public GameObject ragdoll;
        public bool dontSpawnRagdoll;
        public float deadTime;
        GameObject tempdoll;
        bool meleeDeath;
        public bool isAiOrDummy;

        [Header("Sound")]
        public bool playNoiseOnHurt;
        public float percentageToPlay;
        public AudioClip hurtNoise;

        [Header("Regen")]
        public bool regen;
        public float timeBeforeRegen;
        float origTimeBeforeRegen;
        public float regenSpeed;
        bool alreadyRegenning;

        void Start()
        {
            //Get a reference to the original reset time
            origTimeBeforeRegen = timeBeforeRegen;

            //Set maxHealth to what our max is at start of the scene
            maxHealth = health;
        }

        void Update()
        {
            //If health is low enough and we are not kicked to death, then die normally
            if (health <= 0)
            {
                if (!meleeDeath)
                    Die();
            }

            //Only update HUD text if we are a player
            if (!isAiOrDummy)
            {
                if (health > 0)
                {
                    HudController.instance.uiHealth.value = health / 100;
                }
                else
                {
                    HudController.instance.uiHealth.value = 0;
                }
            }


            //Check if we are done regenning and stop
            if (health == maxHealth && regen && alreadyRegenning)
            {
                alreadyRegenning = false;
                StopCoroutine("regenHealth");
            }

        }

        public void Damage(float damage)
        {
            //If we are a player, take damage, otherwise (AI), apply the hit animation and attack the player
            if (!isAiOrDummy)
            {
                health -= damage;

                if (playNoiseOnHurt)
                {
                    if (Random.value < percentageToPlay)
                    {
                        GetComponent<AudioSource>().PlayOneShot(hurtNoise);
                    }
                }
            }
            else
            {
                health -= damage;
                GetComponent<Animator>().SetTrigger("hit");

                if (GetComponent<AiController>())
                    GetComponent<AiController>().overrideAttack = true;

                if (playNoiseOnHurt)
                {
                    if (Random.value < percentageToPlay)
                    {
                        GetComponent<AudioSource>().PlayOneShot(hurtNoise);
                    }
                }

            }

            //If we are allowed to regen, start gaining health
            if (regen)
            {
                timeBeforeRegen = origTimeBeforeRegen;
                StopCoroutine("regenHealth");
                CancelInvoke();
                if (timeBeforeRegen == origTimeBeforeRegen)
                {
                    alreadyRegenning = true;
                    Invoke(nameof(regenEnumeratorStart), timeBeforeRegen);
                }
            }
        }

        void regenEnumeratorStart()
        {
            StartCoroutine("regenHealth");
        }

        IEnumerator regenHealth()
        {
            //Only regen while under max health and gain 1 health every regenSpeed seconds
            while (health < maxHealth)
            {
                health++;
                yield return new WaitForSeconds(regenSpeed);
            }
        }

        void Die()
        {
            //Only spawn ragdoll if option is selected
            if (!dontSpawnRagdoll)
            {
                //Spawn ragdoll and destroy us
                tempdoll = Instantiate(ragdoll, this.transform.position, this.transform.rotation) as GameObject;


                //Destroy ragdoll if we are an AI after deadTime seconds
                if (isAiOrDummy)
                    Destroy(tempdoll, deadTime);

                if (tag == "Player")
                {
                    Time.timeScale = .35f;

                    // Find the PostProcessVolume component on the object with the "postProcessing" tag
                    PostProcessVolume postProcessVolume = GameObject.FindGameObjectWithTag("postProcessing").GetComponent<PostProcessVolume>();

                    // Create a settings container for the ColorGrading effect
                    ColorGrading colorGrading;

                    // Check if the volume has the ColorGrading effect
                    if (postProcessVolume.profile.TryGetSettings(out colorGrading))
                    {
                        // Set the saturation to 0
                        colorGrading.saturation.Override(-100f);
                    }

                    HudController.instance.deathText.SetActive(true);
                }

                //Tell the ragdoll if we are a player or not so it knows to move our camera or not to the ragdoll
                Destroy(gameObject);

            }
            else if (isAiOrDummy)
            {
                //If we aren't spawning a ragdoll, then disable all important scripts on us and destroy after deadtime seconds
                //This feature is for AI with the ragdoll built in for a more realistic death
                if (GetComponent<Animator>())
                    GetComponent<Animator>().enabled = false;

                if (GetComponent<AiController>())
                    GetComponent<AiController>().enabled = false;

                if (GetComponent<HealthController>())
                    GetComponent<HealthController>().enabled = false;

                if (GetComponent<SimpleFootsteps>())
                    GetComponent<SimpleFootsteps>().enabled = false;

                if (GetComponent<NavMeshAgent>())
                    GetComponent<NavMeshAgent>().enabled = false;

                if (GetComponentInChildren<Adjuster>())
                    GetComponentInChildren<Adjuster>().enabled = false;

                if (GetComponent<PoliceAIBehavior>())
                {
                    PoliceManager.instance.policeNPCs.Remove(gameObject);
                }

                Destroy(gameObject, deadTime);
            }
            else
            {
                //Die
            }
        }
      
        public void DamageByMelee(Vector3 pos, float Force, int Damage)
        {
            health -= Damage;

            //If kicked enough, then die
            if (health <= 0)
            {
                meleeDeath = true;
                tempdoll = Instantiate(ragdoll, this.transform.position, this.transform.rotation) as GameObject;
                Destroy(gameObject);

                foreach (Rigidbody rb in tempdoll.GetComponentsInChildren<Rigidbody>())
                {
                    rb.AddForce(pos * Force);
                }
            }
            else
            {
                //Dont die just play hit anim
                GetComponent<Animator>().SetTrigger("hit");
            }
        }
    }
}