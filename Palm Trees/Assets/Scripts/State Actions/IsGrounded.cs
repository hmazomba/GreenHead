using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Is Grounded")]
    public class IsGrounded : StateActions
    {
        
        public override void Execute(StateManager states)
        {
            //sets the origin point of the ray
            Vector3 origin = states.mTransform.position;
            origin.y += .7f;
            //sets the direction od the ray
            Vector3 dir = -Vector3.up;
            //sets the distance that the maximum ray will travel
            float dis = 1.4f;

            RaycastHit hit;
            //if the ray hits something
            //if(Physics.Raycast(origin, dir, out hit, dis))
            if(Physics.SphereCast(origin, .3f, dir, out hit, dis, Layers.ignoreLayersController))
            {
                states.isGrounded = true;
            }
            else{
                states.isGrounded = false;
            }
            if(states.isGrounded)
            {
                Vector3 targetPosition = states.mTransform.position;
                targetPosition.y = hit.point.y;
                states.mTransform.position = targetPosition;
            }          
            
        }
    }
}