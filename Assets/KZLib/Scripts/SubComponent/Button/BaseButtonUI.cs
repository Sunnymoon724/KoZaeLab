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

		m_Button.onClick.AddAction(OnClickedButton);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

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