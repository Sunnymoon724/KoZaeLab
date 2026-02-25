using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static partial class CommonUtility
{
	public static TComponent CopyComponent<TComponent>(TComponent component,GameObject gameObject) where TComponent : Component
	{
		var componentType = component.GetType();
		var componentObject = gameObject.AddComponent(componentType);
		var fieldArray = componentType.GetFields();
		
		for(var i=0;i<fieldArray.Length;i++)
		{
			var field = fieldArray[i];

			field.SetValue(componentObject,field.GetValue(component));
		}

		return componentObject as TComponent;
	}

#if UNITY_EDITOR
	public static void ExecuteMatchedComponentInUnity<TComponent>(Action<TComponent> onAction = null) where TComponent : Component
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

	public static void ExecuteMatchedComponentInActiveScene<TComponent>(Action<TComponent> onAction = null) where TComponent : Component
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
#endif
}