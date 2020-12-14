using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hacking
{
    public class SetActiveAtTime : MonoBehaviour
    {
        public int MinuteToTurnOn = 0;

        void Start()
        {
            int t = MinuteToTurnOn - GameManager.GAME_TIME_DIFFERENCE;
            if (t > 0)
            {
                LeanTween.delayedCall(60 * t, () => gameObject.SetActive(true));
                gameObject.SetActive(false);
            }
        }
    }
}