using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Hacking
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour, IClickable
    {
        public float PressTimeToInteract = 1;
        public string UIInfo = "Interactable";

        protected Collider mCollider;

        protected virtual void Awake()
        {
            mCollider = GetComponent<Collider>();
        }
        public ClickResponse OnClick()
        {
            return ClickResponse.Press;
        }
        public bool OnPress(float f)
        {
            if (f < PressTimeToInteract) return false;
            else
            {
                Fire();
                return true;
            }
        }
        protected virtual void Fire() { }
        public bool OnScan()
        {
            if (UIInfo == "") return false;
            CursorCtrl.CURSOR_HOVER_INFO_EVENT.Invoke(this, UIInfo);
            return true;
        }
    }
}