using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

public class TestCon : MonoBehaviour
{
    [SerializeField]
    private string m_ObjectPath = null;

    void Start()
    {
        LoadResourcesAsync().Forget();
    }

    private async UniTaskVoid LoadResourcesAsync()
    {
        // var size = await AddressablesMgr.In.GetDownloadAssetSizeAsync("test");

        // Log.Effect.I(size);

        await AddressablesMgr.In.LoadResourceAsync(new string[] { "default" },null);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ResMgr.In.GetObject(m_ObjectPath);
        }
    }
}