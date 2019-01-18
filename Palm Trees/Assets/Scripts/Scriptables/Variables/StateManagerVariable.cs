using UnityEngine;
using System.Collections;
namespace SA
{
    [CreateAssetMenu(menuName = "Variables/State Manager")]
    public class StateManagerVariable: ScriptableObject
    {
        public StateManager value;
        public void Set(StateManager s)
        {
            value = s;
        }
    }
}