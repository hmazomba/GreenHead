using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;

namespace Climbing {
	public class ClimbBehaviour : MonoBehaviour {
		#region Variables
		public bool climbing;
		bool initClimb;
		bool waitToStartClimb;

		bool dropOnLedge = true;

		//Components
		Animator anim;
		ClimbIK ik;

		//Point Variables
		Manager currentManager;
		Point targetPoint;
		Point currentPoint;
		Point previousPoint;
		Neighbour neighhbour;
		ConnectionType currentConnection;

		//current and targetState
		ClimbStates climbState;
		ClimbStates targetState;

		public enum ClimbStates{
			onPoint,
			betweenPoints,
			inTransit
		}

		#region Curves

		CurvesHolder curvesHolder;
		BezierCurve mountCurve;
		BezierCurve currentCurve;
		#endregion

		//Interpolation
		Vector3 lerpStartPosition;
		Vector3 lerpTargetPosition;
		float lerpDistance;
		float lerpTime;
		bool initTransit;
		bool rootReached;
		bool ikLandSideReached;
		bool ikFollowSideReached;

		//Input Variables
		bool lockInput;
		Vector3 inputDirection;
		Vector3 targetPosition;

		//Tweakable Variables
		//how much the hips are above the ground
		public Vector3 rootOffset = new Vector3(0, -0.86f, 0);
		public float linearSpeed =3;
		public float directSpeed = 2;
		public float ledgeDropSpeed = 1.5f;
		public AnimationCurve jumpingAnimationCurve;
		public AnimationCurve hangingToBrace;
		public AnimationCurve zeroToOne;
		public AnimationCurve mountAnimationCurve;	
		public bool enableRootMovement;
		float rootMotionMax = 0.25f;
		float rootMotionTime;

		StateManager states;

		#endregion
		#region Methods

		void SetCurveReferences()
		{
			GameObject curvesHolderPrefab = Resources.Load("CurvesHolder") as GameObject;
			GameObject curvesHolderGO = Instantiate(curvesHolderPrefab) as GameObject;

			curvesHolder = curvesHolderGO.GetComponent<CurvesHolder>();
		}

		void Start(){
			anim = GetComponentInChildren<Animator>();
			states = GetComponent<Controller.StateManager>();
			ik = GetComponentInChildren<ClimbIK>();
			SetCurveReferences();
		}

		void FixedUpdate()
		{
			if(climbing)
			{
				if(!waitToStartClimb)
				{
					HandleClimbing();
					InitiateFallOff();
				}
				else{
					InitClimbing();
					HandleMount();
				}
			}
			else{
				if(initClimb)
				{
					transform.parent = null;
					initClimb = false;
				}

				if(Input.GetKey(KeyCode.Space))
					LookForClimbSpot();

				CharacterOnEdge();	
			}
		}

        private void LookForClimbSpot()
        {
            Transform cameraTransform = Camera.main.transform;
			Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
			RaycastHit hit;
			LayerMask lm =(1<<gameObject.layer) | (1 << 3);
			lm = ~lm;

			float maxDistance = 20;
			if(Physics.Raycast(ray, out hit, maxDistance, lm))
			{
				if(hit.transform.GetComponentInParent<Manager>())
				{
					Manager tm = hit.transform.GetComponentInParent<Manager>();

					Point closestPoint = tm.ReturnClosest(transform.position);
					float distanceToPoint = Vector3.Distance(transform.position, closestPoint.transform.parent.position);

					if(distanceToPoint < 5)
					{
						currentManager = tm;
						targetPoint = closestPoint;
						targetPosition = closestPoint.transform.position;
						currentPoint = closestPoint;
						climbing = true;
						lockInput = true;
						targetState = ClimbStates.onPoint;
						
						anim.CrossFade("Climb", 0.4f);
						GetComponent<Controller.StateManager>().DisableController();
						waitToStartClimb = true;
					}
				}
			}
        }

