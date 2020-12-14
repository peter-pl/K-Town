using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    [RequireComponent(typeof(Collider))]
    class StepOnTrap : MonoBehaviour, IRefreshable, IPlayerTrigger
    {
        public string UIInfo = "Detector Contact";
        public int HeatValue = 1;

        Collider mCollider;
        bool available = false;

        void Awake()
        {
            mCollider = GetComponent<Collider>();
        }
        public void Refesh(MapController.VRMode mode)
        {
            mCollider.enabled = true;
            available = mode == MapController.VRMode.Loot;
        }
        public bool OnPlayerTriggered()
        {
            if (available)
            {
                GameManager.INSTANCE.AddHeatEvent.Invoke(HeatValue, UIInfo);
                mCollider.enabled = false;
                available = false;
                return true;
            }
            return false;
        }
    }
}