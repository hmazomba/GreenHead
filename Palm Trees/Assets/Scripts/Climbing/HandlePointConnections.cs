#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
	public class Connection{
		public Point target1;
		public Point target2;
		public ConnectionType connectionType;
	}
	[ExecuteInEditMode]
	public class HandlePointConnections : MonoBehaviour {
		//mininmum distance need to connect from one point to another
		public float minDistance = 2.5f;
		//threshold for a two-step transition or a jump to a point
		public float directThreshold = 1;
		public bool updateConnections;
		public bool resetConnections;

		List<Point> allPoints = new List<Point>();
		Vector3[] availableDirections = new Vector3[8];

		void CreateDirections()
		{
			availableDirections[0] = new Vector3(1, 0, 0);
			availableDirections[1] = new Vector3(-1, 0, 0);
			availableDirections[2] = new Vector3(0, 1, 0);
			availableDirections[3] = new Vector3(0, -1, 0);
			availableDirections[4] = new Vector3(-1, -1, 0);
			availableDirections[5] = new Vector3(1, 1, 0);
			availableDirections[6] = new Vector3(1, -1, 0);
			availableDirections[7] = new Vector3(-1, 1, 0);
		}

		void Update()
		{
			if(updateConnections)
			{
				GetPoints();
				CreateDirections();
				CreateConnections();
				FindDismountCandidates();
				RefreshAll();

				updateConnections=false;
			}

			if(resetConnections)
			{
				GetPoints();
				for (int p = 0; p < allPoints.Count; p++)
				{
					
				}
			}
		}

		void GetPoints()
		{
			allPoints.Clear();
			Point[] hp = GetComponentsInChildren<Point>();
			allPoints.AddRange(hp);
		}

		void CreateConnections()
		{
			for (int p = 0; p < allPoints.Count; p++)
			{
				Point currentPoint = allPoints[p];
				for (int d = 0; d < availableDirections.Length; d++)
				{
					List<Point> candidatePoints = CandidatePointsOnDirection(availableDirections[d], currentPoint);

					Point closest = ReturnClosest(candidatePoints, currentPoint);

					if(closest != null)
					{
						if(Vector3.Distance(currentPoint.transform.position, closest.transform.position) < minDistance)
						{	
							//disable diagonal jumping and allows two step transitions
							if(Mathf.Abs(availableDirections[d].y) > 0 && Mathf.Abs(availableDirections[d].x) > 0)
							{

								if(Vector3.Distance(currentPoint.transform.position, closest.transform.position) > directThreshold)
								{
									continue;
								}
							}

							AddNeighbour(currentPoint, closest, availableDirections[d]);
						}
					}
				}
			}
		}

        private void AddNeighbour(Point from, Point target, Vector3 targetDir)
        {
            Neighbour n = new Neighbour();
			n.direction = targetDir;
			n.target = target;
			n.connectionType = (Vector3.Distance(from.transform.position, target.transform.position) < directThreshold) ? ConnectionType.inBetween : ConnectionType.direct;

			from.neighbours.Add(n);

			UnityEditor.EditorUtility.SetDirty(from);
        }

        Point ReturnClosest(List<Point> listPoints, Point from)
        {
            Point retVal = null;

			float minDist = Mathf.Infinity;
			for (int i = 0; i < listPoints.Count; i++)
			{
				float tempDistance = Vector3.Distance(listPoints[i].transform.position, from.transform.position);

				if(tempDistance < minDist && listPoints[i] != from)
				{
					minDist = tempDistance;
					retVal=listPoints[i];
				}
			}

			return retVal;
        }

        void FindDismountCandidates()
		{
			GameObject dismountPrefab = Resources.Load("Dismount") as GameObject;
			if(dismountPrefab == null)
			{
				Debug.Log("You forgot the dismount prefab STUPID!!");
				return;
			}

			HandlePoints[] hp = GetComponentsInChildren<HandlePoints>();
			List<Point> candidates = new List<Point>();

			for (int i = 0; i < hp.Length; i++)
			{
				if(hp[i].dismountPoint)
				{
					candidates.AddRange(hp[i].pointsInOrder);
				}
			}

			if(candidates.Count > 0)
			{
				GameObject parentObj = new GameObject();
				parentObj.name = "Dismount Points";
				parentObj.transform.parent = transform;
				parentObj.transform.localPosition = Vector3.zero;
				parentObj.transform.position = candidates[0].transform.position;

				foreach (Point p in candidates)
				{
					Transform worldP = p.transform.parent;
					GameObject dismountObject = Instantiate(dismountPrefab, worldP.position, worldP.rotation) as GameObject;

					Vector3 targetPosition = worldP.position +((worldP.forward /1.6f) + Vector3.up * 1.7f);
					dismountObject.transform.position = targetPosition;
					Point dismountPoint = dismountObject.GetComponentInChildren<Point>();

					Neighbour n = new Neighbour();
					n.direction = Vector3.up;
					n.target = dismountPoint;
					n.connectionType = ConnectionType.dismount;
					p.neighbours.Add(n);

					Neighbour n2 = new Neighbour();
					n2.direction = -Vector3.up;
					n2.target = dismountPoint;
					n2.connectionType = ConnectionType.dismount;
					dismountPoint.neighbours.Add(n2);
					dismountObject.transform.parent = parentObj.transform; 
				}
			}
		}

		void RefreshAll()
		{
			DrawLine dl = transform.GetComponent<DrawLine>();
			if(dl != null)
				dl.refresh = true;

			for (int i = 0; i < allPoints.Count; i++)
			{
				DrawLineIndividual d = allPoints[i].transform.GetComponent<DrawLineIndividual>();
				if(d != null)
					dl.refresh = true;
			}	
		}

		public List<Connection> GetAllConnections()
		{
			List<Connection> retVal = new List<Connection>();

			for (int p = 0; p < allPoints.Count; p++)
			{
				for (int n = 0; n < allPoints[p].neighbours.Count; n++)
				{
					Connection con = new Connection();
					con.target1 = allPoints[p];
					con.target2 = allPoints[p].neighbours[n].target;
					con.connectionType = allPoints[p].neighbours[n].connectionType;

					if(!ContainsConnection(retVal, con))
					{
						retVal.Add(con);
					}
				}
			}

			return retVal;
		}

        bool ContainsConnection(List<Connection> listConnections, Connection connection)
        {
            bool retVal = false;
			for (int i = 0; i < listConnections.Count; i++)
			{
				if(listConnections[i].target1== connection.target1 && listConnections[i].target2 == connection.target2
					|| listConnections[i].target2 == connection.target1 && listConnections[i].target1 == connection.target2
				)
				{
					retVal = true;
					break;
				}
			}

			return retVal;
        }

        List<Point> CandidatePointsOnDirection(Vector3 targetDirection, Point from)
		{
			List<Point> retVal = new List<Point>();
			for (int p = 0; p < allPoints.Count; p++)
			{
				Point targePoint = allPoints[p];
				Vector3 direction = targePoint.transform.position - from.transform.position;
				Vector3 relativeDirection = from.transform.InverseTransformDirection(direction);
				if(IsDirectionValid(targetDirection, relativeDirection))
				{
					retVal.Add(targePoint);
				}
			}

			return retVal;
		}

        bool IsDirectionValid(Vector3 targetDirection, Vector3 candidate)
        {
            bool retVal = false;
			float targetAngle = Mathf.Atan2(targetDirection.x , targetDirection.y)* Mathf.Rad2Deg;
			float angle = Mathf.Atan2(candidate.x , candidate.y) * Mathf.Rad2Deg;
			if(angle < targetAngle + 22.5f && angle > targetAngle - 22.5f)
			{
				retVal = true;
			}

			return retVal;
        }
    }
}
#endif