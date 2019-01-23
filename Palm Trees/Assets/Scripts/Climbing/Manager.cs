#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    public class Manager: MonoBehaviour
    {
        public List<Point> allPoints = new List<Point>();

        void Start()
        {
            PopulateAllPoints();
        }

        public void Init()
        {
            PopulateAllPoints();

        }

        void PopulateAllPoints()
        {
            Point[] allP = GetComponentsInChildren<Point>();
            foreach (Point p in allP)
            {
                if(!allPoints.Contains(p))
                    allPoints.Add(p);
            }
        }

        public Point ReturnNeighbourPointFromDirection(Vector3 inputDirection, Point currentPoint)
        {
            Point retVal = null;

            foreach (Neighbour n in currentPoint.neighbours)
            {
                if(n.direction == inputDirection)
                {
                    retVal = n.target;
                }
            }

            return retVal;
        }

        public Neighbour ReturnNeighbour(Vector3 inputDirection, Point currentPoint)
        {
            Neighbour retVal = null;

            foreach (Neighbour n in currentPoint.neighbours)
            {
                if(n.direction == inputDirection)
                {
                    retVal = n;
                }
            }

            return retVal;
        }

        public Point ReturnClosest(Vector3 from)
        {
            Point retVal = null;

            float minDist = Mathf.Infinity;

            for (int i = 0; i < allPoints.Count; i++)
            {
                float dist = Vector3.Distance(allPoints[i].transform.position, from);

                if(dist < minDist)
                {
                    retVal = allPoints[i];
                    minDist = dist;
                }
            }

            return retVal;
        }
    }
}
#endif