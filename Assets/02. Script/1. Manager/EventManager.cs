using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    private static Dictionary<string, Action<float>> floatEventDirctionary = new Dictionary<string, Action<float>>();
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();

    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent)) // 留뚯빟 eventName?덉뼱 thisEvent???ｌ뼱??諛섑솚, ?놁뼱 洹몃읆
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }

    }

    public static void StopListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Remove(eventName);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke();//?놁쓣 ???ㅽ뻾 x
        }
    }
}