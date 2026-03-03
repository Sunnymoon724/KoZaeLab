using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class BaseImage : MonoBehaviour
{
	[SerializeField]
	protected Image m_image = null;

	private void Awake()
	{
		_Initialize();
	}

	protected virtual void _Initialize() { }

	private void OnDestroy()
	{
		_Release();
	}

	protected virtual void _Release() { }

	private void Reset()
	{
		if(!m_image)
		{
			m_image = GetComponent<Image>();
		}

		_Reset();
	}

	protected virtual void _Reset() { }
}