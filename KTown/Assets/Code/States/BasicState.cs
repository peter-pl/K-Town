using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace States
{
    /// <summary>
    /// 
    /// </summary>
    public class BasicState : MonoBehaviour
    {
        public static Dictionary<StateType, Dictionary<string, BasicState>> ALL_STATES;

        public string StateName = "State";
        public IntEvent ValueChangedEvent = new IntEvent();
        public StateType mStateType = StateType.GameState;

        public int Value { get { return value; } }
        int value;

        void Awake()
        {
            if (ALL_STATES == null) ALL_STATES = new Dictionary<StateType, Dictionary<string, BasicState>>();
            if (!ALL_STATES.ContainsKey(mStateType))
            {
                ALL_STATES.Add(mStateType, new Dictionary<string, BasicState>());
            }
            if (ALL_STATES[mStateType].ContainsKey(StateName))
            {
                Debug.LogError($"Duplicate GameState named: {StateName}");
            }
            else
            {
                ALL_STATES[mStateType].Add(StateName, this);
            }
        }
        public void SetValue(int val, bool onlyIncrement = false)
        {
            if (onlyIncrement && val <= value) { Debug.Log($"Discarding set value only increment: {onlyIncrement}, from: {value}, trying to set to: {val}"); return; }
            value = val;
            ValueChangedEvent.Invoke(val);
        }
        public void UserChangedValue(int val)
        {
            SetValue(val);
            Dictionary<string, int> args = new Dictionary<string, int>();
            args.Add("game_state_change", 1);
            args.Add(StateName, val);
            GameManager.INSTANCE.SendServerMessage(args, true, "");
        }

        public static BasicState GET_STATE(string stateName)
        {
            foreach (var v in ALL_STATES)
            {
                if (v.Value.ContainsKey(stateName)) return v.Value[stateName];
            }
            return null;
        }
        public static BasicState GET_STATE(StateType t, string stateName)
        {
            if (!ALL_STATES.ContainsKey(t)) return null;
            if (ALL_STATES[t].ContainsKey(stateName)) return ALL_STATES[t][stateName];
            return null;
        }
        public static bool SET_STATE(StateType t, string stateName, int value)
        {
            var v = GET_STATE(t, stateName);
            if (v == null) return false;
            v.SetValue(value);
            return true;
        }
        public static void GET_ALL(StateType t, List<string> args, List<int> vals)
        {
            if (!ALL_STATES.ContainsKey(t)) return;
            foreach (var v in ALL_STATES[t])
            {
                if (v.Value.Value > 0) //if value = 0, there is no need to pass it
                {
                    args.Add(v.Key);
                    vals.Add(v.Value.Value);
                }
            }
        }
    }
}

public class IntEvent : UnityEvent<int> { }
public enum StateType { GameState = 0, InfoState = 1, UserVariable = 2 }