#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
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
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        var newGroup1 = settings.FindGroup($"Localization-Asset-Tables-{SystemLanguage.Japanese}");

        // 새 그룹에서 BundledAssetGroupSchema 가져오기
        BundledAssetGroupSchema schema2 = newGroup1.GetSchema<BundledAssetGroupSchema>();

        if (schema2 != null)
        {
            LogTag.System.I($"{schema2.BundleNaming}");
        }

        if (schema2 != null)
        {
            schema2.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            // 필요 시 여기에 다른 설정도 바꿀 수 있음
        }

        AddressableAssetGroup newGroup = settings.CreateGroup(testString,false,true,false,newGroup1.Schemas);

        newGroup.AddSchema(schema2);

        // 새 그룹에서 BundledAssetGroupSchema 가져오기
        var schema = newGroup.GetSchema<BundledAssetGroupSchema>();

        if (schema != null)
        {
            LogTag.System.I($"{schema.BundleNaming}");
        }

        AssetDatabase.SaveAssets();
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