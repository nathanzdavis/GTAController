using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMiniMapRotation : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (player)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, player.localEulerAngles.y + 180);    
    }
}
