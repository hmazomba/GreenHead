using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        #region Variables
        public float health;        
        public State currentState;
        public MovementVariables movementVariables;

        [Header("Controller Physics")]
        [HideInInspector]
        public new Rigidbody rigidbody;
        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Transform mTransform;
        [HideInInspector]
        public float timeSinceJump;

        [Header("Animator Data")]
        [HideInInspector]
        public Animator anim;        
        [HideInInspector]
        public new CapsuleCollider collider;        
        public AnimHashes hashes;
        public AnimatorData animData;
        public VaultData vaultData;

        [Header("Flags")]
        public bool isJumping;
        public bool isGrounded;
        public bool isVaulting;
        #endregion
        private void Start()
        {
            
            mTransform = this.transform;
            collider = GetComponent<CapsuleCollider>();
            rigidbody = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            hashes = new AnimHashes();
            animData = new AnimatorData(anim);
            //vaultData = new VaultData();
        }

        private void Update()
        {
            delta = Time.deltaTime;
            if(currentState != null)
            {
                currentState.Tick(this);
            }
        }
        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            if(currentState != null)
            {
                currentState.FixedTick(this);
            }
        }
    }
}
