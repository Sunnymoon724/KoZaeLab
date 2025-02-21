using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test")]
    void Text()
    {
        var assembly = Assembly.GetAssembly(typeof(MonoBehaviour));

        LogTag.Build.I($"{assembly.FullName}");

        var assembly2 = Assembly.GetExecutingAssembly();

        LogTag.Build.I($"{assembly2.FullName}");

        var assembly3 =  Assembly.Load("Assembly-CSharp");

        LogTag.Build.I($"{assembly3.FullName}");
    }
}
