using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    class Prompts : MonoBehaviour
    {
        public Text mText;
        public float TimeToHide = 10;

        void Awake()
        {
            mText.text = "";
        }
        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }
        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }
        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
            {
                DisplayPrompt(logString);
            }
        }
        public void DisplayPrompt(string s)
        {
            mText.text = s;
            LeanTween.cancel(gameObject);
            LeanTween.delayedCall(gameObject, TimeToHide, (o) => mText.text = "");
        }
    }
}