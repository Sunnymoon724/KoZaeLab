using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButtonUI : BaseComponentUI
{
	[SerializeField,LabelText("버튼")]
	protected Button m_Button = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Button.AddListener(OnClickedButton);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_Button.RemoveListener(OnClickedButton);
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