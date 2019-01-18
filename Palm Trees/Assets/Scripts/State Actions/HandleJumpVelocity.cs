using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Handle Jump Velocity")]
    public class HandleJumpVelocity : StateActions
    {
        //handles the velocity experienced during a jump i.e. how fast you go up and down
        public float jumpSpeed = 4;
        public override void Execute(StateManager states)
        {
            states.rigidbody.drag = 0;
            Vector3 currentVelocity = states.rigidbody.velocity;
            //stores time since the controller has left the ground
            states.timeSinceJump = Time.realtimeSinceStartup;
            states.isGrounded = false;    
            
            states.anim.CrossFade(states.hashes.jumpForward, 0.2f);
            currentVelocity += jumpSpeed * Vector3.up;
                 
            
            
            states.rigidbody.velocity = currentVelocity;

        }
    }
}