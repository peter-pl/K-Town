using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
class GMInspector : Editor
{
    float timeScale = 1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        timeScale = GUILayout.HorizontalSlider(timeScale, 1, 20);
        if (Application.isPlaying)
        {
            Time.timeScale = timeScale;
        }
        GUILayout.Space(20);
    }

}