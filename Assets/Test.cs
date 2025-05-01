#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using ExcelDataReader.Log;
using KZLib;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Test : MonoBehaviour
{
    [SerializeField] private string testString = "Test";

    [Button("Test1")]
    void Text1()
    {
        TextAsync().Forget();
    }

    private async UniTaskVoid TextAsync()
    {
        await LocalizationSettings.InitializationOperation.Task;

        var handle = LocalizationSettings.StringDatabase.GetAllTables();

        await handle.Task;

        if (handle.Result == null || handle.Result.Count == 0)
        {
            Debug.LogError("No tables found.");
            return;
        }

        foreach (var table in handle.Result)
        {
            Debug.Log("Table Name: " + table.TableCollectionName);
        }
    }

    [Button("Test2")]
    void Text2()
    {
        var route = RouteMgr.In.GetOrCreateRoute("defaultRes:lingo");

        LogTag.Build.I(route.LocalPath);
        LogTag.Build.I(route.Extension);
        LogTag.Build.I(route.AbsolutePath);

        var route3 = RouteMgr.In.GetOrCreateRoute("defaultRes:lingo:test.txt");

        LogTag.Build.I(route3.LocalPath);
        LogTag.Build.I(route3.Extension);
        LogTag.Build.I(route3.AbsolutePath);


        var route2 = RouteMgr.In.GetOrCreateRoute("Assets\\Test.cs");

        LogTag.Build.I(route2.LocalPath);
        LogTag.Build.I(route2.Extension);
        LogTag.Build.I(route2.AbsolutePath);
    }

    [Button("Test3")]
    void Text3()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        var japaneseGroup = settings.FindGroup($"Localization-Asset-Tables-{SystemLanguage.Japanese}");

        var newGroup = CommonUtility.CreateAddressableGroup(testString,japaneseGroup.Schemas,true);

        var namingSchema = newGroup.GetSchema<BundledAssetGroupSchema>();

        if (namingSchema != null)
        {
            namingSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
        }

        // newGroup.AddSchema(namingSchema);
    }
}
#endif