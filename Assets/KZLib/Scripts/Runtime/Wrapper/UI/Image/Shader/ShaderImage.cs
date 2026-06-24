using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// <see cref="Image"/> with an instanced shader material. Creates the material on init and destroys it on release.
/// </summary>
public class ShaderImage : BaseImage
{
	[SerializeField,HideInInspector]
	protected Shader m_materialShader = null;

	private bool m_ownsMaterial = false;

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

			_ApplyMaterialShader();
		}
	}

	protected override void _Initialize()
	{
		base._Initialize();

		if(!m_image || !MaterialShader || m_image.material)
		{
			return;
		}

		m_image.material = new Material(MaterialShader);
		m_ownsMaterial = true;
	}

	protected override void _Release()
	{
		_DestroyOwnedMaterial();

		base._Release();
	}

	private void _ApplyMaterialShader()
	{
		if(!m_image)
		{
			return;
		}

		_DestroyOwnedMaterial();

		if(m_materialShader)
		{
			m_image.material = new Material(m_materialShader);
			m_ownsMaterial = true;
		}
	}

	private void _DestroyOwnedMaterial()
	{
		if(!m_ownsMaterial || !m_image?.material)
		{
			return;
		}

		m_image.material.DestroyObject();

		m_image.material = null;
		m_ownsMaterial = false;
	}
}
