using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;

#endif

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
	public static IEnumerable<TComponent> FindComponentGroupInInUnity<TComponent>() where TComponent : Component
	{
		foreach(var component in FindComponentGroupInActiveScene<TComponent>())
		{
			yield return component;
		}

		foreach(var assetPath in FindAssetPathGroup("t:prefab"))
		{
			var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			var componentArray = asset.GetComponentsInChildren<TComponent>(true);

			for(var i=0;i<componentArray.Length;i++)
			{
				yield return componentArray[i];
			}
		}
	}

	public static IEnumerable<TComponent> FindComponentGroupInActiveScene<TComponent>() where TComponent : Component
	{
		var activeScene = SceneManager.GetActiveScene();
		
		var rootGameObjectArray = activeScene.GetRootGameObjects();
		
		for(var i=0;i<rootGameObjectArray.Length;i++)
		{
			var componentArray = rootGameObjectArray[i].GetComponentsInChildren<TComponent>(true);

			for(var j=0;j<componentArray.Length;j++)
			{
				yield return componentArray[j];
			}
		}
	}
#endif
}