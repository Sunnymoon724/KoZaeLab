using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public abstract class BaseMesh : BaseComponent
{
	protected const long c_maxIndexCount = 65535L;

	[SerializeField]
	protected MeshFilter m_meshFilter = null;
	[SerializeField]
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

	protected long CalculateMeshIndexCount(params MeshFilter[] meshFilterArray)
	{
		var count = 0L;

		for(var i=0;i<meshFilterArray.Length;i++)
		{
			var meshFilter = meshFilterArray[i];

			if(meshFilter == null || meshFilter.sharedMesh == null)
			{
				continue;
			}

			for(var j=0;j<meshFilter.sharedMesh.subMeshCount;j++)
			{
				count += meshFilter.sharedMesh.GetIndexCount(j);
			}
		}

		return count;
	}

	protected long CalculateMeshIndexCount(params Mesh[] meshArray)
	{
		var count = 0L;

		for(var i=0;i<meshArray.Length;i++)
		{
			var mesh = meshArray[i];

			if(mesh == null)
			{
				continue;
			}

			for(var j=0;j<mesh.subMeshCount;j++)
			{
				count += mesh.GetIndexCount(j);
			}
		}

		return count;
	}

	protected bool IsValidMeshIndexCount(long count)
	{
		return count < c_maxIndexCount;
	}
}