using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMiniMapRotation : MonoBehaviour
{
    public Transform transformToRotateYWith;

    private void Update()
    {
        transform.localEulerAngles = new Vector3(90, 0, -transformToRotateYWith.localEulerAngles.y);
    }
}
