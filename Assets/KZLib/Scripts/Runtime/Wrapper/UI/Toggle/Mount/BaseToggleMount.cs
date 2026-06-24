using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Abstract toggle visual mount. <see cref="IsOn"/> drives <see cref="Set"/>; <see cref="InverseSelf"/> flips the effective state.
/// </summary>
public abstract class BaseToggleMount : BaseComponent
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

	/// <summary>Effective on-state after applying <see cref="InverseSelf"/>.</summary>
	protected bool IsOnNow => m_inverseSelf ? !IsOn : IsOn;

	/// <summary>Apply visual state for the current <see cref="IsOnNow"/>.</summary>
	protected abstract void Set();
}
