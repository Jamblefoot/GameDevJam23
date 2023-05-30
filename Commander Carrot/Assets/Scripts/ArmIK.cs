using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIK : MonoBehaviour
{
    Transform leftHandTarget, rightHandTarget;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void OnAnimatorIK()
    {
        if(rightHandTarget != null)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        }
        else anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        if(leftHandTarget != null)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        }
        else anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
    }

    public void SetHandAnchors(Transform leftHand, Transform rightHand)
    {
        leftHandTarget = leftHand;
        rightHandTarget = rightHand;
    }
}
