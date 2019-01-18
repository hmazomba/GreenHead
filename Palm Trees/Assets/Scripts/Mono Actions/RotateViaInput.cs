using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SO;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/Mono Actions/Rotate Via Input")]
    public class RotateViaInput : Action
    {
        //rotate camera based on input axis;
        public FloatVariable targetFloat;
        public TransformVariable targetTransform;
        public float angle;
        public float speed = 9;
        public bool negative;
        public bool clamp;
        public float minClamp = -35;
        public float maxClamp = 35;
        public RotateAxis targetAxis;
        public override void Execute()
        {
            if(!negative)
                angle += targetFloat.value * speed;
            else
                angle -= targetFloat.value * speed;  

            if(clamp)
            {
                angle = Mathf.Clamp(angle, minClamp, maxClamp);

            }   

            switch(targetAxis)
            {
                case RotateAxis.x:
                    targetTransform.value.localRotation = Quaternion.Euler(angle, 0, 0);
                    break;
                case RotateAxis.y:
                    targetTransform.value.localRotation = Quaternion.Euler(0, angle, 0);
                    break;
                case RotateAxis.z:
                    targetTransform.value.localRotation = Quaternion.Euler(0, 0, angle);
                    break;  
                default: 
                    break;          
            }
        }

        public enum RotateAxis
        {
            x,y,z
        }
    }
}