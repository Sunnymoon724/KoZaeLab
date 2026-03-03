using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButtonUI : MonoBehaviour
{
	[SerializeField]
	protected Button m_button = null;

	private void OnEnable()
	{
		m_button.onClick.AddAction(_OnClickedButton);
	}

	private void OnDisable()
	{
		m_button.onClick.RemoveAction(_OnClickedButton);
	}

	private void Reset()
	{
		if(!m_button)
		{
			m_button = GetComponent<Button>();
		}
	}

	protected abstract void _OnClickedButton();
}