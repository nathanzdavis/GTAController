//SlapChickenGames
//2024
//Bullet hit registration

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgGTAController
{
    public class RegisterHit : MonoBehaviour
    {
        //IMPORTANT, this script must be on root of bullet object

        public GameObject impactParticle;
        public GameObject impactBloodParticle;
        public float impactDespawnTime;
        [HideInInspector] public int damage;

        void OnCollisionEnter(Collision col)
        {
            //If we (the bullet) hit the col object check for Player tag
            if (col.transform.tag == "Player" || col.transform.tag == "NPC")
            {
                //If the root object we hit has a healthcontroller then apply damage
                if (col.transform.root.gameObject.GetComponent<HealthController>())
                {
                    col.transform.root.gameObject.GetComponent<HealthController>().Damage(damage);
                }

                //Spawn blood on player
                GameObject tempImpact;
                tempImpact = Instantiate(impactBloodParticle, this.transform.position, this.transform.rotation) as GameObject;
                tempImpact.transform.Rotate(Vector3.left * 90);
                Destroy(tempImpact, impactDespawnTime);
            }
            else
            {
                //We hit something else just spawn basic impact prefab
                GameObject tempImpact;
                tempImpact = Instantiate(impactParticle, this.transform.position, this.transform.rotation) as GameObject;
                tempImpact.transform.Rotate(Vector3.left * 90);
                Destroy(tempImpact, impactDespawnTime);
            }

            //Finally, destroy us (the bullet)
            Destroy(gameObject);
        }
    }
}
