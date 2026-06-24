using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using KZLib.Attributes;

/// <summary>
/// Wrapper base for Unity <see cref="Toggle"/>. Wires <see cref="Toggle.onValueChanged"/> and exposes <see cref="Set"/> / <see cref="Toggle"/>.
/// </summary>
[RequireComponent(typeof(Toggle))]
public abstract class BaseToggleUI : BaseComponent
{
	[VerticalGroup("0",Order = 0),SerializeField]
	protected Toggle m_toggle = null;

	[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("O","X")]
	public bool IsOn => m_toggle != null && m_toggle.isOn;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(!m_toggle)
		{
			return;
		}

		m_toggle.onValueChanged.AddAction(_OnClickedToggle);
	}

	protected override void OnDisable()
	{
		if(m_toggle)
		{
			m_toggle.onValueChanged.RemoveAction(_OnClickedToggle);
		}

		base.OnDisable();
	}

	/// <summary>Called when the toggle value changes (user or programmatic).</summary>
	protected abstract void _OnClickedToggle(bool isToggle);

	public void Toggle()
	{
		if(!m_toggle)
		{
			return;
		}

		m_toggle.isOn = !m_toggle.isOn;
	}

	public void Set(bool isOn,bool isForce = false)
	{
		if(!m_toggle)
		{
			return;
		}

		if(!isForce && m_toggle.isOn == isOn)
		{
			return;
		}

		m_toggle.isOn = isOn;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_toggle)
		{
			m_toggle = GetComponent<Toggle>();
		}
	}

	public void SetToggleGroup(ToggleGroup toggleGroup)
	{
		if(!m_toggle)
		{
			return;
		}

		m_toggle.group = toggleGroup;
	}
}
