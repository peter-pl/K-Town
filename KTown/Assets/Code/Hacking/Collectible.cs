using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Hacking
{
    [RequireComponent(typeof(Collider))]
    class Collectible : Interactable, IRefreshable
    {
        static float TIME_TO_RETRY_SCOUT = 5;
        
        public int MoneyValue, DataValue, HeatValue;
        public InfoUnit LootableInfo = null;
        public UnityEvent CollectEvent = new UnityEvent();
        public UnityEvent RefreshEvent = new UnityEvent();
        public bool IsScoutable = true;
        public float ScoutChance = 0.9f;

        bool available = true;

        protected override void Fire()
        {
            if (available)
            {
                if (MapController.IS_LOOTING)
                {
                    if (MoneyValue > 0) GameManager.INSTANCE.AddMoneyEvent.Invoke(MoneyValue, UIInfo);
                    if (DataValue > 0) GameManager.INSTANCE.AddDataEvent.Invoke(DataValue, UIInfo);
                    if (HeatValue > 0) GameManager.INSTANCE.AddHeatEvent.Invoke(HeatValue, $"Detector routine in {UIInfo}");
                    //if (LootableInfo != null) GameManager.INSTANCE.CollectibleWithInfoLooted(LootableInfo);
                    available = false;
                    CollectEvent.Invoke();
                }
                else if (MapController.IS_SCOUTING)
                {
                    if (IsScoutable)
                    {
                        available = false;
                        LeanTween.delayedCall(TIME_TO_RETRY_SCOUT, () => available = true);
                        SendScoutMsg();
                    }
                }
            }
        }
        public virtual void Refesh(MapController.VRMode mode)
        {
            available = true;
            RefreshEvent.Invoke();
        }
        public void SendScoutMsg()
        {
            string info;
            float time;
            if (IsScoutable)
            {
                if (UnityEngine.Random.value < ScoutChance)
                {
                    StringBuilder sb = new StringBuilder("Scouting results:");
                    if (HeatValue > 0) sb.Append($"\nIncludes detector routine!");
                    if (DataValue > 0) sb.Append($"\nPossible encrypted confidential information.");
                    if (MoneyValue > 0) sb.Append($"\nEncrypted credit retrival passcodes present.");
                    info = sb.ToString();
                    time = 5;
                }
                else
                {
                    info = "Scouting failed";
                    time = 2;
                }
            }
            else
            {
                info = "Can't scout data bank structures";
                time = 2;
            }
            UserInterface.ActionUI.SCOUT_INFO_GATHERED.Invoke(this, info, time);
        }
    }
}