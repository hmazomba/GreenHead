using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Vaulting/Animation Data")]
    public class VaultAnim : ScriptableObject
    {
        public bool isDown;
        public float min;
        public float max;
        public AnimationClip clip;
        public AnimationCurve speedCurve;
        public AnimationCurve elevationCurve;
        public string animName;


    }
}