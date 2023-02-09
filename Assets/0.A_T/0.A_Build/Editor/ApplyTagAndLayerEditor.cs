using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public class ApplyTagAndLayerEditor
{
    static ApplyTagAndLayerEditor()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectGUI;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnProjectGUI(string guid, Rect selectionRect)
    {
        if (Selection.gameObjects.Length == 0)
        {
            return;
        }

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            if (PrefabUtility.GetPrefabAssetType(Selection.gameObjects[i]) != PrefabAssetType.Regular)
            {
                return;
            }
        }

        Event evt = Event.current;
        //Rect contextRect = new Rect(10, 10, 100, 100);

        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
                 && Event.current.button == 1 && Event.current.type <= EventType.MouseUp)
        {
            GenericMenu menu = new GenericMenu();

            if(Event.current.shift && Event.current.control == false)
            {
                foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
                {
                    menu.AddItem(new GUIContent(tag), false, new GenericMenu.MenuFunction2(MenuTagArg1), tag);
                }

                menu.ShowAsContext();
                evt.Use();
            }
            else if(Event.current.shift && Event.current.control)
            {
                foreach (string layer in UnityEditorInternal.InternalEditorUtility.layers)
                {
                    menu.AddItem(new GUIContent(layer), false, new GenericMenu.MenuFunction2(MenuLayerArg1), layer);
                }

                menu.ShowAsContext();
                evt.Use();
            }
        }
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (Selection.gameObjects.Length == 0)
        {
            return;
        }
               
        Event evt = Event.current;
        //Rect contextRect = new Rect(10, 10, 100, 100);

        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
                 && Event.current.button == 1 && Event.current.type <= EventType.MouseUp)
        {
            GenericMenu menu = new GenericMenu();

            if (Event.current.shift && Event.current.control == false)
            {
                foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
                {
                    menu.AddItem(new GUIContent(tag), false, new GenericMenu.MenuFunction2(MenuTagArg1), tag);
                }

                menu.ShowAsContext();
                evt.Use();
            }
            else if (Event.current.shift && Event.current.control)
            {
                foreach (string layer in UnityEditorInternal.InternalEditorUtility.layers)
                {
                    menu.AddItem(new GUIContent(layer), false, new GenericMenu.MenuFunction2(MenuLayerArg1), layer);
                }

                menu.ShowAsContext();
                evt.Use();
            }
        }
    }

    static void MenuTagArg1(object obj)
    {
        Debug.Log(obj);

        string tag = (string)obj;

        if(string.IsNullOrEmpty(tag))
        {
            UnityEngine.Debug.LogError("ApplyTagAndLayerEditor has Error !!");
        }

        //ChangeShaders((Shader)obj);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Selection.gameObjects[i].tag = tag;
        }
    }

    static void MenuLayerArg1(object obj)
    {
        Debug.Log(obj);

        string layer = (string)obj;

        if (string.IsNullOrEmpty(layer))
        {
            UnityEngine.Debug.LogError("ApplyTagAndLayerEditor has Error !!");
        }

        //ChangeShaders((Shader)obj);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Selection.gameObjects[i].layer = LayerMask.NameToLayer(layer);
        }
    }
}