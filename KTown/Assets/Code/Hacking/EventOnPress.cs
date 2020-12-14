using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

namespace Hacking
{
    class EventOnPress : Interactable
    {
        public UnityEvent OnPressEvent = new UnityEvent();
        public bool ManyUses = false;

        bool available = true;

        protected override void Fire()
        {
            if (available || ManyUses)
            {
                available = false;
                OnPressEvent.Invoke();
            }
        }
        public void TurnOff()
        {
            UIInfo = "Unavailable";
            available = false;
            ManyUses = false;
        }
    }
}