using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test")]
    void Text()
    {
        for(int i=0;i<100;i++)
        {
            LogTag.Build.I($"{CryptoUtility.SHA.ComputeHashToString(wasdwe(new SquadParam(5,2)))}");
        }

        
        
        // LogTag.Build.I($"{CryptoUtility.SHA.ComputeHashToString(wasdwe(new SquadParam(5,2)))}");
        // LogTag.Build.I($"{CryptoUtility.SHA.ComputeHashToString(wasdwe(new SquadParam(5,2)))}");
        // LogTag.Build.I($"{CryptoUtility.SHA.ComputeHashToString(wasdwe(new SquadParam(5,2)))}");
    }

    private string wasdwe(object aa)
    {
        return aa.ToString();
    }
}

public record SquadParam
{
    public int AAA { get; set; }
    public int BBB { get; set; }

    public SquadParam(int aaa,int bbb)
    {
        AAA = aaa;
        BBB = bbb;
    }
}