		void CharacterOnEdge()
		{
			if(!states.isGroundForward)
			{
				Vector3 origin = transform.position;
				origin += transform.forward;
				origin -= Vector3.up /3;
				Vector3 direction = transform.position - origin;
				direction.y = 0;
				RaycastHit hit;

				LayerMask lm=(1<<gameObject.layer) | (1<<3);
				lm =~lm;

				Debug.DrawRay(origin, direction);

				if(Physics.Raycast(origin, direction, out hit, 1, lm))
				{
					if(hit.transform.GetComponentInParent<Manager>())
					{
						Manager tm = hit.transform.GetComponentInParent<Manager>();

						Point closestPoint = tm.ReturnClosest(transform.position);

						float distanceToPoint = Vector3.Distance(transform.position, closestPoint.transform.parent.position);

						if(distanceToPoint < 5)
						{
							if(Input.GetKey(KeyCode.Space))
							{
								currentManager = tm;
								targetPoint = closestPoint;
								targetPosition = closestPoint.transform.position;
								currentPoint = closestPoint;
								climbing = true;
								lockInput = true;
								dropOnLedge = true;
								GetComponent<Controller.StateManager>().DisableController();
								waitToStartClimb = true;
							}
						}
					}
				}
			}
		}

		void HandleClimbing()
		{
			if(!lockInput)
			{
				inputDirection = Vector3.zero;
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				inputDirection = ConvertToInputDirection(h, v);
				if(inputDirection != Vector3.zero)
				{
					switch(climbState){
						case ClimbStates.onPoint:
							OnPoint(inputDirection);
							break;
						case ClimbStates.betweenPoints:
							BetweenPoints(inputDirection);
							break;
					}
					
				}

				transform.parent = currentPoint.transform.parent;
				if(climbState== ClimbStates.onPoint)
				{
					ik.UpdateAllTargetPositions(currentPoint);
					ik.ImmediatePlaceHelpers();
				}
			}
			else{
				InTransit(inputDirection);
			}
		}

		void OnPoint(Vector3 inputD)
		{
			Neighbour neighbour = null;
			neighhbour = currentManager.ReturnNeighbour(inputD, currentPoint);

			if(neighbour != null)
			{
				targetPoint = neighhbour.target;
				previousPoint = currentPoint;
				climbState = ClimbStates.inTransit;
				UpdateConnectionTransitionByType(neighbour, inputD);

				lockInput = true;
			}
		}

		void BetweenPoints(Vector3 inputD)
		{
			Neighbour n = targetPoint.ReturnNeighbour(previousPoint);
			if(n != null)
			{
				if(inputD == n.direction)
					targetPoint = previousPoint;
			}
			else{
				targetPoint = currentPoint;
			}
			targetPosition = targetPoint.transform.position;
			climbState = ClimbStates.inTransit;
			targetState = ClimbStates.onPoint;
			previousPoint = currentPoint;
			lockInput = true;
			anim.SetBool("Move", false);
		}

