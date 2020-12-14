using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using States;

[CustomEditor(typeof(BasicState))]
public class StateInspector : Editor
{
    string valInput = "";

    public override void OnInspectorGUI()
    {
        BasicState bs = target as BasicState;

        base.OnInspectorGUI();
        GUILayout.Space(10);
        StringBuilder s = new StringBuilder("Value: 0 = default, 1, 2, 3... = ");
        if (bs.mStateType == StateType.GameState)
        {
            s.Append("further progress in game state");
        }
        else if (bs.mStateType == StateType.InfoState)
        {
            s.Append("more player knowledge");
        }
        else if (bs.mStateType == StateType.UserVariable)
        {
            s.Append("user's state is different");
        }
        GUILayout.Label(s.ToString());
        GUILayout.BeginHorizontal();
        GUILayout.Label("Value:");
        valInput = bs.Value.ToString();
        valInput = GUILayout.TextField(valInput);
        int i;
        if (int.TryParse(valInput, out i))
        {
            bs.SetValue(i, false);
        }
        GUILayout.EndHorizontal();
    }
}