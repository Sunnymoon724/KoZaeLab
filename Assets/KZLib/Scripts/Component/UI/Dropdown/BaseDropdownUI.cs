using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class BaseDropdownUI : MonoBehaviour
{
	[SerializeField]
	protected TMP_Dropdown m_dropdown = null;

	private void OnEnable()
	{
		m_dropdown.onValueChanged.AddAction(_OnValueChanged);
	}

	private void OnDisable()
	{
		m_dropdown.onValueChanged.RemoveAction(_OnValueChanged);
	}

	private void Reset()
	{
		if(!m_dropdown)
		{
			m_dropdown = GetComponent<TMP_Dropdown>();
		}
	}

	protected abstract void _OnValueChanged(int _index);
}