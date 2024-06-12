using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

public class AddressableTestCon : MonoBehaviour
{
    private void Start()
    {
        AddressablesMgr.In.LoadResourceAsync(new string[] { "default" },null).Forget();
    }

    public void OnCreateCapsule()
    {
        CreateObject("Assets/GameResources/Prefabs/Capsule.prefab").Forget();
    }

    public void OnCreateCylinder()
    {
        CreateObject("Assets/GameResources/Prefabs/Cylinder.prefab").Forget();
    }

    private async UniTaskVoid CreateObject(string _filePath)
    {
        var data = ResMgr.In.GetObject(_filePath);

        CommonUtility.ReAssignShader(data);

        await UniTask.WaitForSeconds(2.0f);

        CommonUtility.DestroyObject(data);
    }
}
