using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_rectTransform = null;

	public RectTransform UIRectTransform
	{
		get
		{
			_InitializeRectTransform();

			return m_rectTransform;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		_InitializeRectTransform();
	}

	private void _InitializeRectTransform()
	{
		if(!m_rectTransform)
		{
			m_rectTransform = GetComponent<RectTransform>();
		}
	}
}