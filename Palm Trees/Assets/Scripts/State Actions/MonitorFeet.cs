using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Actions/State Actions/Monitor Feet Position")]
	public class MonitorFeet : StateActions {
		public override void Execute(StateManager states)
		{
			//gets the relative position of the right foot and the left foot
			Vector3 rightFootRelative = states.mTransform.InverseTransformPoint(states.animData.rightFoot.position);
			Vector3 leftFootRelative = states.mTransform.InverseTransformPoint(states.animData.leftFoot.position);

			bool leftForward = false;
			//if the left foot is more forward than the right foot
			if(leftFootRelative.z > rightFootRelative.z)
				leftForward = true;

			states.anim.SetBool(states.hashes.leftFootForward, leftForward);	
		}
		
	
	}
}
