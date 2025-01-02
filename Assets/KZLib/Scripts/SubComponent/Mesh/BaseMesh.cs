using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public abstract class BaseMesh : BaseComponent
{
	protected const long c_max_index_count = 65535L;

	[SerializeField,LabelText("Mesh Filter")]
	protected MeshFilter m_meshFilter = null;
	[SerializeField,LabelText("Mesh Renderer")]
	protected MeshRenderer m_meshRenderer = null;

	private MaterialPropertyBlock m_propertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new();

	protected override void Reset()
	{
		base.Reset();

		if(!m_meshFilter)
		{
			m_meshFilter = GetComponent<MeshFilter>();
		}

		if(!m_meshRenderer)
		{
			m_meshRenderer = GetComponent<MeshRenderer>();
		}
	}

	public void SetColor(Color _color)
	{
		PropertyBlock.SetColor("_Color",_color);
		m_meshRenderer.SetPropertyBlock(PropertyBlock);
	}

	protected long CalculateMeshIndexCount(params MeshFilter[] filterArray)
	{
		var count = 0L;

		foreach(var filter in filterArray)
		{
			if(filter == null || filter.sharedMesh == null)
			{
				continue;
			}

			for(var i=0;i<filter.sharedMesh.subMeshCount;i++)
			{
				count += filter.sharedMesh.GetIndexCount(i);
			}
		}

		return count;
	}

	protected long CalculateMeshIndexCount(params Mesh[] meshArray)
	{
		var count = 0L;

		foreach(var mesh in meshArray)
		{
			if(mesh == null)
			{
				continue;
			}

			for(var i=0;i<mesh.subMeshCount;i++)
			{
				count += mesh.GetIndexCount(i);
			}
		}

		return count;
	}

	protected bool IsValidMeshIndexCount(long count)
	{
		return count < c_max_index_count;
	}
}