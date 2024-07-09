using Sirenix.OdinInspector;
using UnityEngine;

public class ShaderImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	protected Shader m_Shader = null;

	[ShowInInspector,LabelText("쉐이더")]
	protected Shader Shader
	{
		get => m_Shader;
		set
		{
			if(m_Shader == value)
			{
				return;
			}

			m_Shader = value;

			if(m_Image)
			{
				m_Image.material = m_Shader != null ? new Material(m_Shader) : null;
			}
		}
	}

	protected override void Initialize()
	{
		base.Initialize();

		if(m_Image && !m_Image.material && Shader)
		{
			m_Image.material = new Material(Shader);
		}
	}

	protected override void Release()
	{
		base.Release();

		if(m_Image && m_Image.material)
		{
			CommonUtility.DestroyObject(m_Image.material);

			m_Image.material = null;
		}
	}
}