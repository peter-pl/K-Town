using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace States
{
    public class ConditionalOnGameState : MonoBehaviour
    {
        public BasicState mState;
        public GameObject Target;
        public List<int> ValuesActive;

        void Awake()
        {
            if (mState == null || Target == null)
            {
                Debug.LogError($"{this} doesn't have dependencies assigned!");
            }
            else
            {
                mState.ValueChangedEvent.AddListener(OnValueChanged);
            }
        }
        void OnValueChanged(int i)
        {
            if (ValuesActive.Contains(i)) Target.SetActive(true);
            else Target.SetActive(false);
        }
    }
}