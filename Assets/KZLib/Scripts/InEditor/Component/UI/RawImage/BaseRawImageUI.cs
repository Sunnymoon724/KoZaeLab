using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public abstract class BaseRawImageUI : MonoBehaviour
{
	[SerializeField]
	protected RawImage m_rawImage = null;

	private void Reset()
	{
		if(!m_rawImage)
		{
			m_rawImage = GetComponent<RawImage>();
		}

		_Reset();
	}

	protected virtual void _Reset() { }
}