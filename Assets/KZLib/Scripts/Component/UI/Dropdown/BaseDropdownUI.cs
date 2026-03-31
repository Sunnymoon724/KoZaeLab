using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class BaseDropdownUI : MonoBehaviour
{
	[SerializeField]
	protected TMP_Dropdown m_dropdown = null;

	protected virtual void OnEnable()
	{
		m_dropdown.onValueChanged.AddAction(_OnValueChanged);
	}

	protected virtual void OnDisable()
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