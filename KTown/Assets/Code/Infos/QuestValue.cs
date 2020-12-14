using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class QuestValue : MonoBehaviour
{
    public static List<QuestValue> ALL_QUEST_VALUES;
    public static void REFRESH_ALL() { foreach (var q in ALL_QUEST_VALUES) q.Refresh(); }

    public string HashKey = "QuestValue";
    [Header("Event index in this array corresposnds to its value on server")]
    public UnityEvent[] EventsOnValue;
    [Header("This event is called on every refresh")]
    public UnityEvent EventOnReset;

    void Awake()
    {
        EventOnReset.Invoke();
    }
    public void Refresh()
    {
        if (EventOnReset != null) EventOnReset.Invoke();
    }
    public void OnGetValueServerMessage(ServerMsg m)
    {
        if (m == null) return;
        if (m.Values.Count == 0) return;
        if (EventsOnValue.Length > m.Values[0])
        {
            if (EventsOnValue[m.Values[0]] != null) EventsOnValue[m.Values[0]].Invoke();
        }
    }
}