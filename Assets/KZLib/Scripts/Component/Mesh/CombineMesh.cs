using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : BaseMesh
{
	public void CombineMeshFilter(MeshFilter[] filterArray)
	{
		if(filterArray == null || filterArray.Length == 0)
		{
			return;
		}

		_InitializeMesh();

		var count = CalculateMeshIndexCount(filterArray);

		if(!IsValidMeshIndexCount(count))
		{
			KZLogType.System.W($"The number of indices in the batched mesh is high. Object: {m_meshFilter.gameObject.name} : Index Count: {count}");

			return;
		}

		var instanceList = new List<CombineInstance>();

		foreach(var filter in filterArray)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = filter.sharedMesh, transform = filter.transform.localToWorldMatrix, };

			filter.gameObject.SetActive(false);

			instanceList.Add(instance);
		}

		_SetMesh(instanceList.ToArray());
	}

	public void CombineMeshFilter(Mesh[] _meshArray)
	{
		if(_meshArray == null || _meshArray.Length == 0)
		{
			return;
		}

		_InitializeMesh();

		var count = CalculateMeshIndexCount(_meshArray);

		if(!IsValidMeshIndexCount(count))
		{
			KZLogType.System.W($"The number of indices in the batched mesh is high. Object: {m_meshFilter.gameObject.name} : Index Count: {count}");


			return;
		}

		var instanceList = new List<CombineInstance>();

		foreach(var mesh in _meshArray)
		{
			if(mesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = mesh, transform = Matrix4x4.identity, };

			instanceList.Add(instance);
		}

		_SetMesh(instanceList.ToArray());
	}

	public bool AppendMesh(params MeshFilter[] filterArray)
	{
		if(filterArray == null)
		{
			return true;
		}

		if(m_meshFilter.sharedMesh == null)
		{
			if(filterArray.Length == 1)
			{
				m_meshFilter.sharedMesh = filterArray[0].sharedMesh;
				m_meshFilter.sharedMesh.name = m_meshFilter.gameObject.name;

				filterArray[0].gameObject.SetActive(false);

				return true;
			}

			m_meshFilter.sharedMesh = new Mesh { name = m_meshFilter.gameObject.name };
		}

		var count = CalculateMeshIndexCount(m_meshFilter)+CalculateMeshIndexCount(filterArray);

		if(!IsValidMeshIndexCount(count))
		{
			return false;
		}

		var instanceList = new List<CombineInstance> { new() { mesh = m_meshFilter.sharedMesh, transform = Matrix4x4.identity, } };

		foreach(var filter in filterArray)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = filter.sharedMesh, transform = filter.transform.localToWorldMatrix, };

			filter.gameObject.SetActive(false);

			instanceList.Add(instance);
		}

		_SetMesh(instanceList.ToArray());

		return true;
	}

	public bool AppendMesh(params Mesh[] meshArray)
	{
		if(meshArray == null)
		{
			return true;
		}

		if(m_meshFilter.sharedMesh == null)
		{
			if(meshArray.Length == 1)
			{
				m_meshFilter.sharedMesh = meshArray[0];
				m_meshFilter.sharedMesh.name = m_meshFilter.gameObject.name;

				return true;
			}

			m_meshFilter.sharedMesh = new Mesh { name = m_meshFilter.gameObject.name };
		}

		var count = CalculateMeshIndexCount(m_meshFilter)+CalculateMeshIndexCount(meshArray);

		if(!IsValidMeshIndexCount(count))
		{
			return false;
		}

		var instanceList = new List<CombineInstance> { new() { mesh = m_meshFilter.sharedMesh, transform = Matrix4x4.identity, } };

		foreach(var mesh in meshArray)
		{
			if(mesh == null)
			{
				continue;
			}

			instanceList.Add(new CombineInstance { mesh = mesh, transform = Matrix4x4.identity, });
		}

		_SetMesh(instanceList.ToArray());

		return true;
	}

	private void _InitializeMesh()
	{
		if(m_meshFilter.sharedMesh == null)
		{
			m_meshFilter.sharedMesh = new Mesh { name = m_meshFilter.gameObject.name };
		}
		else
		{
			m_meshFilter.sharedMesh.Clear();
		}
	}

	private void _SetMesh(CombineInstance[] instanceArray)
	{
		var combined = new Mesh();

		combined.CombineMeshes(instanceArray,true);

		combined.RecalculateBounds();
		combined.RecalculateNormals();

		m_meshFilter.sharedMesh = combined;
	}
}