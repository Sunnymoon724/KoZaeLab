using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wrapper base for Unity <see cref="Button"/>. Wires <see cref="Button.onClick"/> in <see cref="OnEnable"/> when <see cref="UseButtonClickEvent"/> is true.
/// </summary>
[RequireComponent(typeof(Button))]
public abstract class BaseButtonUI : BaseComponent
{
	[SerializeField]
	protected Button m_button = null;

	/// <summary>When false, subclasses handle input themselves (e.g. long-press, pointer click).</summary>
	protected virtual bool UseButtonClickEvent => true;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(!UseButtonClickEvent || !m_button)
		{
			return;
		}

		m_button.onClick.AddAction(_OnClickedButton);
	}

	protected override void OnDisable()
	{
		if(UseButtonClickEvent && m_button)
		{
			m_button.onClick.RemoveAction(_OnClickedButton);
		}

		base.OnDisable();
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_button)
		{
			m_button = GetComponent<Button>();
		}
	}

	/// <summary>Invoked on left-click when <see cref="UseButtonClickEvent"/> is enabled.</summary>
	protected abstract void _OnClickedButton();
}
