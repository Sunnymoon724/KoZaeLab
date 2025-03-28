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
        var code = CryptoUtility.SHA.ComputeHashToBytes(SystemInfo.deviceUniqueIdentifier);

        var text = "textKey";

        var encrypt = CryptoUtility.AES.EncryptToString(text,code);

        LogTag.Build.I($"{encrypt}");

        var decrypt1 = CryptoUtility.AES.DecryptFromString(encrypt,code);

        LogTag.Build.I($"{decrypt1}");

        var decrypt2 = CryptoUtility.AES.DecryptFromString(text,code);

        LogTag.Build.I($"{decrypt2}");

        LogTag.Build.I($"End");
    }
}
