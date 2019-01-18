using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SO;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Rotation Based On Camera Orientation")]
    public class RotateBasedOnCameraOrientation: StateActions
    {
        public TransformVariable cameraTransform;
        public float speed = 8;
        public override void Execute(StateManager states)
        {
            if(cameraTransform.value == null)
                return;

            float h = states.movementVariables.horizontal;
            float v = states.movementVariables.vertical;
            //get target direction via input from user based on the camera's orientation or where it is looking
            Vector3 targetDir = cameraTransform.value.forward * v;
            targetDir += cameraTransform.value.right *h;
            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
                targetDir = states.mTransform.forward;
            //calculate the targetRotation by getting Rotation from Camera Orientaion
            Quaternion tr = Quaternion.LookRotation(targetDir);
            //Rotate character to match the target rotation by spherically lerping from own rotation to target rotation
            Quaternion targetRotation = Quaternion.Slerp(states.mTransform.rotation, tr, 
                states.delta * states.movementVariables.moveAmount * speed);

            states.mTransform.rotation = targetRotation;        
        }
    }
}