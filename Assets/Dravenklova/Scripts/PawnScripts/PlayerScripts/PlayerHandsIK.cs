using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class PlayerHandsIK : MonoBehaviour
{
    [SerializeField]
    private Animator m_HandsAnimator;
    public Animator HandsAnimator
    {
        get { return m_HandsAnimator; }
    }
    [SerializeField]
    private Transform m_LeftHandTarget;
    public Transform LeftHandTarget
    {
        get { return m_LeftHandTarget; }
    }
    [SerializeField]
    private Transform m_RightHandTarget;
    public Transform RightHandTarget
    {
        get { return m_RightHandTarget; }
    }
    [SerializeField]
    private bool m_IKActive;
    public bool IKActive
    {
        get { return m_IKActive; }
        set { m_IKActive = value; }
    }

    void OnAnimatorIK()
    {
        if (HandsAnimator != null)
        {
            if (IKActive)
            {
                if (LeftHandTarget != null)
                {
                    HandsAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    HandsAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

                    HandsAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.transform.position);
                    HandsAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.transform.rotation);
                }
                if (RightHandTarget != null)
                {
                    HandsAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    HandsAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

                    HandsAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.transform.position);
                    HandsAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.transform.rotation);
                }
            }

            else
            {
                HandsAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                HandsAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);

                HandsAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                HandsAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }
}
