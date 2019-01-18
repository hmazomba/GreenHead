using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Actions/State Actions/Is Grounded Non Placing")]
	public class IsGroundedNonPlacing : StateActions {
		public float groundedDis = .8f;
		public float onAirDis = 1f;
		public override void Execute(StateManager states)
		{
			Vector3 origin = states.mTransform.position;
			origin.y += .7f;
			Vector3 dir = -Vector3.up;
			float dis = groundedDis;
			if(!states.isGrounded)
				dis = onAirDis;

			RaycastHit hit;
			Debug.DrawRay(origin, dir * dis);
			if(Physics.SphereCast(origin, .3f, dir, out hit, dis, Layers.ignoreLayersController))
			{
				states.isGrounded = true;
			}
			else
			{
				states.isGrounded = false;
			}	
		}
	}
}


