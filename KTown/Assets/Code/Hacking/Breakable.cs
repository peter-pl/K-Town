using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hacking
{
    class Breakable : Interactable, IRefreshable
    {
        public float BreakingTime = 30;
        public Renderer mRenderer;
        public Collider BlockingCollider;

        bool available = false;

        public void Refesh(MapController.VRMode mode)
        {
            available = true;
            mCollider.enabled = true; 
            mRenderer.enabled = true;
            BlockingCollider.enabled = mode == MapController.VRMode.Loot; //don't block scout
        }
        protected override void Fire()
        {
            if (available) MapController.INSTANCE.mIcebreaker.Start(this, OnBroken, UIInfo, BreakingTime);
        }
        void OnBroken()
        {
            Debug.Log($"{this} was broken");
            mCollider.enabled = false;
            mRenderer.enabled = false;
            BlockingCollider.enabled = false;
        }
    }
}