using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class BaseLineRenderer : BaseComponent
{
	[SerializeField]
	protected LineRenderer m_LineRenderer = null;

	private MaterialPropertyBlock m_PropertyBlock = null;

	protected MaterialPropertyBlock PropertyBlock => m_PropertyBlock ??= new();

	protected override void Reset()
	{
		base.Reset();

		if(!m_LineRenderer)
		{
			m_LineRenderer = GetComponent<LineRenderer>();
		}
	}

	public void SetLineRenderer(Vector2? _width = null,Color? _color = null)
	{
		if(_width.HasValue)
		{
			m_LineRenderer.startWidth = _width.Value.x;
			m_LineRenderer.endWidth = _width.Value.y;
		}

		if(_color.HasValue)
		{
			PropertyBlock.SetColor("_Color",_color.Value);
			m_LineRenderer.SetPropertyBlock(PropertyBlock);
		}
	}

	public void SetPointArray(Vector3[] _pointArray)
	{
		m_LineRenderer.positionCount = _pointArray.Length;
		m_LineRenderer.SetPositions(_pointArray);
	}

	public Mesh BakeMesh()
	{
		var mesh = new Mesh();

		m_LineRenderer.BakeMesh(mesh,true);

		return mesh;
	}
}