using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        Start2(2);
    }

    private void Start2(int _aaa)
    {
        LogTag.Test.E("test");

        Debug.LogError("test");

        throw new Exception("test");
    }
}