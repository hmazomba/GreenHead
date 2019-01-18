using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	public class AssignStateManager : MonoBehaviour {

		//assigns the statemanagervariable to the statemanager
		public StateManagerVariable targetVariable;
		private void OnEnable()
		{
			targetVariable.value = GetComponent<StateManager>();
			Destroy(this);
		}
	}
}

