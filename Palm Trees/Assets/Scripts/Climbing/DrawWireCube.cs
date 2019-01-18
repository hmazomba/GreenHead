#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Climbing
{
    public class DrawWireCube : MonoBehaviour
    {
        public List<IKPositions> ikPositions = new List<IKPositions>();
        public bool refresh;

        void Update()
        {
            if(refresh)
            {
                ikPositions.Clear();
                refresh = false;
            }
        }
    }
}
#endif