#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor-only utility methods for iterating components in scenes and prefabs.
/// </summary>
public static partial class KZEditorKit
{
	#region Component
	/// <summary>
	/// Invokes an action on every matching component in the active scene and in all prefab assets.
	/// </summary>
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

	/// <summary>
	/// Invokes an action on every matching component under the active scene's root objects.
	/// </summary>
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
