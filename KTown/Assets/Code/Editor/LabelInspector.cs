using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Label))]
public class LabelInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Label l = target as Label;
        l.LabelText = GUILayout.TextArea(l.LabelText);
    }
}
