using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{

    Animator anim;

    public LayerMask layerMask; // Select all layers that foot placement applies to.
    public Transform leftFootIK;
    public Transform rightFootIK;
    public Transform leftFoot;
    public Transform rightFoot;
    Transform origRight;
    Transform origLeft;

    [Range(0, 1f)]
    public float DistanceToGround; // Distance from where the foot transform is to the lowest possible position of the foot.

    void Start()
    {
        origLeft = leftFootIK;
        origRight = rightFootIK;
        anim = GetComponent<Animator>();
    }
    private void LateUpdate()
    {      
        // Left Foot
        RaycastHit hit;
        // We cast our ray from above the foot in case the current terrain/floor is above the foot position.
        Ray ray = new Ray(leftFoot.position + Vector3.up, Vector3.down);
        Debug.DrawRay(leftFoot.position + Vector3.up, Vector3.down, Color.green);
        if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
        {
            // We're only concerned with objects that are tagged as "Walkable"
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
                footPosition.y += DistanceToGround; // ... taking account the distance to the ground we added above.
                leftFootIK.position = footPosition;
                leftFootIK.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            }
        }

        // Right Foot
        ray = new Ray(rightFoot.position + Vector3.up, Vector3.down);
        Debug.DrawRay(rightFoot.position + Vector3.up, Vector3.down, Color.green);
        if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
        {

            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point;
                footPosition.y += DistanceToGround;
                rightFootIK.position = footPosition;
                rightFootIK.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            }

        }  
    }
}