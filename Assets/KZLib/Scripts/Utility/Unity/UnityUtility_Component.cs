using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



#if UNITY_EDITOR

using UnityEditor;

#endif

public static partial class UnityUtility
{
	public static void SetCanvasCullTransparentMeshOff(GameObject _panel)
	{
		var canvas = _panel.GetComponent<CanvasRenderer>();

		if(canvas)
		{
			canvas.cullTransparentMesh = false;
		}
	}

	public static TComponent CopyComponent<TComponent>(TComponent _source,GameObject _destination) where TComponent : Component
	{
		var type = _source.GetType();
		var data = _destination.AddComponent(type);

		foreach(var field in type.GetFields())
		{
			field.SetValue(data,field.GetValue(_source));
		}

		return data as TComponent;
	}

#if UNITY_EDITOR
	public static IEnumerable<TComponent> GetComponentGroupInInUnity<TComponent>() where TComponent : Component
	{
		var componentList = new List<TComponent>(GetComponentGroupInActiveScene<TComponent>());

		foreach(var prefab in LoadAssetGroup<GameObject>("t:prefab"))
		{
			componentList.AddRange(prefab.GetComponentsInChildren<TComponent>(true));
		}

		return componentList;
	}

	public static IEnumerable<TComponent> GetComponentGroupInActiveScene<TComponent>() where TComponent : Component
	{
		var scene = SceneManager.GetActiveScene();
		var componentList = new List<TComponent>();

		foreach(var data in scene.GetRootGameObjects())
		{
			componentList.AddRange(data.GetComponentsInChildren<TComponent>(true));
		}

		return componentList;
	}
#endif
}