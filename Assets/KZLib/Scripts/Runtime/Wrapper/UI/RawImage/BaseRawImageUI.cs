using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wrapper base for Unity <see cref="RawImage"/>. <see cref="Reset"/> auto-fills <see cref="m_rawImage"/> and calls <see cref="_Reset"/>.
/// </summary>
[RequireComponent(typeof(RawImage))]
public abstract class BaseRawImageUI : BaseComponent
{
	[SerializeField]
	protected RawImage m_rawImage = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_rawImage)
		{
			m_rawImage = GetComponent<RawImage>();
		}

		_Reset();
	}

	/// <summary>Editor reset hook for raw-image defaults (anchors, raycast, etc.).</summary>
	protected virtual void _Reset() { }
}