		void UpdateConnectionTransitionByType(Neighbour n, Vector3 inputD)
		{	
			Vector3 desiredPosition = Vector3.zero;
			currentConnection = n.connectionType;
			Vector3 direction = targetPoint.transform.position -currentPoint.transform.position;
			direction.Normalize();

			switch(n.connectionType)
			{
				case ConnectionType.inBetween:
					float distance = Vector3.Distance(currentPoint.transform.position, targetPoint.transform.position);
					desiredPosition = currentPoint.transform.position + (direction *(distance / 2));
					targetState = ClimbStates.betweenPoints;
					TransitDir transitDir = ReturnTransitDirection(inputD,false);
					PlayAnim(transitDir);
					break;
				case ConnectionType.direct:					
					desiredPosition = targetPoint.transform.position;
					targetState = ClimbStates.onPoint;
					TransitDir transitDir2 = ReturnTransitDirection(inputD, true);
					PlayAnim(transitDir2, true);
					break;

				case ConnectionType.dismount:
					desiredPosition = targetPoint.transform.position;
					targetState = ClimbStates.betweenPoints;
					anim.SetInteger("JumpType", 20);
					anim.SetBool("Move", true);
					break;

				case ConnectionType.falling:
					climbing = false;
					initTransit = false;
					ik.AddWeightInfluenceAll(0);
					GetComponent<Controller.StateManager>().EnableController();
					anim.SetBool("onAir", true);
					break;			
			}

			switch (targetPoint.pointType)
			{
				case PointType.braced:
					anim.SetFloat("Stance", 0);
					break;
				case PointType.hanging:
					anim.SetFloat("Stance", 1);
					ik.InfluenceWeight(AvatarIKGoal.LeftFoot, 0);
					ik.InfluenceWeight(AvatarIKGoal.RightFoot, 0);
					break;
				default:
					break;
			}
			targetPosition = desiredPosition;
		}

		void InTransit(Vector3 inputD)
		{
			switch (currentConnection)
			{
				case ConnectionType.inBetween:
					UpdateLinearVariables();
					LinearRootMovemnt();
					LerpIKLandingSide_Linear();
					WrapUp();
					break;

				case ConnectionType.direct:
					UpdateDirectVariables(inputDirection);
					DirectRootMovement();
					WrapUp(true);
					break;

				case ConnectionType.dismount:
					HandleDismountVariables();
					DismountRootMovement();
					DismountWrapUp();	
					break;		
			}
		}
		
		#region Mount
		void InitClimbing()
		{
			if(!initClimb)
			{
				initClimb = true;

				if(ik != null)
				{
					ik.UpdateAllPointsOnOne(targetPoint);
					ik.UpdateAllTargetPositions(targetPoint);
					ik.ImmediatePlaceHelpers();					
				}
				//Our Connection Type
				currentConnection = ConnectionType.direct;
					//the state we will be in when our current state ends
				targetState = ClimbStates.onPoint;
				anim.SetBool("Move", false);
				anim.SetInteger("JumpType", 0);
			}
		}
		
		void HandleMount()
		{
			if(!initTransit)
			{
				initTransit = true;
				ikFollowSideReached = false;
				ikLandSideReached = false;
				lerpTime = 0;
				lerpStartPosition = transform.position;
				lerpTargetPosition = targetPosition+ rootOffset;

				currentCurve = (dropOnLedge) ? curvesHolder.ReturnCurve(CurveType.dropLedge) : curvesHolder.ReturnCurve(CurveType.mount);
				currentCurve.transform.rotation = targetPoint.transform.rotation;
				BezierPoint[] points = currentCurve.GetAnchorPoints();
				points[0].transform.position = lerpStartPosition;
				points[points.Length -1].transform.position = lerpTargetPosition;
			}

			if(dropOnLedge)
				anim.CrossFade("Drop Ledge", 0.4f);

			dropOnLedge = false;	

			if(enableRootMovement)
				lerpTime += Time.deltaTime *2;

			if(lerpTime>= 0.99f)
			{
				lerpTime =1;
				waitToStartClimb = false;
				lockInput = false;
				initTransit = false;
				ikLandSideReached =false;
				climbState = targetState;
			}

			Vector3 targetPos = currentCurve.GetPointAt(lerpTime);
			transform.position = targetPos;

			HandleWeightAll(lerpTime, mountAnimationCurve);
			HandleRotation();	
		}

        
        #endregion

		#region Falloff

		void InitiateFallOff()
		{
			if(climbState== ClimbStates.onPoint)
			{
				if(Input.GetKeyUp(KeyCode.X)){
					climbing = false;
					initTransit = false;
					ik.AddWeightInfluenceAll(0);
					GetComponent<Controller.StateManager>().EnableController();
					anim.SetBool("onAir", true);
				}
			}
		}
		#endregion

		#region Animations

