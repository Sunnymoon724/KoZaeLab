using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventMultiTrigger : MonoBehaviour
{
    public bool IsLockEvent { get; set; } = false;
    private EventTrigger myTrigger = null;

    [SerializeField]
    private EventTriggerDataArrayDictionary eventTriggerDataDictionary = new EventTriggerDataArrayDictionary();

    void Awake()
    {
        if(myTrigger == null)
        {
            myTrigger = this.GetComponent<EventTrigger>();

            if(myTrigger == null)
            {
                myTrigger = gameObject.AddComponent<EventTrigger>();
            }
        }
    }

    void Start()
    {
        myTrigger.triggers.Clear();

        foreach(var dataArray in eventTriggerDataDictionary)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = dataArray.Key
            };

            entry.callback.AddListener((data)=>TriggerEvent(dataArray.Key));
            

            myTrigger.triggers.Add(entry);
        }
    }

    public void TriggerEvent(EventTriggerType _Type)
    {
        StartCoroutine(PlayTriggerEvent(_Type));
    }

    public IEnumerator PlayTriggerEvent(EventTriggerType _Type)
    {
        if(!IsLockEvent)
        {
            var sortedDataArray = eventTriggerDataDictionary[_Type].OrderBy(ob=>ob.startTime);
            float nowTime = 0.0f;

            foreach(var data in sortedDataArray)
            {
                if((data.startTime-nowTime) != 0.0f)
                {
                    yield return new WaitForSeconds((data.startTime-nowTime));
                }

                data.eventData.Invoke();

            }

            yield return null;
        }
        else
        {
            yield return null;
        }
    }

    #region Event Trigger Data

    [Serializable]
    private class EventTriggerData
    {
        public float startTime = 0.0f;
        public UnityEvent eventData = new UnityEvent();
    }

    [Serializable]
    private class EventTriggerDataArrayStorage : SerializableDictionary.Storage<EventTriggerData[]> { }

    [Serializable]
    private class EventTriggerDataArrayDictionary : SerializableDictionary<EventTriggerType,EventTriggerData[],EventTriggerDataArrayStorage> { }
    #endregion
}