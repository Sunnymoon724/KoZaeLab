using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BaseToggleMount : BaseComponentUI
{
	[SerializeField,HideInInspector]
	private bool m_isOn = false;
	[SerializeField,HideInInspector]
	private bool m_inverseSelf = false;

	[FoldoutGroup("Toggle State",Order = -10)]
	[HorizontalGroup("Toggle State/0"),ShowInInspector,ToggleLeft]
	public bool IsOn
	{
		get => m_isOn;
		set
		{
			m_isOn = value;

			Set();
		}
	}

	[HorizontalGroup("Toggle State/0"),ShowInInspector,ToggleLeft]
	public bool InverseSelf
	{
		get => m_inverseSelf;
		set
		{
			m_inverseSelf = value;

			Set();
		}
	}

	protected bool IsOnNow => m_inverseSelf ? !IsOn : IsOn;

	protected abstract void Set();
}