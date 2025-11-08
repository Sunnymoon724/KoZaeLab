using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class BaseDropdownUI : BaseComponentUI
{
	[SerializeField]
	protected TMP_Dropdown m_dropdown = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_dropdown.onValueChanged.AddAction(_OnValueChanged);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_dropdown.onValueChanged.RemoveAction(_OnValueChanged);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_dropdown)
		{
			m_dropdown = GetComponent<TMP_Dropdown>();
		}
	}

	protected abstract void _OnValueChanged(int _index);
}