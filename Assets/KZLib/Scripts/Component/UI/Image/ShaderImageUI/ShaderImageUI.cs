using Sirenix.OdinInspector;
using UnityEngine;

public class ShaderImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	protected Shader m_shader = null;

	[ShowInInspector]
	protected Shader Shader
	{
		get => m_shader;
		set
		{
			if(m_shader == value)
			{
				return;
			}

			m_shader = value;

			if(m_image)
			{
				m_image.material = m_shader != null ? new Material(m_shader) : null;
			}
		}
	}

	protected override void _Initialize()
	{
		base._Initialize();

		if(m_image && !m_image.material && Shader)
		{
			m_image.material = new Material(Shader);
		}
	}

	protected override void _Release()
	{
		base._Release();

		if(m_image && m_image.material)
		{
			m_image.material.DestroyObject();

			m_image.material = null;
		}
	}
}