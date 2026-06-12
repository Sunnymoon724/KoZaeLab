#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class KZEditorKit
{
	#region Component
	public static void ExecuteMatchedComponentInUnity<TComponent>(System.Action<TComponent> onAction = null) where TComponent : Component
	{
		ExecuteMatchedComponentInActiveScene(onAction);

		bool _Execute(GameObject asset,int index,int totalCount)
		{
			var componentArray = asset.GetComponentsInChildren<TComponent>(true);

			for(var i=0;i<componentArray.Length;i++)
			{
				onAction?.Invoke(componentArray[i]);
			}

			return true;
		}

		KZAssetKit.ExecuteMatchedAsset<GameObject>("t:prefab",null,_Execute);
	}

	public static void ExecuteMatchedComponentInActiveScene<TComponent>(System.Action<TComponent> onAction = null) where TComponent : Component
	{
		var activeScene = SceneManager.GetActiveScene();
		var rootGameObjectArray = activeScene.GetRootGameObjects();

		for(var i=0;i<rootGameObjectArray.Length;i++)
		{
			var componentArray = rootGameObjectArray[i].GetComponentsInChildren<TComponent>(true);

			for(var j=0;j<componentArray.Length;j++)
			{
				onAction?.Invoke(componentArray[j]);
			}
		}
	}
	#endregion Component
}
#endif
