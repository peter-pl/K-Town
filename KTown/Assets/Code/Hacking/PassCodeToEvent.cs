using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Hacking
{
    class PassCodeToEvent : MonoBehaviour
    {
        public string Password = "pass";
        public UnityEvent OnPasswordSuccess = new UnityEvent();
        public UnityEvent OnPasswordFailed = new UnityEvent();

        void TryGuess(string s)
        {
            if (s.Equals(Password))
            {
                OnPasswordSuccess.Invoke();
            }
            else
            {
                OnPasswordFailed.Invoke();
            }
        }
        public void Init()
        {
            UserInterface.PassCode.INSTANCE.Init(TryGuess);
        }
    }
}