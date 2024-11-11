using System;
using UnityEngine;

public class GraphImageUI : ShaderImageUI
{
	[SerializeField]
	protected int m_GraphLength = 0;

	[SerializeField]
	protected bool m_ResetEnable = true;

	protected float[] m_GraphArray = null;

	protected bool IsValidate => m_GraphLength <= 0;

	protected override void Initialize()
	{
		base.Initialize();

		if(IsValidate)
		{
			return;
		}

		m_GraphArray = new float[m_GraphLength];

		m_Image.material.SetFloat("_GraphLength",m_GraphLength);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_ResetEnable)
		{
			ResetGraph();
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Shader)
		{
			m_Shader = Shader.Find("KZLib/GraphTexture");
		}
	}

	public void ResetGraph()
	{
		if(IsValidate)
		{
			return;
		}

		Array.Clear(m_GraphArray,0,m_GraphArray.Length);

		SetGraphArray();
	}

	public virtual void UpdateGraph(float[] _dataArray)
	{
		if(IsValidate)
		{
			return;
		}

		Array.Copy(_dataArray,0,m_GraphArray,0,m_GraphArray.Length);

		SetGraphArray();
	}

	protected void SetGraphArray()
	{
		m_Image.material.SetFloatArray("_GraphArray",m_GraphArray);
	}
}