		TransitDir ReturnTransitDirection(Vector3 inputD, bool jump)
		{
			TransitDir retVal = default(TransitDir);
			float targetAngle = Mathf.Atan2(inputD.x, inputD.y) * Mathf.Rad2Deg;

			if(!jump)
			{
				if(Mathf.Abs(inputD.y) > 0)
				{
					retVal = TransitDir.movementHorizontal;
				}
				else{
					retVal = TransitDir.movementVertical;
				}
			}
			else{
				if(targetAngle < 22.5 && targetAngle> -22.5f)
				{
					retVal = TransitDir.jumpUp;
				}
				else if(targetAngle < 180 + 22.5f && targetAngle > 180 - 22.5f)
				{
					retVal = TransitDir.jumpDown;
				}
				else if(targetAngle < 90 + 22.5f && targetAngle > 90 - 22.5f)
				{
					retVal = TransitDir.jumpRight;
				}
				else if(targetAngle < -90 + 22.5f && targetAngle > -90 - 22.5f)
				{
					retVal = TransitDir.jumpLeft;
				}

				if(Mathf.Abs(inputD.y) > Mathf.Abs(inputD.x))
				{
					if(inputD.y < 0)
					{
						retVal = TransitDir.jumpDown;
					}
					else{
						retVal = TransitDir.jumpUp;
					}
				}
			}
			return retVal;
		}
		enum TransitDir{
			movementHorizontal,
			movementVertical,
			jumpUp,
			jumpDown,
			jumpLeft,
			jumpRight
		}

		void PlayAnim(TransitDir dir, bool jump=false)
		{
			int target = 0;
			switch (dir)
			{
				case TransitDir.movementHorizontal:
					target = 5;
					break;
				case TransitDir.movementVertical:
					target = 6;
					break;
				case TransitDir.jumpUp:
					target = 0;
					break;
				case TransitDir.jumpDown:
					target = 1;
					break;
				case TransitDir.jumpLeft:
					target = 3;
					break;
				case TransitDir.jumpRight:
					target = 4;
					break;
			}
			anim.SetInteger("JumpType", target);
			if(!jump)
				anim.SetBool("Move", true);
			else
				anim.SetBool("Jump", true);	
		}

		#endregion

		#region Linear

		void UpdateLinearVariables()
		{
			if(!initTransit)
			{
				initTransit = true;
				enableRootMovement = true;
				rootReached = false;
				ikFollowSideReached = false;
				ikLandSideReached = false;
				lerpTime = 0;
				lerpStartPosition = transform.position;
				lerpTargetPosition = targetPosition+ rootOffset;

				Vector3 directionToPoint =(lerpTargetPosition- lerpStartPosition).normalized;

				bool twoStep = (targetState == ClimbStates.betweenPoints);
				Vector3 back = -transform.forward * 0.05f;

				bool diffType = targetPoint.pointType != currentPoint.pointType;
				Vector3 down =-transform.up * 0.2f;

				if(diffType)
				{
					if(currentPoint.pointType == PointType.hanging)
						diffType = false;
				}
				if(diffType && twoStep)
					lerpTargetPosition += down;
				else if(twoStep)
					lerpTargetPosition += back;

				lerpDistance = Vector3.Distance(lerpTargetPosition, lerpStartPosition);
				InitIK(directionToPoint, !twoStep);	
			}
		}

		void LinearRootMovemnt()
		{
			float speed = linearSpeed * Time.deltaTime;
			float _lerpSpeed = speed / lerpDistance;
			lerpTime += _lerpSpeed;

			if(lerpTime > 1)
			{
				lerpTime =1;
				rootReached = true;
			}

			Vector3 currentPosition = Vector3.LerpUnclamped(lerpStartPosition, lerpTargetPosition, lerpTime);
			transform.position = currentPosition;

			HandleRotation();
		}

