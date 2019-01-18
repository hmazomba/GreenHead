using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Set Root Motion Application")]
    public class SetRootMotionApplication : StateActions
    {
        public bool status;
        public override void Execute(StateManager states)
        {
            states.anim.applyRootMotion = status;
        }
    }
}