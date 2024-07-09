using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public abstract class BaseMesh : BaseComponent
{
	protected const long MAX_INDEX_COUNT = 65535L;

	[SerializeField,LabelText("메쉬 필터")]
	protected MeshFilter m_MeshFilter = null;
	[SerializeField,LabelText("메쉬 렌더러")]
	protected MeshRenderer m_MeshRenderer = null;

	private MaterialPropertyBlock m_PropertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_PropertyBlock ??= new();

	protected override void Reset()
	{
		base.Reset();

		if(!m_MeshFilter)
		{
			m_MeshFilter = GetComponent<MeshFilter>();
		}

		if(!m_MeshRenderer)
		{
			m_MeshRenderer = GetComponent<MeshRenderer>();
		}
	}

	public void SetColor(Color _color)
	{
		PropertyBlock.SetColor("_Color",_color);
		m_MeshRenderer.SetPropertyBlock(PropertyBlock);
	}

	protected long GetMeshIndexCount(params MeshFilter[] _filterArray)
	{
		var count = 0L;

		foreach(var filter in _filterArray)
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

	protected long GetMeshIndexCount(params Mesh[] _meshArray)
	{
		var count = 0L;

		foreach(var mesh in _meshArray)
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

	protected bool IsValidMeshIndexCount(long _count)
	{
		return _count < MAX_INDEX_COUNT;
	}
}