		void LerpIKLandingSide_Linear()
		{
			float speed = linearSpeed * Time.deltaTime;
			float _lerpSpeed = speed / lerpDistance;

			landingSideIKTime += _lerpSpeed * 2;

			if(landingSideIKTime > 1)
			{
				landingSideIKTime = 1;
				ikLandSideReached = true;
			}

			Vector3 landingIKPosition = Vector3.LerpUnclamped(ikStartPosition[0], ikTargetPosition[0], landingSideIKTime);
			ik.UpdateTargetPosition(landingSideIK, landingIKPosition);

			followingSideIKTime += _lerpSpeed *2;
			if(followingSideIKTime > 1)
			{
				followingSideIKTime = 1;
				ikFollowSideReached = true;
			}
			if(targetPoint.pointType == PointType.hanging)
			{
				ik.InfluenceWeight(AvatarIKGoal.LeftFoot, 0);
				ik.InfluenceWeight(AvatarIKGoal.RightFoot, 0);
			}
			else
			{
				Vector3 followingIKPosition = Vector3.LerpUnclamped(ikStartPosition[0], ikTargetPosition[0], followingSideIKTime);
				ik.UpdateTargetPosition(followingSideIK, followingIKPosition);
			}

			
 		}
		#endregion
		#region Direct
		void UpdateDirectVariables(Vector3 inputD)
		{
			if(!initTransit)
			{
				initTransit = true;
				enableRootMovement = false;
				rootReached = false;
				ikFollowSideReached= false;
				ikLandSideReached = false;
				lerpTime = 0;
				rootMotionTime = 0;
				lerpTargetPosition = targetPosition + rootOffset;
				lerpStartPosition = transform.position;

				bool vertical =(Mathf.Abs(inputD.y) > 0.1f);
				currentCurve = FindCurveByInput(vertical, inputD);
				currentCurve.transform.rotation = currentPoint.transform.rotation;

				/* if(!vertical)
				{
					if(!(inputD.x > 0))
					{
						Vector3 eulers = currentCurve.transform.eulerAngles;
						eulers.y = -180;
						currentCurve.transform.eulerAngles = eulers;
					}
				}
				else{
					if(!(inputD.y > 0))
					{
						Vector3 eulers = currentCurve.transform.eulerAngles;
						eulers.x = 180;
						eulers.y = 180;
						currentCurve.transform.eulerAngles = eulers;
					}
				} */

				BezierPoint[] points = currentCurve.GetAnchorPoints();
				points[0].transform.position = lerpStartPosition;
				points[points.Length -1].transform.position = lerpTargetPosition;

				InitIK_Direct(inputDirection);
			}

		}

		BezierCurve FindCurveByInput(bool vertical, Vector3 inpD)
		{
			BezierCurve retVal = null;

			if(!vertical)
			{
				if(inpD.x > 0)
				{
					retVal = curvesHolder.ReturnCurve(CurveType.right);
				}
				else{
					retVal = curvesHolder.ReturnCurve(CurveType.left);
				}
			}
			else{
				if(inpD.y > 0)
				{
					retVal = curvesHolder.ReturnCurve(CurveType.up);
				}
				else{
					retVal = curvesHolder.ReturnCurve(CurveType.down);
				}
			}

			return retVal;
		}

		void DirectRootMovement()
		{
			if(enableRootMovement)
			{lerpTime += Time.deltaTime * directSpeed;}
			else{
				if(rootMotionTime < rootMotionMax)
					rootMotionTime += Time.deltaTime;
				else	
					enableRootMovement= true;	
			}

			if(lerpTime > 0.95f)
			{
				lerpTime = 1;
				rootReached = true;
			}

			HandleWeightAll(lerpTime, jumpingAnimationCurve);

			Vector3 targetPos = currentCurve.GetPointAt(lerpTime);
			transform.position = targetPos;

			HandleRotation();
		}

