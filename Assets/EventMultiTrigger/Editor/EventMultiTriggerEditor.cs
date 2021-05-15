using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventMultiTrigger))]
public class EventMultiTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("This Script Creates Event Trigger And Erases All In Event Trigger.");
        EditorGUILayout.LabelField("If there is one, Event Trigger Is Not Working.");
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}