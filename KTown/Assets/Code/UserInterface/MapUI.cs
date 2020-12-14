using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    class MapUI : MonoBehaviour
    {
        [Header("Location")]
        public Button BtProbe;
        public Button BtLoot, BtScout; //, BtDetect;
        public Text TextLocName, TextLocDesc, TextProbeDesc;
        public GameObject PanelLocation, PanelProbe;
        public string ProbeGood = "Entry point located", ProbeNothing = "No target", ProbeBad = "No entry point found", ProbeAlreadyHacked = "Entry point located. Deck signature compromised! You will be detected hacking it again.";
        [Header("HQ")]
        public Button BtVisitHQ;

        void Start()
        {
            BtLoot.onClick.AddListener(MapController.INSTANCE.UserTryLoot);
            //BtDetect.onClick.AddListener(MapController.INSTANCE.UserTryDetect);
            BtScout.onClick.AddListener(MapController.INSTANCE.UserTryScout);
            BtProbe.onClick.AddListener(Probe);
            BtVisitHQ.onClick.AddListener(MapController.INSTANCE.UserVisitHQ);
            MapController.INSTANCE.EnterLocationEvent.AddListener(OnArrivedOnLocation);
            MapController.INSTANCE.LeftLocationEvent.AddListener(OnLeftLocation);
            PanelLocation.SetActive(false);
            PanelProbe.SetActive(false);
            CursorCtrl.SWITCH_TO_MAP_CAM.AddListener(OnSwitchToMapCam);
        }
        void OnSwitchToMapCam(bool b)
        {
            gameObject.SetActive(b);
            PanelProbe.SetActive(false);
        }
        void OnArrivedOnLocation(Location l)
        {
            PanelLocation.SetActive(true);
            PanelProbe.SetActive(false);
            TextLocName.text = l.LocationName;
            TextLocDesc.text = l.LocationDescr;
        }
        void OnLeftLocation()
        {
            PanelLocation.SetActive(false);
            PanelProbe.SetActive(false);
        }
        void Probe()
        {
            BtLoot.interactable = false;
            BtScout.interactable = false;
            //BtDetect.interactable = false;
            PanelProbe.SetActive(true);
            Location l = MapController.INSTANCE.NowAtLocation;
            if (l != null)
            {
                if (l.IsAlreadyHacked) TextProbeDesc.text = ProbeAlreadyHacked;
                else
                {
                    if (l.IsLootable || l.IsScoutable)
                    {
                        TextProbeDesc.text = ProbeGood;
                    }
                    else
                    {
                        TextProbeDesc.text = ProbeNothing;
                    }
                }
                if (l.IsLootable) BtLoot.interactable = true;
                if (l.IsScoutable) BtScout.interactable = true;
            }
            else
            {
                TextProbeDesc.text = ProbeBad;
            }
        }
    }
}