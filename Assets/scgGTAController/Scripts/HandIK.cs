using KovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

public class HandIK : MonoBehaviour
{
    [HideInInspector] public Transform targetPoint;  // The target point where you want the hand to reach
    public AvatarIKGoal ikGoal = AvatarIKGoal.RightHand;  // Specify the IK goal (RightHand or LeftHand)

    private Animator animator;  // Reference to the Animator component

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (targetPoint)
        {
            // Check if the Animator component is assigned
            if (animator == null)
            {
                Debug.LogError("Animator component is not assigned!");
                return;
            }

            // Set the IK position and rotation for the specified IK goal during the OnAnimatorIK phase
            // This if else is here because the hand ik looks weird while sprinting with a gun and this fixes it
            if (!GetComponent<ThirdPersonControl>().sprintPressed)
            {
                animator.SetIKPositionWeight(ikGoal, 1f);
            }
            else
            {
                animator.SetIKPositionWeight(ikGoal, .15f);
            }

            animator.SetIKPosition(ikGoal, targetPoint.position);
            animator.SetIKRotation(ikGoal, targetPoint.rotation);
        }
    }
}
