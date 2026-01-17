using UnityEngine;
using UnityEngine.UI;

public class EnchantedContentSizeFitter : ContentSizeFitter
{
	private RectTransform m_currentRectTrans = null;
	private RectTransform m_parentRectTrans = null;

	protected override void Awake()
	{
		base.Awake();

		m_currentRectTrans = transform.GetComponent<RectTransform>();

		Transform parentTransform = transform.parent;

		if(parentTransform != null)
		{
			m_parentRectTrans = parentTransform.GetComponent<RectTransform>();
		}
	}

	public override void SetLayoutHorizontal()
	{
		base.SetLayoutHorizontal();

		if(m_HorizontalFit == FitMode.Unconstrained)
		{
			return;
		}

		if(!m_parentRectTrans)
		{
			return;
		}

		var parentWidth = m_parentRectTrans.rect.width; 
		var currentWidth = m_currentRectTrans.rect.width;

		if(currentWidth > parentWidth)
		{
			m_currentRectTrans.sizeDelta = new Vector2(0.0f,m_currentRectTrans.sizeDelta.y);
		}
	}

	public override void SetLayoutVertical()
	{
		base.SetLayoutVertical();

		if(m_VerticalFit == FitMode.Unconstrained)
		{
			return;
		}

		if(!m_parentRectTrans)
		{
			return;
		}

		var parentHeight = m_parentRectTrans.rect.height;
		var currentHeight = m_currentRectTrans.rect.height;

		if( currentHeight > parentHeight )
		{
			m_currentRectTrans.sizeDelta = new Vector2(m_currentRectTrans.sizeDelta.x,0.0f);
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();

		SetDirty();
	}
}