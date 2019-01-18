#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Climbing

{
    public class DrawLineIndividual : MonoBehaviour
    {
        public List<Neighbour> ConnectedPoints = new List<Neighbour>();
        public bool refresh;

        void Update()
        {
            if(refresh)
            {
                ConnectedPoints.Clear();
                refresh = false;
            }
        }
    }
}
#endif