		void DirectHandleIK()
		{
			if(inputDirection.y != 0)
			{
				LerpIKHands_Direct();
				LerpIKFeet_Direct();
			}
			else{
				LerpIKLandingSide_Direct();
				LerpIKFollowingSide_Direct();
			}
		}
		#endregion

		#region IK
		AvatarIKGoal landingSideIK;
		AvatarIKGoal followingSideIK;
		float landingSideIKTime = 0;
		float followingSideIKTime = 0;

		Vector3[] ikStartPosition = new Vector3[4];
		Vector3[] ikTargetPosition = new Vector3[4];

		void InitIK(Vector3 directionToPoint, bool opposite)
		{
			Vector3 relativeDirection = transform.InverseTransformDirection(directionToPoint);
			if(Mathf.Abs(relativeDirection.y) > 0.5f)
			{
				float targetAnim = 0;
				if(targetState == ClimbStates.onPoint)
				{
					landingSideIK = ik.ReturnOppositeIK(landingSideIK);
				}
				else
				{
					if(Mathf.Abs(relativeDirection.x) > 0)
					{
						if(relativeDirection.x < 0)
							landingSideIK = AvatarIKGoal.LeftHand;
						else	
							landingSideIK = AvatarIKGoal.RightHand;						
					}

					targetAnim = (landingSideIK == AvatarIKGoal.RightHand) ? 1 : 0;
					if(relativeDirection.y < 0)
						targetAnim=(landingSideIK == AvatarIKGoal.RightHand) ? 0 : 1;

					anim.SetFloat("Movement", targetAnim);	
				}
			}
			else
			{
				landingSideIK = (relativeDirection.x < 0) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

				if(opposite)
				{
					landingSideIK = ik.ReturnOppositeIK(landingSideIK);
				}
			}

			landingSideIKTime = 0;

			UpdateIKTarget(0, landingSideIK, targetPoint);

			followingSideIK = ik.ReturnOppositeLimb(landingSideIK);
			followingSideIKTime = 0;
			UpdateIKTarget(1, followingSideIK, targetPoint);	
		}
		void InitIK_Direct(Vector3 directionToPoint)
		{
			if(directionToPoint.y != 0)
			{
				followingSideIKTime = 0;
				landingSideIKTime = 0;

				UpdateIKTarget(0, AvatarIKGoal.LeftHand, targetPoint);
				UpdateIKTarget(1, AvatarIKGoal.LeftFoot, targetPoint);
				UpdateIKTarget(2, AvatarIKGoal.RightHand, targetPoint);
				UpdateIKTarget(3, AvatarIKGoal.RightFoot, targetPoint);
			}
			else
			{
				InitIK(directionToPoint, false);
				InitIKOppsoite();
			}
		}
		void InitIKOppsoite()
		{
			UpdateIKTarget(2, ik.ReturnOppositeIK(landingSideIK), targetPoint);
			UpdateIKTarget(3, ik.ReturnOppositeIK(followingSideIK), targetPoint);
		}
		void UpdateIKTarget(int positionIndex, AvatarIKGoal iKGoal, Point targetPoint)
		{
			ikStartPosition[positionIndex] = ik.ReturnCurrentPosition(iKGoal);
			ikTargetPosition[positionIndex] = targetPoint.ReturnIK(iKGoal).target.transform.position;
			ik.UpdatePoint(iKGoal, targetPoint);
		}
		void HandleWeightAll(float t, AnimationCurve aCurve)
		{
			float inf = aCurve.Evaluate(t);
			ik.AddWeightInfluenceAll(1 - inf);

			if(currentPoint.pointType == PointType.hanging && targetPoint.pointType == PointType.braced)
			{
				float inf2 = zeroToOne.Evaluate(t);
				ik.InfluenceWeight(AvatarIKGoal.LeftFoot, inf2);
				ik.InfluenceWeight(AvatarIKGoal.RightFoot, inf2);
			}
		}
		void LerpIKFeet_Direct(){
			if(targetPoint.pointType == PointType.hanging)
			{
				ik.InfluenceWeight(AvatarIKGoal.LeftFoot, 0);
				ik.InfluenceWeight(AvatarIKGoal.RightFoot, 0);
			}
			else
			{
				if(enableRootMovement)				
					followingSideIKTime += Time.deltaTime * 5;
				if(followingSideIKTime > 1)
				{
					followingSideIKTime =1;
					ikFollowSideReached= true;
				}

				Vector3 leftFootPosition = Vector3.LerpUnclamped(ikStartPosition[1], ikTargetPosition[1], followingSideIKTime);
				ik.UpdateTargetPosition(AvatarIKGoal.LeftFoot, leftFootPosition);

				Vector3 rightFootPosition = Vector3.LerpUnclamped(ikStartPosition[3], ikTargetPosition[3], followingSideIKTime);
				ik.UpdateTargetPosition(AvatarIKGoal.RightFoot, rightFootPosition);
			}
		}
		void LerpIKHands_Direct(){
			if(targetPoint.pointType == PointType.hanging)
			{
				ik.InfluenceWeight(AvatarIKGoal.LeftHand, 0);
				ik.InfluenceWeight(AvatarIKGoal.RightHand, 0);
			}
			else
			{
				if(enableRootMovement)				
					followingSideIKTime += Time.deltaTime * 5;
				if(followingSideIKTime > 1)
				{
					followingSideIKTime =1;
					ikFollowSideReached= true;
				}

				Vector3 leftHandPosition = Vector3.LerpUnclamped(ikStartPosition[1], ikTargetPosition[1], followingSideIKTime);
				ik.UpdateTargetPosition(AvatarIKGoal.LeftHand, leftHandPosition);

				Vector3 rightHandPosition = Vector3.LerpUnclamped(ikStartPosition[3], ikTargetPosition[3], followingSideIKTime);
				ik.UpdateTargetPosition(AvatarIKGoal.RightHand, rightHandPosition);
			}
		}
		void LerpIKLandingSide_Direct()
		{
			if(enableRootMovement)
				landingSideIKTime += Time.deltaTime * 3.2f;

			if(landingSideIKTime > 1)
			{
				landingSideIKTime = 1;
				ikLandSideReached = true;
			}	
			Vector3 landPosition = Vector3.LerpUnclamped(ikStartPosition[0], ikTargetPosition[0], landingSideIKTime);
			ik.UpdateTargetPosition(landingSideIK, landPosition);

			if(targetPoint.pointType == PointType.hanging)
			{
				ik.InfluenceWeight(AvatarIKGoal.LeftFoot, 0);
				ik.InfluenceWeight(AvatarIKGoal.RightFoot, 0);
			}
			else
			{
				Vector3 followPosition = Vector3.LerpUnclamped(ikStartPosition[1], ikTargetPosition[1], landingSideIKTime);
				ik.UpdateTargetPosition(followingSideIK, followPosition);
			}
		}

