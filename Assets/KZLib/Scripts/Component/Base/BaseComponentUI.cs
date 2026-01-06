using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_currentRect = null;

	public RectTransform CurrentRect
	{
		get
		{
			_InitializeRectTransform();

			return m_currentRect;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		_InitializeRectTransform();
	}

	private void _InitializeRectTransform()
	{
		if(!m_currentRect)
		{
			m_currentRect = GetComponent<RectTransform>();
		}
	}
}