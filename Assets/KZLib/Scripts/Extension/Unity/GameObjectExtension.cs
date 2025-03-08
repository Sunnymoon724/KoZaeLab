using System.Collections.Generic;
using KZLib;
using UnityEngine;

public static class GameObjectExtension
{
	public static void SetRenderOff(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = false;
		}
	}

	/// <summary>
	/// gameObject.activeSelf != active => SetActive()
	/// </summary>
	public static void EnsureActive(this GameObject gameObject,bool value)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		if(gameObject.activeSelf != value)
		{
			gameObject.SetActive(value);
		}
	}

	/// <summary>
	/// Toggle active
	/// </summary>
	public static void SetActiveToggle(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		gameObject.SetActive(!gameObject.activeSelf);
	}

	/// <summary>
	/// Set active all children
	/// </summary>
	public static void SetActiveAll(this GameObject gameObject,bool value,bool includeSelf)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		if(includeSelf)
		{
			gameObject.EnsureActive(value);
		}

		gameObject.transform.TraverseChildren((child)=> { child.gameObject.EnsureActive(value); });
	}

	public static void SetAllLayer(this GameObject gameObject,int layer)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		gameObject.layer = layer;

		gameObject.transform.TraverseChildren((child)=> { child.gameObject.layer = layer; });
	}

	public static void SetAllLayer(this GameObject gameObject,string layerName)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		SetAllLayer(gameObject,LayerMask.NameToLayer(layerName));
	}

	public static bool IsPrefab(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return false;
		}

		return !gameObject.scene.IsValid() && !gameObject.scene.isLoaded && gameObject.GetInstanceID() >= 0 && !gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
	}

	public static bool IsExistMeshFilter(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return false;
		}

		var filterArray = gameObject.GetComponentsInChildren<MeshFilter>(true);

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

	public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component
	{
		if(!IsValid(gameObject))
		{
			return null;
		}

		return gameObject.GetComponent<TComponent>() ?? gameObject.AddComponent<TComponent>();
	}

	public static bool IsInComponent<TComponent>(this GameObject gameObject) where TComponent : Component
	{
		if(!IsValid(gameObject))
		{
			return false;
		}

		return gameObject.GetComponent<TComponent>();
	}

	public static void ReAssignShader(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return;
		}


		foreach(var graphic in gameObject.GetComponentsInChildren<TMPro.TMP_Text>(true))
		{
			if(!graphic.fontMaterial)
			{
				continue;
			}

			graphic.fontMaterial.shader = ShaderMgr.In.FindShader(graphic.fontMaterial.shader.name);
		}

		foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
		{
			if(renderer.materials == null)
			{
				continue;
			}

			for(var i=0;i<renderer.materials.Length;i++)
			{
				renderer.materials[i].shader = ShaderMgr.In.FindShader(renderer.materials[i].shader.name);
			}
		}
	}

	public static void SetCanvasCullTransparentMeshOff(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		var canvas = gameObject.GetComponent<CanvasRenderer>();

		if(canvas)
		{
			canvas.cullTransparentMesh = false;
		}
	}

	/// <summary>
	/// Check mesh flip
	/// </summary>
	public static void SkinMeshFlip(this GameObject gameObject)
	{
		if(!IsValid(gameObject))
		{
			return;
		}

		var mesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;

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

	private static bool IsValid(GameObject gameObject)
	{
		if(!gameObject)
		{
			LogTag.System.E("GameObject is null");

			return false;
		}

		return true;
	}
}