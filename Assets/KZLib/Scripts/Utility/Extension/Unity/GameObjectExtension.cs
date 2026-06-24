using System.Collections.Generic;
using KZLib;
using TMPro;
using UnityEngine;

/// <summary>
/// Extension methods for <see cref="GameObject"/> hierarchy, layer, and component management.
/// </summary>
public static class GameObjectExtension
{
	/// <summary>
	/// Disables all <see cref="Renderer"/> components on this object and its children.
	/// </summary>
	public static void DisableRenderers(this GameObject gameObject)
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
	/// Sets active state only when it differs from the current <see cref="GameObject.activeSelf"/> value.
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
	/// Toggles <see cref="GameObject.activeSelf"/> to the opposite value.
	/// </summary>
	public static void ToggleActive(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		gameObject.SetActive(!gameObject.activeSelf);
	}

	/// <summary>
	/// Sets the active state on every child transform, optionally including this object.
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

	/// <summary>
	/// Assigns the given layer to this object and every child in the hierarchy.
	/// </summary>
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

	/// <summary>
	/// Assigns the given layer to this object and every child, resolving the layer by name.
	/// </summary>
	public static void SetAllLayer(this GameObject gameObject,string layerName)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		SetAllLayer(gameObject,LayerMask.NameToLayer(layerName));
	}

	/// <summary>
	/// Returns whether the object is an asset prefab rather than a scene instance.
	/// </summary>
	public static bool IsPrefab(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return false;
		}

		return !gameObject.scene.IsValid() && !gameObject.scene.isLoaded && gameObject.GetInstanceID() >= 0 && !gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
	}

	/// <summary>
	/// Returns whether every <see cref="MeshFilter"/> in the hierarchy has a non-null shared mesh.
	/// </summary>
	public static bool HasValidMeshFilters(this GameObject gameObject)
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

	/// <summary>
	/// Returns the component when present; otherwise adds and returns a new one.
	/// </summary>
	public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component
	{
		if(!_IsValid(gameObject))
		{
			return null;
		}

		return gameObject.GetComponent<TComponent>() ?? gameObject.AddComponent<TComponent>();
	}

	/// <summary>
	/// Returns whether the component is attached to this object.
	/// </summary>
	public static bool HasComponent<TComponent>(this GameObject gameObject) where TComponent : Component
	{
		if(!_IsValid(gameObject))
		{
			return false;
		}

		return gameObject.GetComponent<TComponent>();
	}

	/// <summary>
	/// Re-resolves shaders on all child renderers and TMP text materials via <see cref="ShaderManager"/>.
	/// Mutates shared material assets in place; all users of the same shared material are affected.
	/// </summary>
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

			//? By design: fontSharedMaterial avoids TMP fontMaterial instance copies
			var fontMaterial = textGraphic.fontSharedMaterial;

			if(!fontMaterial || !fontMaterial.shader)
			{
				continue;
			}

			fontMaterial.shader = ShaderManager.In.FindShader(fontMaterial.shader.name);
		}

		var rendererArray = gameObject.GetComponentsInChildren<Renderer>(true);

		for(var i=0;i<rendererArray.Length;i++)
		{
			//? By design: sharedMaterials avoids renderer.materials instance copies
			var sharedMaterialArray = rendererArray[i].sharedMaterials;

			if(sharedMaterialArray == null)
			{
				continue;
			}

			for(var j=0;j<sharedMaterialArray.Length;j++)
			{
				var material = sharedMaterialArray[j];

				if(!material || !material.shader)
				{
					continue;
				}

				material.shader = ShaderManager.In.FindShader(material.shader.name);
			}
		}
	}

	/// <summary>
	/// Disables transparent-mesh culling on the root <see cref="CanvasRenderer"/>.
	/// </summary>
	public static void DisableCanvasCullTransparentMesh(this GameObject gameObject)
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
	/// Mirrors the skinned mesh by offsetting vertices and reversing triangle winding order.
	/// </summary>
	public static void SkinnedMeshFlip(this GameObject gameObject)
	{
		if(!_IsValid(gameObject))
		{
			return;
		}

		var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

		if(!skinnedMeshRenderer)
		{
			LogChannel.Kit.E("SkinnedMeshRenderer is null.");

			return;
		}

		var mesh = skinnedMeshRenderer.sharedMesh;
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
		var uv = mesh.uv;

		for(var i=0;i<vertexArray.Length;i++)
		{
			verticesList.Add(vertexArray[i]);
			normalList.Add(normalArray[i]);
			uvList.Add(uv[i]);
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
			LogChannel.Kit.E("GameObject is null");

			return false;
		}

		return true;
	}
}