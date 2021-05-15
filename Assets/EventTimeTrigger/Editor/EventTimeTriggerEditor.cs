using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(EventTimeTrigger),true)]
public class EventTimeTriggerEditor : EventTriggerEditor
{
    SerializedProperty triggerProperty = null;
    SerializedProperty triggerListProperty = null;
    EventTimeTrigger timeTrigger = null;

    string[] triggerType = null;

    protected override void OnEnable()
    {
        triggerType = System.Enum.GetNames(typeof(EventTriggerType));
        timeTrigger = (EventTimeTrigger) target;

        triggerProperty = serializedObject.FindProperty("triggers22");
        triggerListProperty = serializedObject.FindProperty("eventTriggerList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(triggerProperty);
        int size = triggerListProperty.arraySize;

        for(int i=0;i<size;i++)
        {
            var eventData = triggerListProperty.GetArrayElementAtIndex(i).objectReferenceValue;

            Debug.Log(eventData.GetType());
            
            //eventData.startTime = EditorGUILayout.FloatField("Time",eventData.startTime);

            EditorGUILayout.PropertyField(triggerListProperty.GetArrayElementAtIndex(i));
            EditorGUILayout.Space();
        }

        //base.OnInspectorGUI();
        if(GUILayout.Button("Generate Cubes"))
        {
            timeTrigger.triggers22.Add(new EventTrigger.Entry());

            var keyList = timeTrigger.eventTriggerList.GroupBy(gb=>gb.triggerType).Cast<string>().ToList();
            var indexText = triggerType.Except(keyList).First();
            var key = (EventTriggerType) System.Enum.Parse(typeof(EventTriggerType),indexText);

            timeTrigger.eventTriggerList.Add(new EventTriggerData());
        }
    }
}