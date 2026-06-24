using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Common base for <see cref="MeshFilter"/> / <see cref="MeshRenderer"/> wrappers.
/// </summary>
/// <remarks>
/// Provides per-instance tinting via <see cref="MaterialPropertyBlock"/> and helpers for mesh-combine index budgeting.
/// </remarks>
[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public abstract class BaseMesh : BaseComponent
{
	/// <summary>Index count above this triggers <see cref="IndexFormat.UInt32"/> on combined meshes.</summary>
	protected const long c_uint16IndexLimit = 65535L;

	[SerializeField]
	protected MeshFilter m_meshFilter = null;
	[SerializeField]
	protected MeshRenderer m_meshRenderer = null;

	private MaterialPropertyBlock m_propertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new();

	protected override void _Initialize()
	{
		base._Initialize();

		if(!m_meshFilter)
		{
			m_meshFilter = GetComponent<MeshFilter>();
		}

		if(!m_meshRenderer)
		{
			m_meshRenderer = GetComponent<MeshRenderer>();
		}

		if(!m_meshRenderer || !m_meshFilter)
		{
			return;
		}
	}

	/// <summary>Auto-assigns <see cref="MeshFilter"/> and <see cref="MeshRenderer"/> in the editor.</summary>
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

	/// <summary>Sets tint on Built-in RP (<c>_Color</c>) and URP Lit (<c>_BaseColor</c>) via <see cref="MaterialPropertyBlock"/>.</summary>
	public void SetColor(Color color)
	{
		if(!m_meshRenderer)
		{
			return;
		}

		PropertyBlock.SetColor("_Color",color);
		PropertyBlock.SetColor("_BaseColor",color);
		m_meshRenderer.SetPropertyBlock(PropertyBlock);
	}

	/// <summary>Sums index counts from all sub-meshes on the given filters.</summary>
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

			count += CalculateMeshIndexCount(meshFilter.sharedMesh);
		}

		return count;
	}

	/// <summary>Sums index counts from all sub-meshes on <paramref name="mesh"/>.</summary>
	protected long CalculateMeshIndexCount(Mesh mesh)
	{
		if(mesh == null)
		{
			return 0L;
		}

		var count = 0L;

		for(var j=0;j<mesh.subMeshCount;j++)
		{
			count += mesh.GetIndexCount(j);
		}

		return count;
	}

	/// <summary>Sums index counts from all meshes in <paramref name="meshArray"/>.</summary>
	protected long CalculateMeshIndexCount(params Mesh[] meshArray)
	{
		var count = 0L;

		for(var i=0;i<meshArray.Length;i++)
		{
			count += CalculateMeshIndexCount(meshArray[i]);
		}

		return count;
	}

	/// <summary>Returns false when <paramref name="count"/> is zero. Logs when 32-bit indices are required.</summary>
	protected bool _IsValidMeshIndexCount(long count)
	{
		var objectName = _GetObjectName();

		if(count <= 0L)
		{
			LogChannel.None.W($"Mesh index count must be greater than zero. Object: {objectName}");

			return false;
		}

		if(count > c_uint16IndexLimit)
		{
			LogChannel.None.W($"Mesh index count exceeds 16-bit limit; 32-bit index format will be used. Object: {objectName} : Index Count: {count}");
		}

		return true;
	}

	protected string _GetObjectName()
	{
		return m_meshFilter ? m_meshFilter.gameObject.name : gameObject.name;
	}

	/// <summary>Picks <see cref="IndexFormat.UInt16"/> or <see cref="IndexFormat.UInt32"/> for a combined mesh.</summary>
	protected void _ConfigureMeshIndexFormat(Mesh mesh,long indexCount)
	{
		mesh.indexFormat = indexCount > c_uint16IndexLimit ? IndexFormat.UInt32 : IndexFormat.UInt16;
	}
}