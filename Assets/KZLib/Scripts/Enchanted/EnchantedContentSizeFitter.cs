using UnityEngine;
using UnityEngine.UI;

public class EnchantedContentSizeFitter : ContentSizeFitter
{
	private RectTransform m_rectTransformCurrent;
	private RectTransform m_rectTransformParent;

	protected override void Awake()
	{
		base.Awake();

		m_rectTransformCurrent = transform.GetComponent<RectTransform>();

		Transform parentTransform = transform.parent;

		if(parentTransform != null)
		{
			m_rectTransformParent = parentTransform.GetComponent<RectTransform>();
		}
	}

	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();

		if(m_HorizontalFit == FitMode.Unconstrained)
		{
			return;
		}

		if(!m_rectTransformParent)
		{
			return;
		}

		var parentWidth = m_rectTransformParent.rect.width; 
		var currentWidth = m_rectTransformCurrent.rect.width;

		if(currentWidth > parentWidth)
		{
			m_rectTransformCurrent.sizeDelta = new Vector2(0.0f,m_rectTransformCurrent.sizeDelta.y);
		}
	}

	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();

		if(m_VerticalFit == FitMode.Unconstrained)
		{
			return;
		}

		if(!m_rectTransformParent)
		{
			return;
		}

		var parentHeight = m_rectTransformParent.rect.height;
		var currentHeight = m_rectTransformCurrent.rect.height;

		if( currentHeight > parentHeight )
		{
			m_rectTransformCurrent.sizeDelta = new Vector2(m_rectTransformCurrent.sizeDelta.x,0.0f);
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();

		SetDirty();
	}
}