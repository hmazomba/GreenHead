using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    //adjusts movement velocity to the slope in which a character is moving
    [CreateAssetMenu(menuName = "Actions/State Actions/Movement Forward With AngleSlope")]
    public class MovementForwardWithAngle : StateActions
    {
        public float frontRayOffset =.5f;
        public float movementSpeed = 2;
        public float lerpAdaptSpeed = 10;
        public float groundColliderHeight = 2.0f;
        
        public override void Execute(StateManager states)
		{
			//Moves the Character Forward based on input		
            //this is the base Y we have when we are on the ground
            float frontY = 0;
            RaycastHit hit;
            Vector3 origin = states.mTransform.position + (states.mTransform.forward * frontRayOffset);
            origin.y += .5f;
            Debug.DrawRay(origin, -Vector3.up, Color.red);
            if(Physics.Raycast(origin, -Vector3.up, out hit, 2, Layers.ignoreLayersController))
            {
                float y = hit.point.y;
                frontY = y - states.mTransform.position.y;
            }
            Vector3 currentVelocity = states.rigidbody.velocity;
			//calculate the target velocity for the character to move forward based on the movement speed and assigns it to the rigidbody
			Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed;
			
            if(states.isGrounded)
            {
                float moveAmount = states.movementVariables.moveAmount;
                //IF the player is moving
                if(moveAmount > 0.1f)
                {
                    states.rigidbody.isKinematic = false;
                    states.rigidbody.drag = 0;
                    //if there's a slope
                    if(Mathf.Abs(frontY) > 0.02f)
                    {
                        //then apply velocity on the Y based on whether we are going up or down the slope
                        //targetVelocity.y = ((frontY > 0) ? 1 : -1) * movementSpeed;
                        targetVelocity.y = ((frontY > 0) ? frontY + 0.2f : frontY) * movementSpeed;
                    }
                    //states.collider.height = groundColliderHeight;
                }
                else{

                    float frontYAbs = Mathf.Abs(frontY);
                    if(frontYAbs > 0.02f)
                    {
                        states.rigidbody.isKinematic = false;
                        targetVelocity.y = 0;
                        states.rigidbody.drag = 4;
                    }             
                    
                }
            }
            else{
                //states.collider.height = groundColliderHeight;
                states.rigidbody.isKinematic = false;
                states.rigidbody.drag = 0;
                targetVelocity.y = states.rigidbody.velocity.y;
            }
			Debug.DrawRay((states.mTransform.position + Vector3.up * .2f),targetVelocity, Color.green, 0.01f, false); 
			states.rigidbody.velocity = Vector3.Lerp(currentVelocity, targetVelocity, states.delta * lerpAdaptSpeed);
        }
    }
}