		void LerpIKFollowingSide_Direct()
		{
			if(enableRootMovement)
				followingSideIKTime += Time.deltaTime * 2.6f;

			if(followingSideIKTime > 1)
			{
				followingSideIKTime = 1;
				ikFollowSideReached = true;
			}

			Vector3 landPosition = Vector3.LerpUnclamped(ikStartPosition[2], ikTargetPosition[2], followingSideIKTime);
			ik.UpdateTargetPosition(ik.ReturnOppositeIK(landingSideIK), landPosition);

			if(targetPoint.pointType == PointType.hanging)
			{
				ik.InfluenceWeight(AvatarIKGoal.LeftFoot, 0);
				ik.InfluenceWeight(AvatarIKGoal.RightFoot, 0);
			}
			else{
				Vector3 followPosition = Vector3.LerpUnclamped(ikStartPosition[3], ikTargetPosition[3], followingSideIKTime);
				ik.UpdateTargetPosition(ik.ReturnOppositeIK(followingSideIK), followPosition);
			}	
		}
		#endregion

		#region Dismount

		void HandleDismountVariables()
		{
			if(!initTransit)
			{
				initTransit = true;
				enableRootMovement = false;
				rootReached = false;
				ikLandSideReached = false;
				ikFollowSideReached = false;
				lerpTime = 0;
				rootMotionTime = 0;
				lerpStartPosition = transform.position;
				lerpTargetPosition = targetPosition;

				currentCurve = curvesHolder.ReturnCurve(CurveType.dismount);
				BezierPoint[] points = currentCurve.GetAnchorPoints();
				currentCurve.transform.rotation = transform.rotation;

				landingSideIKTime = 0;
				followingSideIKTime = 0;


			}
		}

