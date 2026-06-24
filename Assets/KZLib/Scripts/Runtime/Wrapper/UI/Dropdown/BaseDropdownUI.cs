using TMPro;
using UnityEngine;

/// <summary>
/// Wrapper base for TMP <see cref="TMP_Dropdown"/>. Subscribes to <see cref="TMP_Dropdown.onValueChanged"/> in enable/disable.
/// </summary>
[RequireComponent(typeof(TMP_Dropdown))]
public abstract class BaseDropdownUI : BaseComponent
{
	[SerializeField]
	protected TMP_Dropdown m_dropdown = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_dropdown)
		{
			m_dropdown.onValueChanged.AddAction(_OnValueChanged);
		}
	}

	protected override void OnDisable()
	{
		if(m_dropdown)
		{
			m_dropdown.onValueChanged.RemoveAction(_OnValueChanged);
		}

		base.OnDisable();
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_dropdown)
		{
			m_dropdown = GetComponent<TMP_Dropdown>();
		}
	}

	/// <summary>Called when the dropdown selection index changes.</summary>
	protected abstract void _OnValueChanged(int index);
}