using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using KZLib.KZAttribute;

[RequireComponent(typeof(Toggle))]
public abstract class BaseToggleUI : BaseComponentUI
{
	[VerticalGroup("0",Order = 0),SerializeField]
	protected Toggle m_toggle = null;

	[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("O","X")]
	public bool IsOn => m_toggle != null && m_toggle.isOn;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_toggle.onValueChanged.AddAction(_OnClickedToggle);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_toggle.onValueChanged.RemoveAction(_OnClickedToggle);
	}

	protected abstract void _OnClickedToggle(bool isToggle);

	public void Toggle()
	{
		m_toggle.isOn = !m_toggle.isOn;
	}

	public void Set(bool isOn,bool isForce = false)
	{
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
		m_toggle.group = toggleGroup;
	}
}