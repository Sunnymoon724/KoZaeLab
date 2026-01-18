using System;
using UnityEngine;

public class GraphImage : ShaderImage
{
	[SerializeField]
	protected int m_graphLength = 0;

	[SerializeField]
	private bool m_resetEnable = true;

	protected float[] m_graphArray = null;

	protected bool IsValidGraph => m_graphLength <= 0;

	protected override void _Initialize()
	{
		base._Initialize();

		if(IsValidGraph)
		{
			return;
		}

		m_graphArray = new float[m_graphLength];

		m_image.material.SetFloat("_GraphLength",m_graphLength);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_resetEnable)
		{
			ResetGraph();
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_materialShader)
		{
			m_materialShader = Shader.Find("KZLib/GraphTexture");
		}
	}

	public void ResetGraph()
	{
		if(IsValidGraph)
		{
			return;
		}

		Array.Clear(m_graphArray,0,m_graphArray.Length);

		SetGraphArray();
	}

	public virtual void RefreshGraph(float[] valueArray)
	{
		if(IsValidGraph)
		{
			return;
		}

		Array.Copy(valueArray,0,m_graphArray,0,m_graphArray.Length);

		SetGraphArray();
	}

	protected void SetGraphArray()
	{
		m_image.material.SetFloatArray("_GraphArray",m_graphArray);
	}
}