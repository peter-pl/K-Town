using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Locations;

public class Location : MonoBehaviour, IClickable
{
    public string LocationName = "Location";
    public string LocationDescr = "Description of location";
    public Lootable mLootable; //null if nothing to loot
    public UnityEvent PlayerEnterEvent;
    public bool IsScoutable = true, IsLootable = true;

    public bool IsAlreadyHacked { get; set; }

    static int HEAT_FOR_SECOND_HACK = 10;

    /*
    public int data { get; set; }
    public int money { get; set; }
    public int heat { get; set; }
    public List<string>*/

    void Awake()
    {
        if (PlayerPrefs.GetInt($"Hacked{LocationName}", 0) == 1)
        {
            Debug.Log($"Location {LocationName} was already hacked on previous application run!");
            IsAlreadyHacked = true;
        }
    }
    public void FinishedLooting()
    {
        Debug.Log($"User finished hacking {this}");
        RemoveListeners();
    }
    void AddListeners()
    {
        GameManager.INSTANCE.AddDataEvent.AddListener(LootedData);
        GameManager.INSTANCE.AddMoneyEvent.AddListener(LootedMoney);
        GameManager.INSTANCE.AddHeatEvent.AddListener(LootedHeat);
    }
    void RemoveListeners()
    {
        GameManager.INSTANCE.AddDataEvent.RemoveListener(LootedData);
        GameManager.INSTANCE.AddMoneyEvent.RemoveListener(LootedMoney);
        GameManager.INSTANCE.AddHeatEvent.RemoveListener(LootedHeat);
    }
    void LootedData(int i, string s)
    {

    }
    void LootedMoney(int i, string s)
    {

    }
    void LootedHeat(int i, string s)
    {

    }
    public ClickResponse OnClick()
    {
        Debug.Log($"{this} clicked");
        MapController.INSTANCE.TryGoTo(this);
        return ClickResponse.Click;
    }
    public bool OnPress(float f)
    {
        return false;
    }
    public bool OnScan()
    {
        CursorCtrl.CURSOR_HOVER_INFO_EVENT.Invoke(this, LocationName);
        return true;
    }
}

public interface IClickable
{
    bool OnScan();
    ClickResponse OnClick();
    bool OnPress(float f);
}

public interface IRefreshable
{
    void Refesh(MapController.VRMode mode);
}

public interface IPlayerTrigger
{
    bool OnPlayerTriggered();
}

public enum ClickResponse
{
    Click, Press, Nothing
}

public class EventLocation : UnityEvent<Location> { }