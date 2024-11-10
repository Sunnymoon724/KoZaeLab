using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static partial class CommonUtility
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
		foreach(var component in GetComponentGroupInActiveScene<TComponent>())
		{
			yield return component;
		}

		foreach(var prefab in LoadAssetGroup<GameObject>("t:prefab"))
		{
			foreach(var component in prefab.GetComponentsInChildren<TComponent>(true))
			{
				yield return component;
			}
		}
	}

	public static IEnumerable<TComponent> GetComponentGroupInActiveScene<TComponent>() where TComponent : Component
	{
		var scene = SceneManager.GetActiveScene();

		foreach(var root in scene.GetRootGameObjects())
		{
			foreach(var component in root.GetComponentsInChildren<TComponent>(true))
			{
				yield return component;
			}
		}
	}
#endif
}