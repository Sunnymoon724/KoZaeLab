using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnEnable()
    {
        Broadcaster.EnableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
    }

    private void OnDisable()
    {
        Broadcaster.DisableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
    }

    private void OnSetLocalizeText()
    {
        LogTag.Test.I("test");
    }
}
