using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combines multiple meshes into one runtime <see cref="Mesh"/> on this <see cref="MeshFilter"/>.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><description><see cref="MeshFilter"/> sources are transformed into this object's local space and disabled after combine.</description></item>
/// <item><description><see cref="Mesh"/> assets are combined at identity (mesh-local space).</description></item>
/// <item><description>Sub-meshes are merged when all source materials match; otherwise each source keeps its own sub-mesh / material slot.</description></item>
/// <item><description>Runtime meshes created here are destroyed in <see cref="_Release"/>.</description></item>
/// </list>
/// </remarks>
[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : BaseMesh
{
	/// <summary>True when <see cref="m_meshFilter.sharedMesh"/> was allocated by this component and should be destroyed on release.</summary>
	private bool m_ownsRuntimeMesh = false;

	protected override void _Release()
	{
		_ReleaseOwnedMesh();

		base._Release();
	}

	/// <summary>Replaces this filter's mesh with a combine of <paramref name="meshFilterArray"/>.</summary>
	public void CombineMeshFilter(MeshFilter[] meshFilterArray)
	{
		if(!_TryBuildMeshFilterInstances(meshFilterArray,out var instanceList,out var materialList,out var indexCount))
		{
			return;
		}

		if(!_IsValidMeshIndexCount(indexCount))
		{
			return;
		}

		_DisableMeshFilters(meshFilterArray);
		_SetCombinedMesh(instanceList,materialList,indexCount);
	}

	/// <summary>Replaces this filter's mesh with a combine of mesh assets at identity transform. Materials are left unchanged.</summary>
	public void CombineMeshes(Mesh[] meshArray)
	{
		if(!_TryBuildMeshInstances(meshArray,out var instanceList,out var indexCount))
		{
			return;
		}

		if(!_IsValidMeshIndexCount(indexCount))
		{
			return;
		}

		_SetCombinedMesh(instanceList,null,indexCount);
	}

	/// <summary>Appends <paramref name="meshFilterArray"/> to the current mesh. Returns false on invalid input or index overflow.</summary>
	public bool AppendMesh(params MeshFilter[] meshFilterArray)
	{
		if(meshFilterArray == null || meshFilterArray.Length == 0)
		{
			return false;
		}

		if(m_meshFilter.sharedMesh == null)
		{
			if(meshFilterArray.Length == 1 && _TryGetValidMeshFilter(meshFilterArray[0],out var singleMeshFilter))
			{
				m_meshFilter.sharedMesh = _DuplicateMesh(singleMeshFilter.sharedMesh);
				m_meshFilter.sharedMesh.name = m_meshFilter.gameObject.name;
				m_ownsRuntimeMesh = true;

				var sourceRenderer = singleMeshFilter.GetComponent<MeshRenderer>();

				if(sourceRenderer)
				{
					m_meshRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
				}

				singleMeshFilter.gameObject.SetActive(false);

				return true;
			}

			if(!_TryBuildMeshFilterInstances(meshFilterArray,out var initialInstanceList,out var initialMaterialList,out var initialIndexCount))
			{
				return false;
			}

			if(!_IsValidMeshIndexCount(initialIndexCount))
			{
				return false;
			}

			_DisableMeshFilters(meshFilterArray);
			_SetCombinedMesh(initialInstanceList,initialMaterialList,initialIndexCount);

			return true;
		}

		if(!_TryBuildMeshFilterInstances(meshFilterArray,out var appendInstanceList,out var appendMaterialList,out var appendIndexCount))
		{
			return false;
		}

		var indexCount = CalculateMeshIndexCount(m_meshFilter.sharedMesh)+appendIndexCount;

		if(!_IsValidMeshIndexCount(indexCount))
		{
			return false;
		}

		var instanceList = new List<CombineInstance>
		{
			new() { mesh = m_meshFilter.sharedMesh, transform = Matrix4x4.identity },
		};

		instanceList.AddRange(appendInstanceList);

		var materialList = new List<Material>(m_meshRenderer.sharedMaterials);
		materialList.AddRange(appendMaterialList);

		_DisableMeshFilters(meshFilterArray);
		_SetCombinedMesh(instanceList,materialList,indexCount);

		return true;
	}

	/// <summary>Appends mesh assets to the current mesh. Returns false on invalid input or index overflow.</summary>
	public bool AppendMesh(params Mesh[] meshArray)
	{
		if(meshArray == null || meshArray.Length == 0)
		{
			return false;
		}

		if(m_meshFilter.sharedMesh == null)
		{
			if(meshArray.Length == 1 && meshArray[0] != null)
			{
				m_meshFilter.sharedMesh = _DuplicateMesh(meshArray[0]);
				m_meshFilter.sharedMesh.name = m_meshFilter.gameObject.name;
				m_ownsRuntimeMesh = true;

				return true;
			}

			if(!_TryBuildMeshInstances(meshArray,out var initialInstanceList,out var initialIndexCount))
			{
				return false;
			}

			if(!_IsValidMeshIndexCount(initialIndexCount))
			{
				return false;
			}

			_SetCombinedMesh(initialInstanceList,null,initialIndexCount);

			return true;
		}

		if(!_TryBuildMeshInstances(meshArray,out var appendInstanceList,out var appendIndexCount))
		{
			return false;
		}

		var indexCount = CalculateMeshIndexCount(m_meshFilter.sharedMesh)+appendIndexCount;

		if(!_IsValidMeshIndexCount(indexCount))
		{
			return false;
		}

		var instanceList = new List<CombineInstance>
		{
			new() { mesh = m_meshFilter.sharedMesh, transform = Matrix4x4.identity },
		};

		instanceList.AddRange(appendInstanceList);
		_SetCombinedMesh(instanceList,null,indexCount);

		return true;
	}

	private bool _TryBuildMeshFilterInstances(MeshFilter[] meshFilterArray,out List<CombineInstance> instanceList,out List<Material> materialList,out long indexCount)
	{
		instanceList = new List<CombineInstance>();
		materialList = new List<Material>();
		indexCount = 0L;

		if(meshFilterArray == null || meshFilterArray.Length == 0)
		{
			return false;
		}

		for(var i=0;i<meshFilterArray.Length;i++)
		{
			if(!_TryGetValidMeshFilter(meshFilterArray[i],out var meshFilter))
			{
				continue;
			}

			var mesh = meshFilter.sharedMesh;

			instanceList.Add(_CreateCombineInstance(mesh,_GetCombineMatrix(meshFilter)));
			indexCount += CalculateMeshIndexCount(mesh);
			_AppendSourceMaterials(meshFilter,mesh,materialList);
		}

		return instanceList.Count > 0;
	}

	private bool _TryBuildMeshInstances(Mesh[] meshArray,out List<CombineInstance> instanceList,out long indexCount)
	{
		instanceList = new List<CombineInstance>();
		indexCount = 0L;

		if(meshArray == null || meshArray.Length == 0)
		{
			return false;
		}

		for(var i=0;i<meshArray.Length;i++)
		{
			var mesh = meshArray[i];

			if(mesh == null)
			{
				continue;
			}

			instanceList.Add(_CreateCombineInstance(mesh,Matrix4x4.identity));
			indexCount += CalculateMeshIndexCount(mesh);
		}

		return instanceList.Count > 0;
	}

	private void _SetCombinedMesh(List<CombineInstance> instanceList,List<Material> materialList,long indexCount)
	{
		var mergeSubMeshes = _ShouldMergeSubMeshes(materialList);
		var combined = _CreateRuntimeMesh();

		_ConfigureMeshIndexFormat(combined,indexCount);
		combined.CombineMeshes(instanceList.ToArray(),mergeSubMeshes);
		combined.RecalculateBounds();
		combined.RecalculateNormals();
		combined.RecalculateTangents();

		_ReleaseOwnedMesh();
		m_meshFilter.sharedMesh = combined;
		m_ownsRuntimeMesh = true;

		_ApplyMaterials(combined,materialList,mergeSubMeshes);
	}

	private void _ApplyMaterials(Mesh combined,List<Material> materialList,bool mergeSubMeshes)
	{
		if(materialList == null || materialList.Count == 0)
		{
			return;
		}

		if(mergeSubMeshes)
		{
			m_meshRenderer.sharedMaterials = new[] { materialList[0] };

			return;
		}

		var materials = materialList.ToArray();

		if(materials.Length != combined.subMeshCount)
		{
			LogChannel.None.W($"Material count ({materials.Length}) does not match combined sub-mesh count ({combined.subMeshCount}). Object: {_GetObjectName()}");
			materials = _ResizeMaterialArray(materials,combined.subMeshCount);
		}

		m_meshRenderer.sharedMaterials = materials;
	}

	private static Material[] _ResizeMaterialArray(Material[] materials,int subMeshCount)
	{
		var resizedMaterials = new Material[subMeshCount];
		var fallbackMaterial = materials.Length > 0 ? materials[^1] : null;

		for(var i=0;i<subMeshCount;i++)
		{
			resizedMaterials[i] = i < materials.Length ? materials[i] : fallbackMaterial;
		}

		return resizedMaterials;
	}

	private static void _AppendSourceMaterials(MeshFilter meshFilter,Mesh mesh,List<Material> materialList)
	{
		var meshRenderer = meshFilter.GetComponent<MeshRenderer>();

		if(!meshRenderer)
		{
			for(var subMeshIndex=0;subMeshIndex<mesh.subMeshCount;subMeshIndex++)
			{
				materialList.Add(null);
			}

			return;
		}

		var materials = meshRenderer.sharedMaterials;

		if(materials == null || materials.Length == 0)
		{
			materialList.Add(meshRenderer.sharedMaterial);

			return;
		}

		if(materials.Length == mesh.subMeshCount)
		{
			materialList.AddRange(materials);

			return;
		}

		if(materials.Length == 1)
		{
			for(var subMeshIndex=0;subMeshIndex<mesh.subMeshCount;subMeshIndex++)
			{
				materialList.Add(materials[0]);
			}

			return;
		}

		materialList.AddRange(materials);
	}

	private Matrix4x4 _GetCombineMatrix(MeshFilter meshFilter)
	{
		return transform.worldToLocalMatrix*meshFilter.transform.localToWorldMatrix;
	}

	private static CombineInstance _CreateCombineInstance(Mesh mesh,Matrix4x4 matrix)
	{
		return new CombineInstance { mesh = mesh, transform = matrix };
	}

	private static bool _TryGetValidMeshFilter(MeshFilter meshFilter,out MeshFilter validMeshFilter)
	{
		validMeshFilter = meshFilter;

		return meshFilter != null && meshFilter.sharedMesh != null;
	}

	private static bool _ShouldMergeSubMeshes(List<Material> materialList)
	{
		if(materialList == null || materialList.Count <= 1)
		{
			return true;
		}

		var firstMaterial = materialList[0];

		for(var i=1;i<materialList.Count;i++)
		{
			if(materialList[i] != firstMaterial)
			{
				return false;
			}
		}

		return true;
	}

	private static void _DisableMeshFilters(MeshFilter[] meshFilterArray)
	{
		for(var i=0;i<meshFilterArray.Length;i++)
		{
			var meshFilter = meshFilterArray[i];

			if(meshFilter == null)
			{
				continue;
			}

			meshFilter.gameObject.SetActive(false);
		}
	}

	private Mesh _CreateRuntimeMesh()
	{
		return new Mesh { name = m_meshFilter.gameObject.name };
	}

	private static Mesh _DuplicateMesh(Mesh source)
	{
		return Instantiate(source);
	}

	private void _ReleaseOwnedMesh()
	{
		if(!m_ownsRuntimeMesh || !m_meshFilter || !m_meshFilter.sharedMesh)
		{
			return;
		}

		Destroy(m_meshFilter.sharedMesh);
		m_meshFilter.sharedMesh = null;
		m_ownsRuntimeMesh = false;
	}
}
