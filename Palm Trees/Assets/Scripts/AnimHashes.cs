using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	
	public class AnimHashes{
		public int vertical = Animator.StringToHash("Vertical");
		public int horizontal = Animator.StringToHash("Horizontal");
		public int leftFootForward = Animator.StringToHash("Left Foot Forward");
		public int jumpForward = Animator.StringToHash("Jump Forward");
		public int jumpIdle = Animator.StringToHash("Jump Idle");
		public int isGrounded = Animator.StringToHash("Is Grounded");
		public int normalLanding = Animator.StringToHash("Land Normal");
		public int hardLanding = Animator.StringToHash("Land Hard");
		public int landWithRoll = Animator.StringToHash("Land Rolling");
		public int isInteracting = Animator.StringToHash("Is Interacting");
		public int vaultWalk = Animator.StringToHash("Vault Walk");
	
	}

}