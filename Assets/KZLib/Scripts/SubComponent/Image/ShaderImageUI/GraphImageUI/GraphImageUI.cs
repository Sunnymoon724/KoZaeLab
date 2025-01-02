using System;
using UnityEngine;

public class GraphImageUI : ShaderImageUI
{
	[SerializeField]
	protected int m_graphLength = 0;

	[SerializeField]
	protected bool m_resetEnable = true;

	protected float[] m_graphArray = null;

	protected bool IsValidate => m_graphLength <= 0;

	protected override void Initialize()
	{
		base.Initialize();

		if(IsValidate)
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

		if(!m_shader)
		{
			m_shader = Shader.Find("KZLib/GraphTexture");
		}
	}

	public void ResetGraph()
	{
		if(IsValidate)
		{
			return;
		}

		Array.Clear(m_graphArray,0,m_graphArray.Length);

		SetGraphArray();
	}

	public virtual void UpdateGraph(float[] valueArray)
	{
		if(IsValidate)
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