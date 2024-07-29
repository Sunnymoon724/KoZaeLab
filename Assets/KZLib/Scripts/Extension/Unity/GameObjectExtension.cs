using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
	public static void SetRenderOff(this GameObject _object)
	{
		if(!_object)
		{
			return;
		}

		foreach(var renderer in _object.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = false;
		}
	}

	/// <summary>
	/// 나 자신이 켜져있는지 확인하고 켜고 끄는거.
	/// </summary>
	public static void SetActiveSelf(this GameObject _object,bool _active)
	{
		if(!_object)
		{
			return;
		}

		if(_object.activeSelf != _active)
		{
			_object.SetActive(_active);
		}
	}

	/// <summary>
	/// 자신의 액티브를 토글한다.
	/// </summary>
	public static void SetActiveToggle(this GameObject _object)
	{
		if(!_object)
		{
			return;
		}

		_object.SetActive(!_object.activeSelf);
	}

	/// <summary>
	/// 자식의 모든 액티브를 꺼버린다
	/// </summary>
	public static void SetActiveAll(this GameObject _object,bool _active,bool _includeSelf)
	{
		if(!_object)
		{
			return;
		}

		if(_includeSelf)
		{
			_object.SetActiveSelf(_active);
		}

		_object.transform.TraverseChildren((child)=> { child.gameObject.SetActiveSelf(_active); });
	}
	
	/// <summary>
	/// 자식 포함 모든 레이어를 변경한다.
	/// </summary>
	public static void SetAllLayer(this GameObject _object,int _layer)
	{
		if(!_object)
		{
			return;
		}

		_object.layer = _layer;

		_object.transform.TraverseChildren((child)=> { child.gameObject.layer = _layer; });
	}

	/// <summary>
	/// 자식 포함 모든 레이어를 변경한다.
	/// </summary>
	public static void SetAllLayer(this GameObject _object,string _layerMask)
	{
		if(!_object)
		{
			return;
		}

		SetAllLayer(_object,LayerMask.NameToLayer(_layerMask));
	}

	public static bool IsPrefab(this GameObject _object)
	{
		if(!_object)
		{
			return false;
		}

		return !_object.scene.IsValid() && !_object.scene.isLoaded && _object.GetInstanceID() >= 0 && !_object.hideFlags.HasFlag(HideFlags.HideInHierarchy);
	}

	public static bool IsExistMeshFilter(this GameObject _object)
	{
		var filterArray = _object.GetComponentsInChildren<MeshFilter>(true);

		for(var i=0;i<filterArray.Length;i++)
		{
			if(!filterArray[i].sharedMesh)
			{
				return false;
			}
		}

		return true;
	}

	public static TComponent GetOrAddComponent<TComponent>(this GameObject _object) where TComponent : Component
	{
		return _object.GetComponent<TComponent>() ?? _object.AddComponent<TComponent>();
	}

	public static void ReAssignShader2(this GameObject _object)
	{
		var cacheDict = new Dictionary<string,Shader>();

		foreach(var graphic in _object.GetComponentsInChildren<TMPro.TMP_Text>(true))
		{
			if(!graphic.fontMaterial)
			{
				continue;
			}

			var shaderName = graphic.fontMaterial.shader.name;

			if (!shaderCache.TryGetValue(shaderName, out Shader shader))
			{
				shader = Shader.Find(shaderName);
				shaderCache[shaderName] = shader;
			}

			graphic.fontMaterial.shader = shader;
		}

		foreach(var renderer in _object.GetComponentsInChildren<Renderer>(true))
		{
			if(renderer.materials == null)
			{
				continue;
			}

			for(var i = 0; i < renderer.materials.Length; i++)
			{
				var shaderName = renderer.materials[i].shader.name;

				if (!shaderCache.TryGetValue(shaderName, out Shader shader))
				{
					shader = Shader.Find(shaderName);
					shaderCache[shaderName] = shader;
				}

				renderer.materials[i].shader = shader;
			}
		}
	}
}