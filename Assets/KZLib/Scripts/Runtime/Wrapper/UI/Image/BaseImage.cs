using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wrapper base for Unity <see cref="Image"/>. <see cref="Reset"/> auto-fills <see cref="m_image"/> and calls <see cref="_Reset"/>.
/// </summary>
[RequireComponent(typeof(Image))]
public abstract class BaseImage : BaseComponent
{
	[SerializeField]
	protected Image m_image = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_image)
		{
			m_image = GetComponent<Image>();
		}

		_Reset();
	}

	/// <summary>Editor reset hook for image defaults (fill type, color, etc.).</summary>
	protected virtual void _Reset() { }
}
