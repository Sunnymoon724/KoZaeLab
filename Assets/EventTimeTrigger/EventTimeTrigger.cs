using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.EventSystems
{
    public class EventTimeTrigger : EventTrigger
    {
        public List<Entry> triggers22 = new List<Entry>();
        public List<EventTriggerData> eventTriggerList = new List<EventTriggerData>();
    }

    [Serializable]
    public class EventTriggerData : ScriptableObject
    {
        public EventTriggerType triggerType = EventTriggerType.PointerEnter;
        public float startTime = 0.0f;
        public UnityEvent eventData = new UnityEvent();
    }
}