using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButtonUI : BaseComponentUI
{
	[SerializeField,LabelText("Button")]
	protected Button m_Button = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(!m_Button)
		{
			LogTag.System.I("Button is null");

			return;
		}

		m_Button.onClick.AddAction(OnClickedButton);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if(!m_Button)
		{
			LogTag.System.I("Button is null");

			return;
		}

		m_Button.onClick.RemoveAction(OnClickedButton);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Button)
		{
			m_Button = GetComponent<Button>();
		}
	}

	protected abstract void OnClickedButton();
}