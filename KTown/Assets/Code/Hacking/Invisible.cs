using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    [RequireComponent(typeof(Renderer))]
    class Invisible : MonoBehaviour, IRefreshable
    {
        protected Renderer mRenderer;

        void Awake()
        {
            mRenderer = GetComponent<Renderer>();
        }
        public void Refesh(MapController.VRMode mode)
        {
            mRenderer.enabled = mode == MapController.VRMode.Scout;
        }
    }
}