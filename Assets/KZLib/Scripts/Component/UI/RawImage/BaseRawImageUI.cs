using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public abstract class BaseRawImageUI : BaseComponentUI
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
	}
}