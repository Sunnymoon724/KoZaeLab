using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using KZLib.Attributes;

[RequireComponent(typeof(Toggle))]
public abstract class BaseToggleUI : MonoBehaviour
{
	[VerticalGroup("0",Order = 0),SerializeField]
	protected Toggle m_toggle = null;

	[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("O","X")]
	public bool IsOn => m_toggle != null && m_toggle.isOn;

	private void OnEnable()
	{
		m_toggle.onValueChanged.AddAction(_OnClickedToggle);
	}

	private void OnDisable()
	{
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

	private void Reset()
	{
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