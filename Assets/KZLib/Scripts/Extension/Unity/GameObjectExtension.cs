using System.Collections.Generic;
using KZLib;
using TMPro;
using UnityEngine;

public static class GameObjectExtension
{
	public static void SetRenderOff(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		var rendererArray = gameObject.GetComponentsInChildren<Renderer>();

		for(var i=0;i<rendererArray.Length;i++)
		{
			rendererArray[i].enabled = false;
		}
	}

	/// <summary>
	/// gameObject.activeSelf != active => SetActive()
	/// </summary>
	public static void EnsureActive(this GameObject gameObject,bool value)
	{
		if(!_IsValid(gameObject))
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
		if(!_IsValid(gameObject))
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
		if(!_IsValid(gameObject))
		{
			return;
		}

		if(includeSelf)
		{
			gameObject.EnsureActive(value);
		}

		void _SetActive(Transform child)
		{
			child.gameObject.EnsureActive(value);
		}

		gameObject.transform.RecursiveChildren(_SetActive);
	}

	public static void SetAllLayer(this GameObject gameObject,int layer)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		gameObject.layer = layer;

		void _SetLayer(Transform child)
		{
			child.gameObject.layer = layer;
		}

		gameObject.transform.RecursiveChildren(_SetLayer);
	}

	public static void SetAllLayer(this GameObject gameObject,string layerName)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		SetAllLayer(gameObject,LayerMask.NameToLayer(layerName));
	}

	public static bool IsPrefab(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return false;
		}

		return !gameObject.scene.IsValid() && !gameObject.scene.isLoaded && gameObject.GetInstanceID() >= 0 && !gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
	}

	public static bool IsExistMeshFilter(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
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
		if(!_IsValid(gameObject))
		{
			return null;
		}

		return gameObject.GetComponent<TComponent>() ?? gameObject.AddComponent<TComponent>();
	}

	public static bool HasComponent<TComponent>(this GameObject gameObject) where TComponent : Component
	{
		if(!_IsValid(gameObject))
		{
			return false;
		}

		return gameObject.GetComponent<TComponent>();
	}

	public static void ReAssignShader(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}


		var textGraphicArray = gameObject.GetComponentsInChildren<TMP_Text>(true);

		for(var i=0;i<textGraphicArray.Length;i++)
		{
			var textGraphic = textGraphicArray[i];

			if(!textGraphic.fontMaterial)
			{
				continue;
			}

			textGraphic.fontMaterial.shader = ShaderManager.In.FindShader(textGraphic.fontMaterial.shader.name);
		}

		var rendererArray = gameObject.GetComponentsInChildren<Renderer>(true);

		for(var i=0;i<rendererArray.Length;i++)
		{
			var materialArray = rendererArray[i].materials;

			if(materialArray == null)
			{
				continue;
			}

			for(var j=0;i<materialArray.Length;j++)
			{
				materialArray[j].shader = ShaderManager.In.FindShader(materialArray[j].shader.name);
			}
		}
	}

	public static void SetCanvasCullTransparentMeshOff(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
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
		if(!_IsValid(gameObject))
		{
			return;
		}

		var mesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		var vertexArray = mesh.vertices;

		for(var i=0;i<vertexArray.Length;i++)
		{
			vertexArray[i] = new Vector3(vertexArray[i].x+100f,vertexArray[i].y,vertexArray[i].z);
		}

		var verticesList = new List<Vector3>();
		var normalList = new List<Vector3>();
		var triangleList = new List<int>();
		var uvList = new List<Vector2>();
		var tangentList = new List<Vector4>();
		var boneWeightList = new List<BoneWeight>();
		var bindposeList = new List<Matrix4x4>();
		
		var normalArray = mesh.normals;
		var tangentArray = mesh.tangents;

		for(var i=0;i<vertexArray.Length;i++)
		{
			verticesList.Add(vertexArray[i]);
			normalList.Add(normalArray[i]);
			uvList.Add(mesh.uv[i]);
			tangentList.Add(tangentArray[i]);
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

	private static bool _IsValid(GameObject gameObject)
	{
		if(!gameObject)
		{
			LogSvc.System.E("GameObject is null");

			return false;
		}

		return true;
	}
}