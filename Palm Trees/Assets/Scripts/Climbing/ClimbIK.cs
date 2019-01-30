using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Climbing
{
    public class ClimbIK : MonoBehaviour
    {
        Animator anim;
        Point leftHandPoint;
        Point leftFootPoint;
        Point rightHandPoint;
        Point rightFootPoint;

        public float leftHandWeight = 1;
        public float leftFootWeight = 1;        
        public float rightHandWeight = 1;
        public float rightFootWeight = 1;

        Transform leftHandHelper;
        Transform leftFootHelper;
        Transform rightHandHelper;
        Transform rightFootHelper;

        Vector3 leftHandTargetPosition;
        Vector3 rightHandTargetPosition;
        Vector3 leftFootTargetPosition;
        Vector3 rightFootTargetPosition;

        public float helperSpeed = 15;

        [HideInInspector]
        public bool useCurve;
        Transform hips;
        public bool forceFeetHeight;

        void Start(){
            anim = GetComponent<Animator>();
            hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            leftHandHelper = new GameObject().transform;
            leftHandHelper.name = "Left Hand IK Helper";
            leftFootHelper = new GameObject().transform;
            leftFootHelper.name = "Left Foot IK Helper";
            rightFootHelper = new GameObject().transform;
            rightFootHelper.name = "Right Foot IK Helper";
            rightHandHelper = new GameObject().transform;
            rightHandHelper.name = "Right Hand IK Helper";
        }
        public void UpdateAllPointsOnOne(Point targetPoint)
        {
            leftHandPoint = targetPoint;
            leftFootPoint= targetPoint;
            rightHandPoint = targetPoint;
            rightFootPoint = targetPoint;
        }

        public void UpdatePoint(AvatarIKGoal ik, Point targetPoint)
        {
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    leftFootPoint = targetPoint;
                    break;
                case AvatarIKGoal.RightFoot:
                    rightFootPoint = targetPoint;
                    break;
                case AvatarIKGoal.RightHand:
                    rightHandPoint = targetPoint;
                    break;
                case AvatarIKGoal.LeftHand:
                    leftHandPoint = targetPoint;
                    break;            
            }
        }

        public void UpdateAllTargetPositions(Point p)
        {
            IKPositions leftHandHolder = p.ReturnIK(AvatarIKGoal.LeftHand);
            if(leftHandHolder.target)
                leftHandTargetPosition = leftHandHolder.target.position;

            IKPositions rightHandHolder = p.ReturnIK(AvatarIKGoal.RightHand);
            if(rightHandHolder.target)
                rightHandTargetPosition = rightHandHolder.target.position;

            IKPositions leftFootHolder = p.ReturnIK(AvatarIKGoal.LeftFoot);
            if(leftFootHolder.target)
                leftFootTargetPosition = leftFootHolder.target.position;

            IKPositions rightFootHolder = p.ReturnIK(AvatarIKGoal.RightFoot);
            if(rightFootHolder.target)
                rightFootTargetPosition = rightFootHolder.target.position;            
        }

        public void UpdateTargetPosition(AvatarIKGoal ik, Vector3 targetPosition)
        {
            switch(ik)
            {
                case AvatarIKGoal.LeftFoot:
                    leftFootTargetPosition = targetPosition;
                    break;

                case AvatarIKGoal.RightFoot:
                    rightFootTargetPosition = targetPosition;
                    break;

                case AvatarIKGoal.LeftHand:
                    leftHandTargetPosition = targetPosition;
                    break;

                case AvatarIKGoal.RightHand:
                    rightHandTargetPosition = targetPosition;
                    break;    
                default: 
                    break;    

            }
        }


        public Vector3 ReturnCurrentPosition(AvatarIKGoal ik)
        {
            Vector3 retVal = default(Vector3);
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    IKPositions leftFootHolder = leftFootPoint.ReturnIK(AvatarIKGoal.LeftFoot);
                    retVal = leftFootHolder.target.transform.position;
                    break;
                case AvatarIKGoal.LeftHand:
                    IKPositions leftHandHolder = leftHandPoint.ReturnIK(AvatarIKGoal.LeftHand);
                    retVal = leftHandHolder.target.transform.position;
                    break;
                case AvatarIKGoal.RightHand:
                    IKPositions rightHandHolder = rightHandPoint.ReturnIK(AvatarIKGoal.RightHand);
                    retVal = rightHandHolder.target.transform.position;
                    break;
                case AvatarIKGoal.RightFoot:
                    IKPositions rightFootHolder = rightFootPoint.ReturnIK(AvatarIKGoal.RightFoot);
                    retVal = rightFootHolder.target.transform.position;
                    break;        
                default:
                    break;
            }
            return retVal;
        }

        public Point ReturnPointForIK(AvatarIKGoal ik)
        {
            Point retVal = null;
            switch(ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = leftFootPoint;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = leftHandPoint;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = rightHandPoint;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = rightFootPoint;
                    break;        

            }

            return retVal;
        }

        public AvatarIKGoal ReturnOppositeIK(AvatarIKGoal ik)
        {
            AvatarIKGoal retVal = default(AvatarIKGoal);
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = AvatarIKGoal.RightFoot;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = AvatarIKGoal.RightHand;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = AvatarIKGoal.LeftHand;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = AvatarIKGoal.LeftFoot;
                    break;            
                default:
                    break;
            }
            return retVal;
        }

        public AvatarIKGoal ReturnOppositeLimb(AvatarIKGoal ik)
        {
            AvatarIKGoal retVal = default(AvatarIKGoal);
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    retVal = AvatarIKGoal.LeftHand;
                    break;
                case AvatarIKGoal.LeftHand:
                    retVal = AvatarIKGoal.LeftFoot;
                    break;
                case AvatarIKGoal.RightHand:
                    retVal = AvatarIKGoal.RightFoot;
                    break;
                case AvatarIKGoal.RightFoot:
                    retVal = AvatarIKGoal.RightHand;
                    break;            
                default:
                    break;
            }
            return retVal;
        }

        public void AddWeightInfluenceAll(float w)
        {
            leftHandWeight = w;
            leftFootWeight = w;
            rightHandWeight = w;
            rightFootWeight = w;
        }

        public void ImmediatePlaceHelpers()
        {
            if(leftHandPoint != null)
            {
                leftHandHelper.position = leftHandTargetPosition;
            }
            if(leftFootPoint != null)
            {
                leftFootHelper.position = leftFootTargetPosition;
            }
            if(rightFootPoint != null)
            {
                rightFootHelper.position = rightFootTargetPosition;
            }
            if(rightHandPoint != null)
            {
                rightHandHelper.position = rightHandTargetPosition;
            }
        }

        void OnAnimatorIK()
        {
            if(leftHandPoint)
            {
                IKPositions leftHandHolder = leftHandPoint.ReturnIK(AvatarIKGoal.LeftHand);

                if(leftHandHolder.target)
                {
                    leftHandHelper.transform.position = Vector3.Lerp(leftHandHelper.transform.position, leftHandTargetPosition, Time.deltaTime * helperSpeed); 
                }
                UpdateIK(AvatarIKGoal.LeftHand, leftHandHolder, leftHandHelper,leftHandWeight, AvatarIKHint.LeftElbow);
            }
            if(rightHandPoint)
            {
                IKPositions rightHandHolder = rightHandPoint.ReturnIK(AvatarIKGoal.RightHand);

                if(rightHandHolder.target)
                {
                    rightHandHelper.transform.position = Vector3.Lerp(rightHandHelper.transform.position, rightHandTargetPosition, Time.deltaTime * helperSpeed); 
                }
                UpdateIK(AvatarIKGoal.RightHand, rightHandHolder, rightHandHelper, rightHandWeight, AvatarIKHint.RightElbow);
            }


            if(hips == null)
                hips = anim.GetBoneTransform(HumanBodyBones.Hips);

            if(leftFootPoint)
            {
                IKPositions leftFootHolder = leftFootPoint.ReturnIK(AvatarIKGoal.LeftFoot);

                if(leftFootHolder.target)
                {
                    Vector3 targetPosition = leftFootTargetPosition;

                    if(forceFeetHeight)
                    {
                        if(targetPosition.y > hips.transform.position.y)
                        {
                            targetPosition.y = targetPosition.y - 0.2f;
                        }
                    }

                    leftFootHelper.transform.position = Vector3.Lerp(leftFootHelper.transform.position, targetPosition, Time.deltaTime * helperSpeed);
                }
                UpdateIK(AvatarIKGoal.LeftFoot, leftFootHolder, leftFootHelper, leftFootWeight, AvatarIKHint.LeftKnee); 
            }

            if(rightFootPoint)
            {
                IKPositions rightFootHolder = rightFootPoint.ReturnIK(AvatarIKGoal.RightFoot);

                if(rightFootHolder.target)
                {
                    Vector3 targetPosition = rightFootTargetPosition;

                    if(forceFeetHeight)
                    {
                        if(targetPosition.y > hips.transform.position.y)
                        {
                            targetPosition.y = targetPosition.y - 0.2f;
                        }
                    }

                    rightFootHelper.transform.position = Vector3.Lerp(rightFootHelper.transform.position, targetPosition, Time.deltaTime * helperSpeed);
                }
                UpdateIK(AvatarIKGoal.LeftFoot, rightFootHolder, rightFootHelper, rightFootWeight, AvatarIKHint.RightKnee); 
            }    
        }
        public void InfluenceWeight(AvatarIKGoal ik, float targetWeight)
        {
            switch (ik)
            {
                case AvatarIKGoal.LeftFoot:
                    leftFootWeight = targetWeight;
                    break;
                case AvatarIKGoal.LeftHand:
                    leftHandWeight = targetWeight;
                    break;    
                case AvatarIKGoal.RightHand:
                    rightHandWeight = targetWeight;
                    break;
                case AvatarIKGoal.RightFoot:
                    rightFootWeight = targetWeight;
                    break;      
            }
        }
        void UpdateIK(AvatarIKGoal ik, IKPositions holder, Transform helper, float weight, AvatarIKHint hint)
        {
            if(holder != null)
            {
                anim.SetIKPositionWeight(ik, weight);
                anim.SetIKRotationWeight(ik, weight);
                anim.SetIKPosition(ik, helper.position);
                anim.SetIKRotation(ik, helper.rotation);

                if(ik == AvatarIKGoal.LeftHand || ik == AvatarIKGoal.RightHand)
                {
                    Transform shoulder = (ik == AvatarIKGoal.LeftHand) ? anim.GetBoneTransform(HumanBodyBones.LeftShoulder): anim.GetBoneTransform(HumanBodyBones.RightShoulder);

                    Vector3 offset = Vector3.zero;
                    offset+= transform.forward;
                    offset += transform.up * 2.2f;
                    offset += transform.position;

                    Vector3 targetRotationDir = shoulder.transform.position - offset;
                    Quaternion targetRotation = Quaternion.LookRotation(-targetRotationDir);
                    helper.rotation = targetRotation;
                }
                else
                {
                    helper.rotation = holder.target.transform.rotation;
                }
                if(holder.hint != null)
                {
                    anim.SetIKHintPositionWeight(hint, weight);
                    anim.SetIKHintPosition(hint, holder.hint.position);
                }
            }
        }
    }
}