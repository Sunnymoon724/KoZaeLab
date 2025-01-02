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

		foreach(var field in componentType.GetFields())
		{
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

			foreach(var component in asset.GetComponentsInChildren<TComponent>(true))
			{
				yield return component;
			}
		}
	}

	public static IEnumerable<TComponent> FindComponentGroupInActiveScene<TComponent>() where TComponent : Component
	{
		var activeScene = SceneManager.GetActiveScene();

		foreach(var rootGameObject in activeScene.GetRootGameObjects())
		{
			foreach(var component in rootGameObject.GetComponentsInChildren<TComponent>(true))
			{
				yield return component;
			}
		}
	}
#endif
}