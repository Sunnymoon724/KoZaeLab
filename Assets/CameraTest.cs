using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraTest : MonoBehaviour
{
    [SerializeField,KZTag] 
    private string m_Test = null;

    [SerializeField] 
    private LayerMask m_Test2 = 0;
}