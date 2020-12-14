using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

class Raycaster : MonoBehaviour
{
    public Camera actionCam;
    public Camera mapCam;
    public float MaxDistMap = 300;
    public float MaxDistAction = 3;

    public string CurrentTarget;

    Camera currentCam { get; set; }
    int raycastMask;
    float maxDist = 300;
    Press mPress = null;
    CursorActivity mActivity;

    void Awake()
    {
        raycastMask = LayerMask.GetMask("Clickable");
        currentCam = mapCam;
        CursorCtrl.SWITCH_TO_MAP_CAM.AddListener(OnCamSwitch);
    }
    void OnCamSwitch(bool isMap)
    {
        if (isMap)
        {
            actionCam.enabled = false;
            mapCam.enabled = true;
            currentCam = mapCam;
            maxDist = MaxDistMap;
        }
        else
        {
            actionCam.enabled = true;
            mapCam.enabled = false;
            currentCam = actionCam;
            maxDist = MaxDistAction;
        }
    }
    void Update()
    {
        CursorActivity activity;
        Scan(out activity);
        CursorCtrl.CURSOR_ACTIVITY_EVENT.Invoke(activity);
    }
    void Scan(out CursorActivity activity)
    {
        CurrentTarget = "no target, cam: " + currentCam;
        bool click = Input.GetButton("Use");
        bool scannedSth = false;
        Ray ray;
        if (currentCam == mapCam) ray = mapCam.ScreenPointToRay(Input.mousePosition);
        else ray = new Ray(actionCam.transform.position, actionCam.transform.forward);
        var hits = Physics.RaycastAll(ray, maxDist, raycastMask);
        foreach (RaycastHit r in hits)
        {
            IClickable c = r.collider.GetComponent<IClickable>();
            if (c != null)
            {
                CurrentTarget = $"{c.ToString()} click: {click}";
                if (click)
                {
                    if (mPress != null)
                    {
                        mPress.Time += Time.deltaTime;
                        if (mPress.Target.OnPress(mPress.Time))
                        {
                            mPress = null;
                        }
                        activity = CursorActivity.pressing;
                        return;
                    }
                    else
                    {
                        ClickResponse cr = c.OnClick();
                        if (cr == ClickResponse.Press)
                        {
                            mPress = new Press(c);
                        }
                        else if (cr == ClickResponse.Click)
                        {
                            activity = CursorActivity.clicking;
                            return; //click accepted, no scanning any more down
                        }
                    }
                }
                else
                {
                    if (!scannedSth)
                    {
                        if (c.OnScan()) { scannedSth = true; } //don't keep scanning down, just the first item!
                    }
                    mPress = null;
                }
            }
        }
        CurrentTarget += $" Press: {mPress}";
        if (scannedSth) activity = CursorActivity.scanning;
        else activity = CursorActivity.nothing;
    }
}
public enum CursorActivity { nothing, scanning, clicking, pressing }
public class BoolEvent : UnityEvent<bool> { }
public class CursorActivityEvent : UnityEvent<CursorActivity> { }
class Press
{
    public IClickable Target;
    public float Time = 0;

    public Press(IClickable c)
    {
        Target = c;
    }
}