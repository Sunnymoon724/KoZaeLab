using System;
using UnityEngine.EventSystems;

/// <summary>
/// Button with separate callbacks for interactable vs non-interactable clicks. Uses <see cref="IPointerClickHandler"/> instead of <see cref="Button.onClick"/>.
/// </summary>
public class ConditionButtonUI : BaseButtonUI,IPointerClickHandler
{
	protected override bool UseButtonClickEvent => false;

	private Action m_onClickedEnable = null;
	private Action m_onClickedDisable = null;

	/// <summary>Registers callbacks for interactable (<paramref name="onClickedEnable"/>) vs disabled (<paramref name="onClickedDisable"/>) clicks.</summary>
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

		if(m_button == null)
		{
			return;
		}

		if(!m_button.interactable)
		{
			m_onClickedDisable?.Invoke();
		}
		else
		{
			m_onClickedEnable?.Invoke();
		}
	}
}