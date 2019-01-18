using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Conditions/Monitor Vaulting")]
	public class MonitorVaulting : Condition {
		//checks if I can vault over an obstacle
		public float origin1Offset = 1;
		//sets the distance for the ray that start from waist height 		
		public float waistRayDistance = 1;
		//sets the distance for the ray that start from shoulder height 	
		public float shoulderRayDistance = 1;
		public float origin2Offset = 0.2f;
		//sets the distance for the ray that start from the end of the shoulder ray checking ground elevation
		public float elevationRayDistance =1.5f;
		public float secondElevationRayDistance = 4;
		public float vaultOffsetPosition = 2;
		public VaultAnim[] vaultAnims;
		public override bool CheckCondition(StateManager state)
		{
			bool result = false;
			RaycastHit hit;
			//origin of the ray
			Vector3 origin = state.mTransform.position;
			//waist height
			origin.y += origin1Offset;
			Vector3 direction = state.mTransform.forward;
			
			Debug.DrawRay(origin, direction* waistRayDistance);
			//if I have hit an obstacle in front of me AROUND waist height
			if(Physics.Raycast(origin, direction, out hit, waistRayDistance))
			{
				//shoulder height
				Vector3 origin2 = origin;
				origin2.y += origin2Offset;

				Vector3 firstHit= hit.point;
				firstHit.y -= origin1Offset;
				Vector3 normalDirection = -hit.normal;
				Debug.DrawRay(origin2, direction* waistRayDistance);
				if(Physics.Raycast(origin2, direction, out hit, shoulderRayDistance))
				{
					//we cant vault over this
				}
				else{
					//draws a raycast down from the height of origin 2 to check if we have hit the ground.

					//based on elevation of the ground? vault over : step up 
					Vector3 origin3 = origin2 + (direction * shoulderRayDistance);
					Debug.DrawRay(origin3, -Vector3.up * elevationRayDistance);
					if(Physics.Raycast(origin3, -Vector3.up, out hit, elevationRayDistance))
					{
						//we hit ground
						result = true;

						VaultAnim v = CheckForVaultingAnim(state.mTransform.position, hit.point, false);
						if(v != null)
						{
							state.anim.SetBool(state.hashes.isInteracting, true);
							state.anim.CrossFade(v.animName, 0.2f);
							state.vaultData.animLength = v.clip.length;
							state.vaultData.isInit = false;
							state.isVaulting = true;
							state.vaultData.startPosition = state.mTransform.position;
							Vector3 endPos = firstHit;
							endPos += normalDirection *  vaultOffsetPosition;
							//endPos.y = hit.point.y;
							state.vaultData.endingPosition = endPos;
							result = true;
						}						
					}
				}
			}

			if(!result)
			{
				Vector3 origin4 = origin + direction;
				Debug.DrawRay(origin4, -Vector3.up * secondElevationRayDistance);
				if(Physics.Raycast(origin4, -Vector3.up,  out hit, secondElevationRayDistance))
				{
					VaultAnim v = CheckForVaultingAnim(state.mTransform.position, hit.point, true);
						if(v != null)
						{
							state.anim.SetBool(state.hashes.isInteracting, true);
							state.anim.CrossFade(v.animName, 0.2f);
							state.vaultData.animLength = v.clip.length;
							state.vaultData.isInit = false;
							state.isVaulting = true;
							state.vaultData.startPosition = state.mTransform.position;
							Vector3 endPos = hit.point;
							state.vaultData.endingPosition = endPos;
							result = true;
						}
				}
			}
			return result;
		}
		public VaultAnim CheckForVaultingAnim(Vector3 origin, Vector3 hitPoint, bool isDown)
		{
			VaultAnim result = null;

			float diff = hitPoint.y - origin.y;

			for (int i = 0; i < vaultAnims.Length; i++)
			{
				if(isDown && !vaultAnims[i].isDown)
					continue;

				if(Mathf.Abs(vaultAnims[i].min) < diff ||
				Mathf.Abs(vaultAnims[i].max) > diff)
				{
					result = vaultAnims[i];
				}
			}
			return result;
		}
	
	}
}

