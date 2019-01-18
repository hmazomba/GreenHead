using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Wait For Animation End")]
    public class WaitForAnimationToEnd : Condition
    {
        //waits for an animation to end before resetting the animator state bool 
        public string targetBool = "Is Interacting";
        public override bool CheckCondition(StateManager state)
        {
            bool retVal = !state.anim.GetBool(targetBool);
            return retVal;
        }
    }
}