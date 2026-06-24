using UnityEngine;

/// <summary>
/// Common base for Unity <see cref="LineRenderer"/> wrappers.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public abstract class BaseLineRenderer : BaseComponent
{
	[SerializeField]
	protected LineRenderer m_lineRenderer = null;

	private MaterialPropertyBlock m_propertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new();

	protected override void _Initialize()
	{
		base._Initialize();

		_EnsureLineRenderer();
	}

	/// <summary>Sets optional start/end width and/or tint via <see cref="MaterialPropertyBlock"/>.</summary>
	public void SetLineRenderer(Vector2? width = null,Color? color = null)
	{
		if(!_EnsureLineRenderer())
		{
			return;
		}

		if(width.HasValue)
		{
			m_lineRenderer.startWidth = width.Value.x;
			m_lineRenderer.endWidth = width.Value.y;
		}

		if(color.HasValue)
		{
			PropertyBlock.SetColor("_Color",color.Value);
			PropertyBlock.SetColor("_BaseColor",color.Value);
			m_lineRenderer.SetPropertyBlock(PropertyBlock);
		}
	}

	/// <summary>Replaces all line positions at once. A single point draws a one-vertex line.</summary>
	public void SetPositionArray(Vector3[] positionArray)
	{
		if(!_EnsureLineRenderer())
		{
			LogChannel.None.W($"LineRenderer is missing on {gameObject.name}.");

			return;
		}

		if(positionArray == null || positionArray.Length == 0)
		{
			LogChannel.None.W($"Position array is null or empty. Object: {gameObject.name}");

			return;
		}

		m_lineRenderer.positionCount = positionArray.Length;
		m_lineRenderer.SetPositions(positionArray);
	}

	/// <summary>Bakes the current line into a new <see cref="Mesh"/>. The caller must destroy the mesh when done.</summary>
	public Mesh BakeMesh()
	{
		if(!_EnsureLineRenderer())
		{
			LogChannel.None.W($"LineRenderer is missing on {gameObject.name}.");

			return null;
		}

		var mesh = new Mesh();

		m_lineRenderer.BakeMesh(mesh,true);

		return mesh;
	}

	protected override void Reset()
	{
		base.Reset();

		_EnsureLineRenderer();
	}

	protected bool _EnsureLineRenderer()
	{
		if(!m_lineRenderer)
		{
			m_lineRenderer = GetComponent<LineRenderer>();
		}

		return m_lineRenderer;
	}
}
