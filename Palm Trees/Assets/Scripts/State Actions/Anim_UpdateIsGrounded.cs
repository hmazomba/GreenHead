using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/State Actions/Anim_UpdateIsGrounded")]
    public class Anim_UpdateIsGrounded : StateActions
    {
        //updates the animtor bool parameter
        public override void Execute(StateManager states)
        {
            states.anim.SetBool(states.hashes.isGrounded, states.isGrounded);
        }
    }
}