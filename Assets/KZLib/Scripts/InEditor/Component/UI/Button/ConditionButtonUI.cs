using System;
using UnityEngine.EventSystems;

public class ConditionButtonUI : BaseButtonUI,IPointerClickHandler
{
	private Action m_onClickedEnable = null;
	private Action m_onClickedDisable = null;

	public void SetClicked(Action onClickedEnable,Action onClickedDisable)
	{
		m_onClickedEnable = onClickedEnable;
		m_onClickedDisable = onClickedDisable;
	}

	protected override void _OnClickedButton() { }

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if(eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}

		if(m_button != null && !m_button.interactable)
		{
			m_onClickedDisable?.Invoke();
		}
		else
		{
			m_onClickedEnable?.Invoke();
		}
	}
}