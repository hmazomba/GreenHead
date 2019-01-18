#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Climbing
{
    public class DrawLine: MonoBehaviour
    {
        public List<Connection> ConnectedPoints = new List<Connection>();
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