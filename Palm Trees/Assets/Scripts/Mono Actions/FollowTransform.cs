using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;

namespace  SA
{
	[CreateAssetMenu(menuName = "Actions/Mono Actions/Follow Transform")]
	public class FollowTransform : Action {
		//Transform to Follow
		public TransformVariable targetTransform;
		//My Transform
		public TransformVariable currentTransform;
		public FloatVariable delta;
		public float speed = 9;
		public override void Execute(){
			if(targetTransform.value == null)
				return;
			if(currentTransform.value == null)
				return;
			//Calculates the position of target vs the position of the camera through time
			Vector3 targetPosition = Vector3.Lerp(currentTransform.value.position, targetTransform.value.position,
				delta.value * speed);

			currentTransform.value.position = targetPosition;		
		}
	}
}

