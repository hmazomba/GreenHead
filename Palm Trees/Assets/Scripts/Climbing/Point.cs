using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
	[System.Serializable]
	public class Point : MonoBehaviour {
		public PointType pointType;
		public List<Neighbour> neighbours = new List<Neighbour>();
		public List<IKPositions> ikPositions = new List<IKPositions>();
        public bool dismountPoint;

        public Neighbour ReturnNeighbourFromDirection(Vector3 dir)
		{
			Neighbour retVal = null;
			for (int i = 0; i < neighbours.Count; i++)
			{
				if(neighbours[i].direction == dir)
				{
					retVal = neighbours[i];
					break;
				}
			}
			return retVal;
		}
		public IKPositions ReturnIK(AvatarIKGoal goal)
		{
			IKPositions retVal = null;
			for(int i = 0; i < ikPositions.Count; i++)
			{
				if(ikPositions[i].ik == goal)
				{
					retVal = ikPositions[i];
					break;
				}
			}
			return retVal;
		}

		public Neighbour ReturnNeighbour(Point target)
		{
			Neighbour retVal = null;
			for (int i = 0; i < neighbours.Count; i++)
			{
				if(neighbours[i].target == target)
				{
					retVal = neighbours[i];
					break;
				}
			}
			return retVal;
		}
	}
	//This a class variable for the players IK positions
	[System.Serializable]
	public class IKPositions
	{
		public AvatarIKGoal ik;
		public Transform target;
		public Transform hint;
	}

	[System.Serializable]
	public class Neighbour
	{
		//direction to nearest neighbour point
		public Vector3 direction;
		public Point target;
		public ConnectionType connectionType;
		public bool customConnection;
	}

	public enum ConnectionType{
		inBetween,
		direct,
		dismount, 
		falling
	}

	public enum PointType
	{
		braced,
		hanging
	}
}
