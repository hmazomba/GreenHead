using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public static class Layers
    {
        public static LayerMask ignoreLayersController;
        static Layers()
        {
            ignoreLayersController = ~(1<<3 | 1 << 9);
        }

    }
}