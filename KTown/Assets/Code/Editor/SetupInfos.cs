using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class SetupInfos : EditorWindow
{
    int team;
    List<InfoUnit> units = new List<InfoUnit>();
    int removeAt = -1;
    string[] teamStrs = { "0", "1", "2" };

    [MenuItem("KTown/Setup Infos Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SetupInfos));
    }

    void OnGUI()
    {
        removeAt = -1;
        GUILayout.Label("Setup Infos");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Team: ");
        team = GUILayout.Toolbar(team, teamStrs);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        for (int i = 0; i < units.Count; i++)
        {
            GUILayout.BeginHorizontal();
            units[i] = EditorGUILayout.ObjectField(units[i], typeof(InfoUnit), true) as InfoUnit;
            if (GUILayout.Button("X")) removeAt = i;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+")) units.Add(null);
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Send to server")) SendToServer();
        }
        if (removeAt > -1)
        {
            units.RemoveAt(removeAt);
        }
    }

    void SendToServer()
    {
        Dictionary<string, int> infos = new Dictionary<string, int>();
        foreach (var u in units)
        {
            int reveal = 0;
            if (u.IsReveled) reveal = 1;
            if (u != null)
            {
                infos.Add(u.InfoName, reveal);
            }
        }
        //GameManager.INSTANCE.AdminReplaceDecryptQueue(team, infos);
    }
}