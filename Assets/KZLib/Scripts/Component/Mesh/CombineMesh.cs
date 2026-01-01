using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : BaseMesh
{
	public void CombineMeshFilter(MeshFilter[] meshFilterArray)
	{
		if(meshFilterArray == null || meshFilterArray.Length == 0)
		{
			return;
		}

		_InitializeMesh();

		var count = CalculateMeshIndexCount(meshFilterArray);

		if(!IsValidMeshIndexCount(count))
		{
			LogSvc.System.W($"The number of indices in the batched mesh is high. Object: {m_meshFilter.gameObject.name} : Index Count: {count}");

			return;
		}

		var instanceList = new List<CombineInstance>();

		for(var i=0;i<meshFilterArray.Length;i++)
		{
			var meshFilter = meshFilterArray[i];

			if(meshFilter == null || meshFilter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = meshFilter.sharedMesh, transform = meshFilter.transform.localToWorldMatrix, };

			meshFilter.gameObject.SetActive(false);

			instanceList.Add(instance);
		}

		_SetMesh(instanceList.ToArray());
	}

	public void CombineMeshFilter(Mesh[] meshArray)
	{
		if(meshArray == null || meshArray.Length == 0)
		{
			return;
		}

		_InitializeMesh();

		var count = CalculateMeshIndexCount(meshArray);

		if(!IsValidMeshIndexCount(count))
		{
			LogSvc.System.W($"The number of indices in the batched mesh is high. Object: {m_meshFilter.gameObject.name} : Index Count: {count}");


			return;
		}

		var instanceList = new List<CombineInstance>();

		for(var i=0;i<meshArray.Length;i++)
		{
			var mesh = meshArray[i];

			if(mesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = mesh, transform = Matrix4x4.identity, };

			instanceList.Add(instance);
		}

		_SetMesh(instanceList.ToArray());
	}

	public bool AppendMesh(params MeshFilter[] meshFilterArray)
	{
		if(meshFilterArray == null)
		{
			return true;
		}

		if(m_meshFilter.sharedMesh == null)
		{
			if(meshFilterArray.Length == 1)
			{
				m_meshFilter.sharedMesh = meshFilterArray[0].sharedMesh;
				m_meshFilter.sharedMesh.name = m_meshFilter.gameObject.name;

				meshFilterArray[0].gameObject.SetActive(false);

				return true;
			}

			m_meshFilter.sharedMesh = new Mesh { name = m_meshFilter.gameObject.name };
		}

		var count = CalculateMeshIndexCount(m_meshFilter)+CalculateMeshIndexCount(meshFilterArray);

		if(!IsValidMeshIndexCount(count))
		{
			return false;
		}

		var instanceList = new List<CombineInstance> { new() { mesh = m_meshFilter.sharedMesh, transform = Matrix4x4.identity, } };

		for(var i=0;i<meshFilterArray.Length;i++)
		{
			var meshFilter = meshFilterArray[i];

			if(meshFilter == null || meshFilter.sharedMesh == null)
			{
				continue;
			}

			var instance = new CombineInstance { mesh = meshFilter.sharedMesh, transform = meshFilter.transform.localToWorldMatrix, };

			meshFilter.gameObject.SetActive(false);

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

		for(var i=0;i<meshArray.Length;i++)
		{
			var mesh = meshArray[i];

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