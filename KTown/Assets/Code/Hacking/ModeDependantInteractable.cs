using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    public class ModeDependantInteractable : Interactable, IRefreshable
    {
        public MapController.VRMode Mode;

        public void Refesh(MapController.VRMode mode)
        {
            mCollider.enabled = mode == Mode;
        }
    }
}