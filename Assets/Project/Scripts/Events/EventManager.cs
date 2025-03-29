using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("EventManager is not initialized in the scene.");
            }
            return _instance;
        }
    }


    // Dictionary to store event types and their listeners
    private Dictionary<EventType, Action> eventDictionary = new Dictionary<EventType, Action>();
    private Dictionary<EventType, Dictionary<string, Action<object>>> paramEventDictionary = 
        new Dictionary<EventType, Dictionary<string, Action<object>>>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate EventManager found. Destroying.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Subscribe to an event type
    public void Subscribe(EventType eventType, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary.Add(eventType, null);
        }
        
        eventDictionary[eventType] += listener;
        Debug.Log($"[EventManager] Subscribed to {eventType} by {listener.Method.DeclaringType}.{listener.Method.Name}");
    }

    // Subscribe to an event with parameters
    public void Subscribe<T>(EventType eventType, string id, Action<T> listener) where T : class
    {
        if (!paramEventDictionary.ContainsKey(eventType))
        {
            paramEventDictionary.Add(eventType, new Dictionary<string, Action<object>>());
        }
        
        if (!paramEventDictionary[eventType].ContainsKey(id))
        {
            paramEventDictionary[eventType].Add(id, null);
        }
        
        paramEventDictionary[eventType][id] += (obj) => listener(obj as T);
    }

    // Unsubscribe from an event
    public void Unsubscribe(EventType eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] -= listener;
        }
    }

    // Unsubscribe from an event with parameters
    public void Unsubscribe<T>(EventType eventType, string id, Action<T> listener) where T : class
    {
        if (paramEventDictionary.ContainsKey(eventType) && 
            paramEventDictionary[eventType].ContainsKey(id))
        {
            // This is a simplification, as you can't easily remove a specific delegate
            // For a complete solution, consider maintaining a list of actions instead
            paramEventDictionary[eventType].Remove(id);
        }
    }

    // Trigger an event
    public void TriggerEvent(EventType eventType)
    {
        Debug.Log($"[EventManager] Triggering event: {eventType}");
        if (eventDictionary.TryGetValue(eventType, out Action callback))
        {
            callback?.Invoke();
            Debug.Log($"[EventManager] Finished event: {eventType}");
        }
    }
    // Trigger an event with parameters
    public void TriggerEvent<T>(EventType eventType, string id, T param) where T : class
    {
        if (paramEventDictionary.TryGetValue(eventType, out var callbackDict) && 
            callbackDict.TryGetValue(id, out var callback))
        {
            callback?.Invoke(param);
            Debug.Log($"Event triggered: {eventType} with ID: {id}");
        }
    }

    // Clear all events when no longer needed
    public void ClearEvents()
    {
        eventDictionary.Clear();
        paramEventDictionary.Clear();
    }
}