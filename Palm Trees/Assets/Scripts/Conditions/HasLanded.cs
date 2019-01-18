using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    //check if the controller has landed or not
    [CreateAssetMenu(menuName ="Conditions/Has Landed")]
    public class HasLanded : Condition
    {
        //the amount of time spent in the air
        public float hardLandingThreshold = 1.5f;
        public float maxLandThreshold = 4f;
        //will be initiate once the #endregion
        //public State fastLandState;
        public override bool CheckCondition(StateManager state)
        {
            float timeDifference = Time.realtimeSinceStartup - state.timeSinceJump;
            if(timeDifference > 0.5f)
            {
                bool result = state.isGrounded;

                if(result)
                {
                    if(timeDifference > hardLandingThreshold && timeDifference < maxLandThreshold){
                        if(state.movementVariables.moveAmount > 0.3f)
                        {
                            state.anim.SetBool(state.hashes.isInteracting, true);
                            state.anim.CrossFade(state.hashes.landWithRoll, 0.2f);
                        }
                        else
                        {
                            state.anim.SetBool(state.hashes.isInteracting, true);
                            state.anim.CrossFade(state.hashes.hardLanding, 0.2f);
                        }
                    }
                    else if (timeDifference > maxLandThreshold)
                    {
                        state.anim.SetBool(state.hashes.isInteracting, true);
                        state.anim.CrossFade(state.hashes.hardLanding, 0.2f);
                    }
                    else{
                        state.anim.CrossFade (state.hashes.normalLanding, 0.2f);
                    }
                }
                return result;
            }
            else
            {
                return false;
            }
            
        }
    }
}