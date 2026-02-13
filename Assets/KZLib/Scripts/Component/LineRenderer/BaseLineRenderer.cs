using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class BaseLineRenderer : BaseComponent
{
	[SerializeField]
	protected LineRenderer m_lineRenderer = null;

	private MaterialPropertyBlock m_propertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_propertyBlock ??= new();

	public void SetLineRenderer(Vector2? width = null,Color? color = null)
	{
		if(width.HasValue)
		{
			m_lineRenderer.startWidth = width.Value.x;
			m_lineRenderer.endWidth = width.Value.y;
		}

		if(color.HasValue)
		{
			PropertyBlock.SetColor("_Color",color.Value);
			m_lineRenderer.SetPropertyBlock(PropertyBlock);
		}
	}

	public void SetPositionArray(Vector3[] positionArray)
	{
		m_lineRenderer.positionCount = positionArray.Length;
		m_lineRenderer.SetPositions(positionArray);
	}

	public Mesh BakeMesh()
	{
		var mesh = new Mesh();

		m_lineRenderer.BakeMesh(mesh,true);

		return mesh;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_lineRenderer)
		{
			m_lineRenderer = GetComponent<LineRenderer>();
		}
	}
}