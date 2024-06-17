using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GraphImageUI : BaseImageUI
{
	[SerializeField]
	private Shader m_Shader = null;

	protected override void Awake()
	{
		base.Awake();

		m_Image.material = new Material(m_Shader);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Shader)
		{
			m_Shader = Shader.Find("KZLib/GraphTexture");
		}
	}

	public void Initialize(int _resolution)
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