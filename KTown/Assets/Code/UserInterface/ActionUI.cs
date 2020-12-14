using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    class ActionUI : MonoBehaviour
    {
        public Button BtDisconnect;
        public GameObject ScoutWindow;
        public Text ScoutText;
        public Text TimerText;
        public Text IceBreakerText;
        public Light PlayerHeatLamp; public float HeatLampFreq = .5f, HeatLampDuration = 3; Coroutine heatCor;
        public GameObject ConfirmDisconnectPanel;

        public static ClickableStringFloatEvent SCOUT_INFO_GATHERED = new ClickableStringFloatEvent();

        void Awake()
        {
            SCOUT_INFO_GATHERED.AddListener(OnScoutInfo);
            ScoutWindow.SetActive(false);
        }
        void Start()
        {
            CursorCtrl.SWITCH_TO_MAP_CAM.AddListener(OnSwitchToMapCam);
            BtDisconnect.onClick.AddListener(MapController.INSTANCE.UserFinishVR);
            gameObject.SetActive(false);
            GameManager.INSTANCE.AddHeatEvent.AddListener(OnHeatAdded);
        }
        void OnHeatAdded(int amt, string message)
        {
            Debug.LogWarning(message);
            if (heatCor != null) StopCoroutine(heatCor);
            heatCor = StartCoroutine(HeatLampRoutine(amt));
        }
        IEnumerator HeatLampRoutine(int amt)
        {
            float t = HeatLampDuration * HeatLampFreq * 2;
            while (t > 0)
            {
                PlayerHeatLamp.intensity = Mathf.PingPong(t, HeatLampFreq);
                t -= Time.deltaTime;
                yield return null;
            }
            PlayerHeatLamp.intensity = 0;
            heatCor = null;
        }
        void OnDisable()
        {
            if (heatCor != null) StopCoroutine(heatCor);
        }
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) ConfirmDisconnectPanel.SetActive(true);
            TimerText.text = MapController.INSTANCE.mTimer.GetUIInfo;
            IceBreakerText.text = MapController.INSTANCE.mIcebreaker.GetUIInfo;
        }
        void OnScoutInfo(IClickable c, string info, float time)
        {
            ScoutText.text = info;
            ScoutWindow.SetActive(true);
            ScoutWindow.LeanCancel();
            ScoutWindow.LeanDelayedCall(time, () => ScoutWindow.SetActive(false));
        }
        void OnSwitchToMapCam(bool b)
        {
            gameObject.SetActive(!b);
            ConfirmDisconnectPanel.SetActive(false);
        }
    }
}