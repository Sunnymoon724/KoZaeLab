using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButtonUI : BaseComponentUI
{
	[SerializeField,LabelText("Button")]
	protected Button m_button = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_button.onClick.AddAction(OnClickedButton);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_button.onClick.RemoveAction(OnClickedButton);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_button)
		{
			m_button = GetComponent<Button>();
		}
	}

	protected abstract void OnClickedButton();
}