using System;
using UnityEngine;

public class GraphImageUI : ShaderImageUI
{
	protected override void Reset()
	{
		base.Reset();

		if(!m_Shader)
		{
			m_Shader = Shader.Find("KZLib/GraphTexture");
		}
	}

	public void SetResolution(int _resolution)
	{
		var graphArray = new float[_resolution];

		Array.Clear(graphArray,0,graphArray.Length);

		m_Image.material.SetFloatArray("_GraphArray",graphArray);
		m_Image.material.SetFloat("_GraphLength",_resolution);
	}

	public void UpdateGraph(float[] _graphArray)
	{
		m_Image.material.SetFloatArray("_GraphArray",_graphArray);
	}
}