		void DismountRootMovement()
		{
			if(enableRootMovement)
				lerpTime += Time.deltaTime / 2;

			if(lerpTime >= 0.99f)
			{
				lerpTime = 1;
				rootReached = true;
			}

			Vector3 targetPos = currentCurve.GetPointAt(lerpTime);
			transform.position = targetPos;	
		}
		void HandleDismountIK()
		{
			if(enableRootMovement)
				landingSideIKTime += Time.deltaTime * 3;

			followingSideIKTime += Time.deltaTime * 2;

			HandleWeight_Dismount(landingSideIKTime, followingSideIKTime, 1, 0);	
		}

		void HandleWeight_Dismount(float ht, float ft, float from, float to)
		{
			float t1 = ht * 3;
			if(t1 > 1)
			{
				t1 = 1;
				ikLandSideReached = true;
			}

			float handsWeight = Mathf.Lerp(from, to, t1);
			ik.InfluenceWeight(AvatarIKGoal.LeftHand, handsWeight);
			ik.InfluenceWeight(AvatarIKGoal.RightHand, handsWeight);

			float t2 = ft * 1;
			if(t2 > 1)
			{
				t2 = 1;
				ikFollowSideReached = true;
			}

			float feetWeight = Mathf.Lerp(from, to, t2);
			ik.InfluenceWeight(AvatarIKGoal.LeftFoot, feetWeight);
			ik.InfluenceWeight(AvatarIKGoal.RightFoot, feetWeight);
		}
		void DismountWrapUp()
		{
			if(rootReached)
			{
				climbing = false;
				initTransit = false;
				GetComponent<Controller.StateManager>().EnableController();
			}
		}
		#endregion

		#region Universal
		bool waitForWrapUp;
		Vector3 ConvertToInputDirection(float horizontal, float vertical)
		{
			int h = (horizontal != 0)?(horizontal< 0) ? -1: 1: 0;
			int v = (vertical != 0) ? (vertical < 0) ? -1: 1 : 0;

			int z = v + h;

			z= (z!= 0)?(z< 0)? -1: 1:0;

			Vector3 retVal = Vector3.zero;
			retVal.x = h;
			retVal.y = v;
			return retVal; 
		}
		void WrapUp(bool direct = false)
		{
			if(rootReached)
			{
				if(!anim.GetBool("Jump"))
				{
					if(!waitForWrapUp)
					{
						StartCoroutine(WrapUpTransition(0.05f));
						waitForWrapUp = true;
					}
				}
			}
		}
		IEnumerator WrapUpTransition(float t)
		{
			yield return new WaitForSeconds(t);
			climbState = targetState;
			if(climbState == ClimbStates.onPoint)
				currentPoint = targetPoint;

			//reset variables
			initTransit = false;
			lockInput = false;
			inputDirection = Vector3.zero;
			waitForWrapUp = false;	
		}
		void HandleRotation()
        {
            Vector3 targetDir = targetPoint.transform.forward;
			if(targetDir == Vector3.zero)
				targetDir =transform.forward;

			Quaternion targetRot = Quaternion.LookRotation(targetDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime* 5);

        }
		#endregion

		#endregion
    }
}
