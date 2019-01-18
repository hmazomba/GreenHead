using UnityEngine;
using System.Collections;

namespace SA
{
    [System.Serializable]
    public class VaultData
    {
        public Vector3 startPosition;
        public Vector3 endingPosition;
        public float vaultSpeed = 2;
        public float animLength;

        public float vaultTime;
        public bool isInit;
    }
}