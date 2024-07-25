using UnityEngine;
using Sirenix.OdinInspector;
using System;

public abstract class ToggleUI : BaseComponentUI
{
	[SerializeField,HideInInspector]
	private bool m_IsOn = false;

	[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("토글 여부")]
	public bool IsOn => m_IsOn;

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("반전 설정")]
	private bool m_InverseSelf = false;

	protected bool IsOnNow => m_InverseSelf ? !m_IsOn : m_IsOn;

	private Action<bool> m_OnChanged = null;

	public event Action<bool> OnChanged
	{
		add { m_OnChanged -= value; m_OnChanged += value; }
		remove { m_OnChanged -= value; }
	}

	protected abstract void PlayToggle();

	public void SetToggle(bool _isOn,bool _force = false)
	{
		if(!_force && m_IsOn == _isOn)
		{
			return;
		}

		m_IsOn = _isOn;

		m_OnChanged?.Invoke(IsOnNow);

		PlayToggle();
	}
}