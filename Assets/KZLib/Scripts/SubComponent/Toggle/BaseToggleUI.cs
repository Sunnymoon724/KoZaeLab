using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using KZLib.KZAttribute;

[RequireComponent(typeof(Toggle))]
public abstract class BaseToggleUI : BaseComponentUI
{
	[Serializable]
	protected abstract class ToggleChild
	{
		[SerializeField,HideInInspector]
		private bool m_isOn = false;
		[SerializeField,HideInInspector]
		private bool m_inverseSelf = false;

		[HorizontalGroup("3",Order = 3),ShowInInspector,ToggleLeft]
		public bool IsOn
		{
			get => m_isOn;
			set
			{
				m_isOn = value;

				Set();
			}
		}

		[HorizontalGroup("3",Order = 3),ShowInInspector,ToggleLeft]
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

	[VerticalGroup("0",Order = 0),SerializeField]
	protected Toggle m_toggle = null;

	[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("O","X")]
	public bool IsOn => m_toggle != null && m_toggle.isOn;

	protected abstract IEnumerable<ToggleChild> ToggleChildGroup { get; }

	protected override void Initialize()
	{
		base.Initialize();

		m_toggle.onValueChanged.AddAction(OnClickedToggle);

		OnClickedToggle(m_toggle.isOn);
	}

	private void OnClickedToggle(bool isToggle)
	{
		foreach(var child in ToggleChildGroup)
		{
			child.IsOn = isToggle;
		}
	}

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