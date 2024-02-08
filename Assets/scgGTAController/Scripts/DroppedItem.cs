using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public enum ItemType
    {
        Money
    }

    public ItemType itemType;
    public int amount;
    public AudioClip pickupSound;
    public bool rotate;
    public float rotationAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MoneyManager moneyManager = other.GetComponent<MoneyManager>();

            if (moneyManager != null)
            {
                // Check the item type and perform corresponding action
                switch (itemType)
                {
                    case ItemType.Money:
                        moneyManager.ChangeMoney(amount);
                        break;
                        // Add more cases for other item types if needed
                }

                // Play pickup sound
                PlayPickupSound();

                // Destroy the DroppedItem object after processing
                Destroy(gameObject);
            }
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            // Create a temporary GameObject to play the sound
            GameObject soundObject = new GameObject("PickupSoundObject");
            soundObject.transform.position = transform.position;

            // Add an AudioSource component to the temporary GameObject
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();

            // Set AudioSource properties
            audioSource.clip = pickupSound;
            audioSource.volume = 0.25f;  // Adjust the volume as needed
            audioSource.spatialBlend = 1f;  // Set to 1 for 3D sound
            audioSource.dopplerLevel = 0f;  // Set Doppler level to 0 to disable Doppler effect
            audioSource.minDistance = 1f;   // Set the minimum distance for the rolloff
            audioSource.maxDistance = 15f;  // Set the maximum distance for the rolloff
            audioSource.rolloffMode = AudioRolloffMode.Linear;  // Set the rolloff mode to linear
            audioSource.Play();

            // Destroy the temporary GameObject after the sound finishes playing
            Destroy(soundObject, pickupSound.length);
        }
    }

    private void Update()
    {
        if (rotate)
        {
            transform.Rotate(0, rotationAmount, 0);
        }
    }
}
