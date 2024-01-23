using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToObject : MonoBehaviour
{
    public Transform target;
    public bool x = true;
    public bool y = true;
    public bool z = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            Vector3 newPosition = transform.position;

            if (x)
                newPosition.x = target.position.x;

            if (y)
                newPosition.y = target.position.y;

            if (z)
                newPosition.z = target.position.z;

            transform.position = newPosition;
        }
    }
}
