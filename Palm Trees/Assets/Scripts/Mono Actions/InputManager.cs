using System.Collections;
using System.Collections.Generic;
using UnityEngine;using SO;
namespace SA {

	//Handles Input from User and updates the movementVariables class
	[CreateAssetMenu(menuName = "Actions/Mono Actions/Input Manager")]
	public class InputManager : Action {
		public FloatVariable horizontal;
		public FloatVariable vertical;
		public BoolVariable jump;
		public StateManagerVariable playerStates;
		public ActionBatch inputUpdateBatch;
		public override void Execute()
		{
			inputUpdateBatch.Execute();
			if(playerStates != null)
			{
				playerStates.value.movementVariables.horizontal = horizontal.value;
				playerStates.value.movementVariables.vertical = vertical.value;
				float moveAmount = Mathf.Clamp01((Mathf.Abs(horizontal.value) + Mathf.Abs(vertical.value)));
				playerStates.value.movementVariables.moveAmount = moveAmount;
				playerStates.value.isJumping = jump.value;
				/* if(jump.value == true)
				{
					playerStates.value.isJumping = true;
				} */
				
			}
		}
	}
}

