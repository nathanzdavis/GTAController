//SlapChickenGames
//2024
//Simple foot logic for detecting movement and adding sound

using UnityEngine;
using System.Collections;
using KovSoft.RagdollTemplate.Scripts.Charachter;

public class SimpleFootsteps : MonoBehaviour
{
    // IMPORTANT, this script needs to be on the root transform

    public AudioClip[] soundGrass;
    public AudioClip[] soundWater;
    public AudioClip[] soundMetal;
    public AudioClip[] soundConcrete;
    public AudioClip[] soundGravel;
    public AudioSource audioSource;
    string floortag;
    public bool isAi;
    private float footstepTimer;
    public float footstepInterval = 0.5f; // Adjust this value to control the footstep interval
    
    private ThirdPersonControl playerController;
    private Vector3 lastPosition;

    private void Start()
    {
        playerController = GetComponent<ThirdPersonControl>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (playerController.state != "InCar")
        {
            // Calculate the movement distance since the last frame
            float movement = Vector3.Distance(transform.position, lastPosition);

            // Update the timer based on movement
            footstepTimer += movement;

            // Check if enough time has passed since the last footstep
            if (footstepTimer >= footstepInterval)
            {
                PlayFootStepSound();
                // Reset the timer
                footstepTimer = 0f;
            }

            // Update the last position
            lastPosition = transform.position;
        }
    }

    public void PlayFootStepSound()
    {
        if (!isAi)
        {
            if (gameObject.GetComponent<ThirdPersonRigid>()._groundChecker)
            {
                if (floortag == "grass")
                {
                    audioSource.clip = soundGrass[Random.Range(0, soundGrass.Length)];
                }
                else if (floortag == "gravel")
                {
                    audioSource.clip = soundGravel[Random.Range(0, soundGravel.Length)];
                }
                else if (floortag == "water")
                {
                    audioSource.clip = soundWater[Random.Range(0, soundWater.Length)];
                }
                else if (floortag == "metal")
                {
                    audioSource.clip = soundMetal[Random.Range(0, soundMetal.Length)];
                }
                else if (floortag == "concrete")
                {
                    audioSource.clip = soundConcrete[Random.Range(0, soundConcrete.Length)];
                }

                if (audioSource.clip != null)
                    audioSource.PlayOneShot(audioSource.clip);
            }
        }
        else
        {
            if (floortag == "grass")
            {
                audioSource.clip = soundGrass[Random.Range(0, soundGrass.Length)];
            }
            else if (floortag == "gravel")
            {
                audioSource.clip = soundGravel[Random.Range(0, soundGravel.Length)];
            }
            else if (floortag == "water")
            {
                audioSource.clip = soundWater[Random.Range(0, soundWater.Length)];
            }
            else if (floortag == "metal")
            {
                audioSource.clip = soundMetal[Random.Range(0, soundMetal.Length)];
            }
            else if (floortag == "concrete")
            {
                audioSource.clip = soundConcrete[Random.Range(0, soundConcrete.Length)];
            }

            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "grass")
        {
            floortag = "grass";
        }
        else if (col.transform.tag == "metal")
        {
            floortag = "metal";
        }
        else if (col.transform.tag == "gravel")
        {
            floortag = "gravel";
        }
        else if (col.transform.tag == "water")
        {
            floortag = "water";
        }
        else if (col.transform.tag == "concrete")
        {
            floortag = "concrete";
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "grass")
        {
            floortag = "grass";
        }
        else if (col.transform.tag == "metal")
        {
            floortag = "metal";
        }
        else if (col.transform.tag == "gravel")
        {
            floortag = "gravel";
        }
        else if (col.transform.tag == "water")
        {
            floortag = "water";
        }
        else if (col.transform.tag == "concrete")
        {
            floortag = "concrete";
        }
    }
}


