#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using ExcelDataReader.Log;
using KZLib;
using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Test : MonoBehaviour
{
    [Button("Test1")]
    void Text1()
    {
        var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

        LogTag.Build.I(optionCfg.MasterVolume.level);

        optionCfg.SetMasterVolume(0.27f);

        LogTag.Build.I(optionCfg.MasterVolume.level);
    }

    private async UniTask Test2()
    {
        var ww = await PlayFabMgr.In.ExecuteCloudScriptAsync("Test",new { Test = "Test" });
    }
}
#endif