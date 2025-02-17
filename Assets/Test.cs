using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test")]
    void Text()
    {
        var folderPath = "Resources/Text/Proto";

        var data = Resources.LoadAll<TextAsset>(CommonUtility.RemoveHeaderInPath(folderPath,"Resources"));

        LogTag.Build.I(CommonUtility.RemoveHeaderInPath(folderPath,"Resources"));
        LogTag.Build.I(data.Length);
    }
}
