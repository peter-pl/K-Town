using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class MapController : MonoBehaviour
{
    public static MapController INSTANCE;
    public static bool IS_LOOTING { get { return INSTANCE.CurrentMode == VRMode.Loot; } }
    public static bool IS_SCOUTING { get { return INSTANCE.CurrentMode == VRMode.Scout; } }

    public Transform TeleportToHQ;
    public CharacterController ActionCharCtrl;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPS;
    public Location NowAtLocation;
    public VRMode CurrentMode { get; set; }
    public string LocationString { get { if (NowAtLocation == null) return "-"; else return NowAtLocation.LocationName; } }
    public double Lat, Lon;
    public float Speed = 5;
    public float LonMult = 2;
    public float ScaleMult = 10000;
    public EventLocation EnterLocationEvent = new EventLocation();
    public UnityEvent LeftLocationEvent = new UnityEvent();
    public UnityEvent EnterLocationEvent2 = new UnityEvent();
    public Timer mTimer = new Timer();
    public Icebreaker mIcebreaker = new Icebreaker();
    public float JumpWhenLooting = 0.5f, JumpWhenScouting = 5;

    public bool IsMoving { get; set; }

    double AtlusLat = 50.26165f, AtlusLon = 19.023299f;
    bool isAbleToMove = true;

    LTDescr activeMoveTween = null;

    public enum VRMode { None = 0, Scout, Detect, Loot }

    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            ActionCharCtrl.enabled = false;
        }
        else
        {
            enabled = false;
            Destroy(this);
        }
    }
    void OnDestroy()
    {
        if (INSTANCE == this) INSTANCE = null;
    }
    void Update()
    {
        Lat = AtlusLat + transform.position.x / ScaleMult;
        Lon = AtlusLon + transform.position.z / ScaleMult * LonMult;
        mTimer.Update();
        mIcebreaker.Update();
    }
    public void TryGoTo(Location l)
    {
        if (isAbleToMove)
        {
            if (activeMoveTween != null)
            {
                LeanTween.cancel(activeMoveTween.uniqueId);
                activeMoveTween = null;
            }
            IsMoving = true;
            NowAtLocation = null;
            LeftLocationEvent.Invoke();
            activeMoveTween = transform.LeanMove(l.transform.position, Vector3.Distance(transform.position, l.transform.position) / Speed).setOnComplete(() => ArrivedAt(l));
        }
    }
    public void ArrivedAt(Location l)
    {
        Debug.Log($"Player arrived at: {l}");
        IsMoving = false;
        NowAtLocation = null;
        NowAtLocation = l;
        activeMoveTween = null;
        if (l.PlayerEnterEvent != null) l.PlayerEnterEvent.Invoke();
        EnterLocationEvent.Invoke(l);
        EnterLocationEvent2.Invoke();
        GameManager.INSTANCE.SendUsersOwnData();
    }
    public void UserTryLoot()
    {
        if (NowAtLocation == null) return;
        if (NowAtLocation.mLootable == null) return;
        UserStartVR(NowAtLocation.mLootable.TeleportTo, VRMode.Loot, NowAtLocation.IsScoutable);
        GameManager.INSTANCE.StartLooting(NowAtLocation);
        NowAtLocation.IsAlreadyHacked = true;
        
    }
    public void UserTryScout()
    {
        if (NowAtLocation == null) return;
        if (NowAtLocation.mLootable == null) return;        
        UserStartVR(NowAtLocation.mLootable.TeleportTo, VRMode.Scout, false);
        NowAtLocation.IsAlreadyHacked = true;
    }
    public void UserTryDetect()
    {
        /*
        if (NowAtLocation == null) return;
        if (NowAtLocation.mLootable == null) return;
        if (GameManager.INSTANCE.EnergyPts > NowAtLocation.mLootable.DetectEnergyCost)
        {
            GameManager.INSTANCE.EnergyPts -= NowAtLocation.mLootable.DetectEnergyCost;
            UserStartVR(NowAtLocation.mLootable.TeleportTo, VRMode.Detect);
        }
        else Debug.LogWarning("Not enough energy!");*/
    }
    public void UserVisitHQ()
    {
        Debug.Log($"Starting HQ VR, teleport to: {TeleportToHQ}");
        InfoUnit.GET_ALL_INFOS();
        UserStartVR(TeleportToHQ, VRMode.None, false);
    }
    public void UserStartVR(Transform teleport, VRMode mode, bool isMajorLocation)
    {
        Debug.Log($"Starting VR, teleport to: {teleport}, mode: {mode}");
        CurrentMode = mode;
        ActionCharCtrl.enabled = false;
        ActionCharCtrl.transform.position = teleport.position;
        ActionCharCtrl.transform.rotation = teleport.rotation;
        ActionCharCtrl.enabled = true;
        CursorCtrl.SWITCH_TO_MAP_CAM.Invoke(false);
        foreach (var v in teleport.GetComponentsInChildren<IRefreshable>())
        {
            v.Refesh(mode);
        }
        if (mode == VRMode.Loot)
        {
            mTimer.Start(isMajorLocation);
            FPS.m_JumpSpeed = JumpWhenLooting;
        }
        else
        {
            FPS.m_JumpSpeed = JumpWhenScouting;
        }
    }
    public void SystemFinishVR()
    {
        Debug.Log($"System finish VR");
        UserFinishVR();
    }
    public void UserFinishVR()
    {
        Debug.Log($"Finishing VR while at: {NowAtLocation} in mode: {CurrentMode}");
        ActionCharCtrl.enabled = false;
        if (NowAtLocation && CurrentMode == VRMode.Loot)
        {
            PlayerPrefs.SetInt($"Looted{NowAtLocation.LocationName}", 1);
            //if (NowAtLocation.mLootable != null)
            //{
            //    if (NowAtLocation.mLootable.mInfoLoot != null) GameManager.INSTANCE.LocationWithInfoJustLooted(NowAtLocation.mLootable.mInfoLoot);
            //}
            //GameManager.INSTANCE.SendInfoOnLootedLocation(NowAtLocation);
        }
        CursorCtrl.SWITCH_TO_MAP_CAM.Invoke(true);
        CurrentMode = VRMode.None;
        mTimer.Stop();
        mIcebreaker.Stop();
    }
    public class Timer
    {
        float t = 0;
        bool isRunning = false;
        List<TimedAction> Actions = new List<TimedAction>();
        public void Start(bool isMajorLocation)
        {
            isRunning = true;
            Actions.Clear();
            if (isMajorLocation)
            {
                Actions.Add(TimedAction.IncreaseHeatOnMinute(5, 1, "Partial Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(6, 1, "Partial Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(7, 2, "Moderate Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(8, 3, "Significant Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(9, 4, "Major Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(10, 5, "Major Detection"));
                Actions.Add(new TimedAction(() => INSTANCE.SystemFinishVR(), 11, "Disconnect"));
            }
            else
            {
                Actions.Add(TimedAction.IncreaseHeatOnMinute(2, 1, "Partial Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(2.5f, 1, "Moderate Detection"));
                Actions.Add(TimedAction.IncreaseHeatOnMinute(3, 2, "Moderate Detection"));
                Actions.Add(new TimedAction(() => INSTANCE.SystemFinishVR(), 3.5f, "Disconnect"));
            }
        }
        public string GetUIInfo
        {
            get
            {
                if (Actions.Count == 0)
                {
                    return "";
                }
                else
                {
                    float f = Actions[0].mTime - t;
                    int minutes = Mathf.FloorToInt(f / 60);
                    f -= minutes * 60;
                    int seconds = Mathf.FloorToInt(f);
                    return $"{minutes}:{seconds} until {Actions[0].UIInfo}";
                }
            }
        }
        public void Stop()
        {
            t = 0;
            isRunning = false;
        }
        public void Update()
        {
            if (isRunning) t += Time.deltaTime;
            if (Actions.Count > 0)
            {
                if (t > Actions[0].mTime)
                {
                    Actions[0].mAction.Invoke();
                    Actions.RemoveAt(0);
                }
            }
        }
        public class TimedAction
        {
            public bool IsUsed = false;
            public Action mAction;
            public float mTime = 1;
            public string UIInfo;
            public TimedAction(Action a, float t, string info)
            {
                mAction = a;
                mTime = t;
                UIInfo = info;
            }
            public static TimedAction IncreaseHeatOnMinute(float t, int heat, string info)
            {
                return new TimedAction(() => GameManager.INSTANCE.AddHeatEvent.Invoke(heat, info), t * 60, info);
            }
        }
    }
    public class Icebreaker
    {
        IClickable client;
        Action onFinished;
        string uiDescr;
        float timeLeft = -1;

        public void Start(IClickable cl, Action a, string desc, float t)
        {
            if (cl == client) { Debug.Log($"Already breaking it!"); return; }
            client = null;
            client = cl;
            onFinished = null;
            onFinished = a;
            uiDescr = desc;
            timeLeft = t;
        }
        public void Update()
        {
            if (timeLeft >= 0)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0)
                {
                    client = null;
                    onFinished.Invoke();
                }
            }
        }
        public void Stop()
        {
            client = null;
            onFinished = null;
            uiDescr = "";
            timeLeft = -1;
        }
        public string GetUIInfo
        {
            get
            {
                if (timeLeft > 0)
                {
                    return $"Ice Breaker Engaged {uiDescr} est. time left: {timeLeft.ToString("f2")}";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}