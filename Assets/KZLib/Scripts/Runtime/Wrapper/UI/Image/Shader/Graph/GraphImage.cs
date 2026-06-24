using System;
using UnityEngine;

/// <summary>
/// Shader-driven graph texture. Pushes float samples to <c>_GraphArray</c> on the image material.
/// </summary>
public class GraphImage : ShaderImage
{
	[SerializeField]
	protected int m_graphLength = 0;

	[SerializeField]
	private bool m_resetEnable = true;

	protected float[] m_graphArray = null;

	protected bool HasGraphData => m_graphLength > 0;

	protected override void _Initialize()
	{
		base._Initialize();

		_EnsureGraphArray();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_resetEnable)
		{
			ResetGraph();
		}
	}

	protected override void _Reset()
	{
		base._Reset();

		if(!m_materialShader)
		{
			m_materialShader = Shader.Find("KZLib/GraphTexture");
		}
	}

	/// <summary>Zero-fills the graph buffer and uploads to the shader.</summary>
	public void ResetGraph()
	{
		if(!_EnsureGraphArray())
		{
			return;
		}

		Array.Clear(m_graphArray,0,m_graphArray.Length);

		SetGraphArray();
	}

	/// <summary>Copies <paramref name="valueArray"/> into the internal buffer and uploads to the shader.</summary>
	public virtual void RefreshGraph(float[] valueArray)
	{
		if(!_EnsureGraphArray() || valueArray == null)
		{
			return;
		}

		var copyLength = Mathf.Min(valueArray.Length,m_graphArray.Length);

		Array.Copy(valueArray,0,m_graphArray,0,copyLength);

		if(copyLength < m_graphArray.Length)
		{
			Array.Clear(m_graphArray,copyLength,m_graphArray.Length-copyLength);
		}

		SetGraphArray();
	}

	protected bool _EnsureGraphArray()
	{
		if(!HasGraphData)
		{
			return false;
		}

		if(m_graphArray != null && m_graphArray.Length == m_graphLength && m_image?.material)
		{
			return true;
		}

		if(!m_image?.material)
		{
			return false;
		}

		m_graphArray = new float[m_graphLength];

		m_image.material.SetFloat("_GraphLength",m_graphLength);

		return true;
	}

	protected void SetGraphArray()
	{
		if(!m_image?.material || m_graphArray == null)
		{
			return;
		}

		m_image.material.SetFloatArray("_GraphArray",m_graphArray);
	}
}
