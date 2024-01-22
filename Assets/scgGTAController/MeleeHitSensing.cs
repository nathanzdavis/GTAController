//SlapChickenGames
//2024
//Kick leg sensing 

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace scgGTAController
{
    public class MeleeHitSensing : MonoBehaviour
    {
        public float playerHitforce;
        public float objectHitforce;
        public AudioClip hitSound;
        public int hitDamage;
        public bool car;
        public GameObject targetPlayer;

        void OnTriggerEnter(Collider col)
        {
            if (car)
            {
                if (transform.root.GetComponent<Rigidbody>().velocity.magnitude > 10f)
                {
                    if (col.tag == "NPC")
                        col.GetComponent<Rigidbody>().isKinematic = false;

                    Damage(col.gameObject);
                }
                else
                {
                    if (col.tag == "NPC")
                        col.GetComponent<AIController>().AvoidPlayer();
                }
            }
            else if ((col.tag == "NPC" || col.tag == "Player") && col != transform.root.GetComponent<Collider>())
            {
                targetPlayer = col.gameObject;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if ((col.tag == "NPC" || col.tag == "Player") && col != transform.root.GetComponent<Collider>())
            {
                targetPlayer = null;
            }
        }

        public void Damage(GameObject target)
        {
            //If we hit a player, apply damage to the player transform root object's health controller
            if ((target.transform.tag == "NPC" || target.transform.tag == "Player") && target.transform.root.GetComponent<HealthController>())
            {
                target.transform.root.GetComponent<HealthController>().DamageByMelee(transform.forward * 360, playerHitforce, hitDamage);
                gameObject.GetComponent<AudioSource>().PlayOneShot(hitSound);
            }
        }
    }
}
