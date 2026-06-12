using Sirenix.OdinInspector;
using UnityEngine;

public class ShaderImage : BaseImage
{
	[SerializeField,HideInInspector]
	protected Shader m_materialShader = null;

	[ShowInInspector]
	protected Shader MaterialShader
	{
		get => m_materialShader;
		set
		{
			if(m_materialShader == value)
			{
				return;
			}

			m_materialShader = value;

			if(m_image)
			{
				m_image.material = m_materialShader != null ? new Material(m_materialShader) : null;
			}
		}
	}

	protected override void _Initialize()
	{
		base._Initialize();

		if(m_image && !m_image.material && MaterialShader)
		{
			m_image.material = new Material(MaterialShader);
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