using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Actions/State Actions/Movement Forward")]
	public class MovementForward : StateActions {
		public float movementSpeed = 2;
		public override void Execute(StateManager states)
		{
			//Moves the Character Forward based on input
			
			//IF the player is moving
			if(states.movementVariables.moveAmount > 0.1f)
			{
				states.rigidbody.drag = 0;
			}
			else{
				//set drag to 4 so the controller does not slide on slopes or when stationary.
				states.rigidbody.drag = 4;
			}
			//calculate the target velocity for the character to move forward based on the movement speed and assigns it to the rigidbody
			Vector3 targetVelocity = states.mTransform.forward * states.movementVariables.moveAmount * movementSpeed;
			
			targetVelocity.y = states.rigidbody.velocity.y;
			states.rigidbody.velocity = targetVelocity;
		}
	}

}
