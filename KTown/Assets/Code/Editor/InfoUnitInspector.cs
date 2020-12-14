using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetActiveToInfo))]
public class InfoUnitInspector : Editor
{
    int team;
    string[] teamStrs = { "0", "1", "2" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Get data from server")) GetData(target as InfoUnit);
        if (GUILayout.Button("Send to server")) SendData(target as InfoUnit);
        if (Application.isPlaying)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Team: ");
            team = GUILayout.Toolbar(team, teamStrs);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Put on Decrypt queue")) PutOnDecryptQueue(team, target as InfoUnit);
            GUILayout.EndHorizontal();
        }
    }
    void SendData(InfoUnit u)
    {
        Debug.Log("Sending data...");
        GameManager.INSTANCE.GetInfo(u.InfoName, (i) =>
        {
            Debug.Log($"Get info with result: {i}");
            if (i == null) { i = new Info(); i.KnownByUsers = new List<ulong>(); }
            i.InfoName = u.InfoName;
            if (u.IsReveled) i.Status = 1;
            else if (u.IsFaked) i.Status = 2;
            GameManager.INSTANCE.GenericSave(i);
        });
    }
    void GetData(InfoUnit u)
    {
        Debug.Log("Getting data...");
        GameManager.INSTANCE.GetInfo(u.InfoName, (i) =>
        {
            Debug.Log($"Get info with result: {i}");
            if (i == null) { Debug.Log($"No info: {u.InfoName} on server!"); return; }
            u.SetToStatus(i.Status);
        });
    }
    void PutOnDecryptQueue(int i, InfoUnit u)
    {
        Debug.Log("Putting on decrypt queue...");
    }
}