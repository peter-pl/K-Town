using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class CursorCtrl : MonoBehaviour
{
    public static ClickableStringEvent CURSOR_HOVER_INFO_EVENT = new ClickableStringEvent();
    public static BoolEvent SWITCH_TO_MAP_CAM = new BoolEvent();
    public static CursorActivityEvent CURSOR_ACTIVITY_EVENT = new CursorActivityEvent();

    public Text ActionText, MapText;
    public Image mImage;
    public Color ColNotPressing = Color.white, ColClick = Color.gray, ColPressing = Color.black;
    public float ScaleForActive = 2;

    string lastInfo;

    void Awake()
    {
        CURSOR_HOVER_INFO_EVENT.AddListener(OnHoverInfo);
        SWITCH_TO_MAP_CAM.AddListener(OnSwitchToMap);
        CURSOR_ACTIVITY_EVENT.AddListener(OnCursorActivity);
    }
    void OnHoverInfo(IClickable client, string info)
    {
        lastInfo = info;
    }
    void OnSwitchToMap(bool b)
    {
        if (b) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = b;
    }
    void OnCursorActivity(CursorActivity a)
    {
        if (a == CursorActivity.pressing) mImage.color = ColPressing;
        else if (a == CursorActivity.clicking) mImage.color = ColClick;
        else mImage.color = ColNotPressing;
        if (a == CursorActivity.nothing) mImage.transform.localScale = Vector3.one;
        else mImage.transform.localScale = Vector3.one * ScaleForActive;
    }
    void Update()
    {
        ActionText.text = lastInfo;
        MapText.text = lastInfo;
        lastInfo = "";
    }
}

public class ClickableStringEvent : UnityEvent<IClickable, string> { }
public class ClickableStringFloatEvent : UnityEvent<IClickable, string, float> { }
public class IntStringEvent : UnityEvent<int, string> { }