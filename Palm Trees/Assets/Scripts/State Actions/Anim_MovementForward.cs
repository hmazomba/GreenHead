using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;


namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Anim_MovementForward")]
    public class Anim_MovementForward : StateActions
    {
        public StateActions[] stateActions;
        //Updates Animator values based on received input
        public override void Execute(StateManager states){
            if(stateActions!=null)
            {
                for (int i = 0; i < stateActions.Length; i++)
                {
                    stateActions[i].Execute(states);
                }
            }
            

            states.anim.SetFloat(states.hashes.vertical, states.movementVariables.moveAmount, 0.2f, states.delta); 
        }
    }
}