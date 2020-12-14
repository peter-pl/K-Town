using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
public class InfoUnit : MonoBehaviour
{
    public static Dictionary<string, InfoUnit> ALL_INFOS = new Dictionary<string, InfoUnit>();
    public static void GET_ALL_INFOS() { Debug.Log("Loading all infos"); foreach (var i in ALL_INFOS) i.Value.ReadFromServer(); }

    public string InfoName = "Info";
    public string UserDescr = "Valuable Information";
    public string RevealedBy = "";
    public bool IKnowThis = false;
    public bool IsReveled = false;
    public bool IsFaked = false;
    public UnityEvent OnDecoded;

    protected virtual void Awake()
    {
        ALL_INFOS.Add(InfoName, this);
        if (PlayerPrefs.GetInt($"i{InfoName}", 0) == 1)
        {
            IKnowThis = true;
            OnDecoded.Invoke();
        }
        Refresh();
    }
    void ReadFromServer()
    {
        GameManager.INSTANCE.GetInfo(InfoName, OnInfoReadFromServer);
    }
    void OnInfoReadFromServer(Info i)
    {
        Debug.Log($"Read info from server on: {i.InfoName}");
        string s = "";
        if (GameManager.INSTANCE.LoginData.GetName(i.RevealedBy, out s))
        {
            RevealedBy = s;
        }
        SetToStatus(i.Status);
    }
    public void Decrypted()
    {
        IKnowThis = true;
        PlayerPrefs.SetInt($"i{InfoName}", 1);
        OnDecoded.Invoke();
        Debug.LogWarning($"Decrypted: {UserDescr}"); //msg to user
        Refresh();
    }
    public void SetToStatus(int status)
    {
        if (status == 1) IsReveled = true;
        if (status == 2) IsFaked = true;
        Refresh();
    }
    protected virtual void Refresh() { }
    public void DEV_Reveal()
    {
        Debug.LogWarning($"DEV TOOL: Revealing {this}");
        IsReveled = true;
        Refresh();
    }
}