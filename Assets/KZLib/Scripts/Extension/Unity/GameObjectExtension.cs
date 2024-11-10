using System.Collections.Generic;
using KZLib;
using UnityEngine;

public static class GameObjectExtension
{
	public static void SetRenderOff(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		foreach(var renderer in _object.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = false;
		}
	}

	/// <summary>
	/// Check null & Set active
	/// </summary>
	public static void SetActiveSelf(this GameObject _object,bool _active)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		if(_object.activeSelf != _active)
		{
			_object.SetActive(_active);
		}
	}

	/// <summary>
	/// Toggle active
	/// </summary>
	public static void SetActiveToggle(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		_object.SetActive(!_object.activeSelf);
	}

	/// <summary>
	/// Set active all children
	/// </summary>
	public static void SetActiveAll(this GameObject _object,bool _active,bool _includeSelf)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		if(_includeSelf)
		{
			_object.SetActiveSelf(_active);
		}

		_object.transform.TraverseChildren((child)=> { child.gameObject.SetActiveSelf(_active); });
	}

	public static void SetAllLayer(this GameObject _object,int _layer)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		_object.layer = _layer;

		_object.transform.TraverseChildren((child)=> { child.gameObject.layer = _layer; });
	}

	public static void SetAllLayer(this GameObject _object,string _layerMask)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		SetAllLayer(_object,LayerMask.NameToLayer(_layerMask));
	}

	public static bool IsPrefab(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return false;
		}

		return !_object.scene.IsValid() && !_object.scene.isLoaded && _object.GetInstanceID() >= 0 && !_object.hideFlags.HasFlag(HideFlags.HideInHierarchy);
	}

	public static bool IsExistMeshFilter(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return false;
		}

		var filterArray = _object.GetComponentsInChildren<MeshFilter>(true);

		if(filterArray.Length <= 0)
		{
			return false;
		}

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
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return null;
		}

		return _object.GetComponent<TComponent>() ?? _object.AddComponent<TComponent>();
	}

	public static bool IsInComponent<TComponent>(this GameObject _object) where TComponent : Component
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return false;
		}

		return _object.GetComponent<TComponent>() is not null;
	}

	public static void ReAssignShader(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		foreach(var graphic in _object.GetComponentsInChildren<TMPro.TMP_Text>(true))
		{
			if(!graphic.fontMaterial)
			{
				continue;
			}

			graphic.fontMaterial.shader = ShaderMgr.In.GetShader(graphic.fontMaterial.shader.name);
		}

		foreach(var renderer in _object.GetComponentsInChildren<Renderer>(true))
		{
			if(renderer.materials == null)
			{
				continue;
			}

			for(var i=0;i<renderer.materials.Length;i++)
			{
				renderer.materials[i].shader = ShaderMgr.In.GetShader(renderer.materials[i].shader.name);
			}
		}
	}

	/// <summary>
	/// Check mesh flip
	/// </summary>
	public static void SkinMeshFlip(this GameObject _object)
	{
		if(!_object)
		{
			LogTag.System.E("GameObject is null");

			return;
		}

		var mesh = _object.GetComponent<SkinnedMeshRenderer>().sharedMesh;

		for(var i=0;i<mesh.vertices.Length;i++)
		{
			mesh.vertices[i] = new Vector3(mesh.vertices[i].x+100f,mesh.vertices[i].y,mesh.vertices[i].z);
		}

		var verticesList = new List<Vector3>();
		var normalList = new List<Vector3>();
		var triangleList = new List<int>();
		var uvList = new List<Vector2>();
		var tangentList = new List<Vector4>();
		var boneWeightList = new List<BoneWeight>();
		var bindposeList = new List<Matrix4x4>();

		for(var i=0;i<mesh.vertices.Length;i++)
		{
			verticesList.Add(mesh.vertices[i]);
			normalList.Add(mesh.normals[i]);
			uvList.Add(mesh.uv[i]);
			tangentList.Add(mesh.tangents[i]);
		}

		for(var i=mesh.triangles.Length-1;i>=0;i--)
		{
			triangleList.Add(mesh.triangles[i]);
		}

		for(var i=0;i<mesh.boneWeights.Length;i++)
		{
			boneWeightList.Add(mesh.boneWeights[i]);
		}

		for(var i=0;i<mesh.bindposes.Length;i++)
		{
			bindposeList.Add(mesh.bindposes[i]);
		}

		mesh.Clear();

		mesh.vertices = verticesList.ToArray();
		mesh.triangles = triangleList.ToArray();
		mesh.uv = uvList.ToArray();
		mesh.normals = normalList.ToArray();
		mesh.tangents = tangentList.ToArray();
		mesh.boneWeights = boneWeightList.ToArray();
		mesh.bindposes = bindposeList.ToArray();
	}
}