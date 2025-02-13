using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_rectTransform = null;

	public RectTransform UIRectTransform
	{
		get
		{
			InitializeRectTransform();

			return m_rectTransform;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		InitializeRectTransform();
	}

	private void InitializeRectTransform()
	{
		if(!m_rectTransform)
		{
			m_rectTransform = GetComponent<RectTransform>();
		}
	}
}