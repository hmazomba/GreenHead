using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{	
	[CreateAssetMenu(menuName = "Conditions/Monitor Jump")]
	public class MonitorJump : Condition {

		public StateActions onTrueAction;
		public State waitForAnimation;
		public override bool CheckCondition(StateManager state)
		{
			//checks if jumping inout is received from the state manager
			bool result = state.isJumping;
			if(state.isJumping)
			{
				state.isJumping = false;
				if(state.movementVariables.moveAmount > 0.2f)
				{
					onTrueAction.Execute(state);
				}
				else
				{
					result = false;
					state.anim.SetBool(state.hashes.isInteracting, true);
					state.anim.CrossFade(state.hashes.jumpIdle, 0.2f);
					state.currentState = waitForAnimation;
				}
				
				
			}
			return result;
		}
	}
}
