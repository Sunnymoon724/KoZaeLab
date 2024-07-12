using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : BaseMesh
{
	public void CombineMeshFilter(MeshFilter[] _filterArray)
	{
		if(_filterArray == null || _filterArray.Length == 0)
		{
			return;
		}

		InitializeMesh();

		var count = GetMeshIndexCount(_filterArray);

		if(!IsValidMeshIndexCount(count))
		{
			LogTag.System.W("배칭된 메쉬의 인덱스 갯수가 많습니다. 오브젝트 : {0} : 인덱스 갯수 : {1}",m_MeshFilter.gameObject.name,count);

			return;
		}

		var instanceList = new List<CombineInstance>();

		foreach(var filter in _filterArray)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = filter.sharedMesh, transform = filter.transform.localToWorldMatrix, };

			filter.gameObject.SetActive(false);

			instanceList.Add(instance);
		}

		SetMesh(instanceList.ToArray());
	}

	public void CombineMeshFilter(Mesh[] _meshArray)
	{
		if(_meshArray == null || _meshArray.Length == 0)
		{
			return;
		}

		InitializeMesh();

		var count = GetMeshIndexCount(_meshArray);

		if(!IsValidMeshIndexCount(count))
		{
			LogTag.System.W("배칭된 메쉬의 인덱스 갯수가 많습니다. 오브젝트 : {0} : 인덱스 갯수 : {1}",m_MeshFilter.gameObject.name,count);

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

		SetMesh(instanceList.ToArray());
	}

	public bool AppendMesh(params MeshFilter[] _filterArray)
	{
		if(_filterArray == null)
		{
			return true;
		}

		if(m_MeshFilter.sharedMesh == null)
		{
			if(_filterArray.Length == 1)
			{
				m_MeshFilter.sharedMesh = _filterArray[0].sharedMesh;
				m_MeshFilter.sharedMesh.name = m_MeshFilter.gameObject.name;

				_filterArray[0].gameObject.SetActive(false);

				return true;
			}

			m_MeshFilter.sharedMesh = new Mesh { name = m_MeshFilter.gameObject.name };
		}

		var count = GetMeshIndexCount(m_MeshFilter)+GetMeshIndexCount(_filterArray);

		if(!IsValidMeshIndexCount(count))
		{
			return false;
		}

		var instanceList = new List<CombineInstance> { new() { mesh = m_MeshFilter.sharedMesh, transform = Matrix4x4.identity, } };

		foreach(var filter in _filterArray)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = filter.sharedMesh, transform = filter.transform.localToWorldMatrix, };

			filter.gameObject.SetActive(false);

			instanceList.Add(instance);
		}

		SetMesh(instanceList.ToArray());

		return true;
	}

	public bool AppendMesh(params Mesh[] _meshArray)
	{
		if(_meshArray == null)
		{
			return true;
		}

		if(m_MeshFilter.sharedMesh == null)
		{
			if(_meshArray.Length == 1)
			{
				m_MeshFilter.sharedMesh = _meshArray[0];
				m_MeshFilter.sharedMesh.name = m_MeshFilter.gameObject.name;

				return true;
			}

			m_MeshFilter.sharedMesh = new Mesh { name = m_MeshFilter.gameObject.name };
		}

		var count = GetMeshIndexCount(m_MeshFilter)+GetMeshIndexCount(_meshArray);

		if(!IsValidMeshIndexCount(count))
		{
			return false;
		}

		var instanceList = new List<CombineInstance> { new() { mesh = m_MeshFilter.sharedMesh, transform = Matrix4x4.identity, } };

		foreach(var mesh in _meshArray)
		{
			if(mesh == null)
			{
				continue;
			}

			instanceList.Add(new CombineInstance { mesh = mesh, transform = Matrix4x4.identity, });
		}

		SetMesh(instanceList.ToArray());

		return true;
	}

	private void InitializeMesh()
	{
		if(m_MeshFilter.sharedMesh == null)
		{
			m_MeshFilter.sharedMesh = new Mesh { name = m_MeshFilter.gameObject.name };
		}
		else
		{
			m_MeshFilter.sharedMesh.Clear();
		}
	}

	private void SetMesh(CombineInstance[] _instanceArray)
	{
		var combined = new Mesh();

		combined.CombineMeshes(_instanceArray,true);

		combined.RecalculateBounds();
		combined.RecalculateNormals();

		m_MeshFilter.sharedMesh = combined;
	}
}