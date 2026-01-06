using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class AccordionToggleMount : BaseToggleMount
{
	[SerializeField]
	private RectTransform m_headerRect = null;

	protected override void Set()
	{
		if(!m_headerRect)
		{
			LogSvc.UI.W("header is null");

			return;
		}

		// Accordion.Transition transition = (this.m_Accordion != null) ? this.m_Accordion.transition : Accordion.Transition.Instant;

		// if(transition == Accordion.Transition.Instant && m_Accordion != null)
		// {
		// 	if(IsOnNow)
		// 	{
		// 		if (m_Accordion.ExpandVerticval)
		// 		{
		// 			this.m_LayoutElement.preferredHeight = -1f;
		// 		}
		// 		else
		// 		{
		// 			this.m_LayoutElement.preferredWidth = -1f;
		// 		}
		// 	}
		// 	else
		// 	{
		// 		if (m_Accordion.ExpandVerticval)
		// 		{
		// 			this.m_LayoutElement.preferredHeight = this.m_MinHeight;
		// 		}
		// 		else
		// 		{
		// 			this.m_LayoutElement.preferredWidth = this.m_MinWidth;
		// 		}
		// 	}
		// }
		// else if(transition == Accordion.Transition.Tween)
		// {
		// 	if(IsOnNow)
		// 	{
		// 		if (m_Accordion.ExpandVerticval)
		// 		{
		// 			this.StartTween(this.m_MinHeight, this.GetExpandedHeight());
		// 		}
		// 		else
		// 		{
		// 			this.StartTween(this.m_MinWidth, this.GetExpandedWidth());
		// 		}
		// 	}
		// 	else
		// 	{
		// 		if (m_Accordion.ExpandVerticval)
		// 		{
		// 			this.StartTween(this.m_RectTransform.rect.height, this.m_MinHeight);
		// 		}
		// 		else
		// 		{
		// 			this.StartTween(this.m_RectTransform.rect.width, this.m_MinWidth);
		// 		}
		// 	}
		// }
	}
}