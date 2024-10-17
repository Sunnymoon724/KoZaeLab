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
		private bool m_IsOn = false;
		[SerializeField,HideInInspector]
		private bool m_InverseSelf = false;

		[BoxGroup("0",ShowLabel = false,Order = 0)]
		[HorizontalGroup("0/1",Order = 1),ShowInInspector,LabelText("Is On")]
		public bool IsOn
		{
			get => m_IsOn;
			set
			{
				m_IsOn = value;

				Set();
			}
		}

		[HorizontalGroup("0/1",Order = 1),ShowInInspector,LabelText("Inverse Self")]
		public bool InverseSelf
		{
			get => m_InverseSelf;
			set
			{
				m_InverseSelf = value;

				Set();
			}
		}

		protected bool IsOnNow => m_InverseSelf ? !IsOn : IsOn;

		protected abstract void Set();
	}

	[VerticalGroup("0",Order = 0),SerializeField,LabelText("Toggle")]
	protected Toggle m_Toggle = null;

	[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("Is On"),KZIsValid("O","X")]
	public bool IsOn => m_Toggle != null && m_Toggle.isOn;

	protected abstract IEnumerable<ToggleChild> ChildGroup { get; }

	protected override void Initialize()
	{
		base.Initialize();

		m_Toggle.AddListener(OnClickedToggle);

		OnClickedToggle(m_Toggle.isOn);
	}

	private void OnClickedToggle(bool _toggle)
	{
		foreach(var child in ChildGroup)
		{
			child.IsOn = _toggle;
		}
	}

	public void Toggle()
	{
		m_Toggle.isOn = !m_Toggle.isOn;
	}

	public void Set(bool _isOn,bool _force = false)
	{
		if(!_force && m_Toggle.isOn == _isOn)
		{
			return;
		}

		m_Toggle.isOn = _isOn;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Toggle)
		{
			m_Toggle = GetComponent<Toggle>();
		}
	}

	public void SetToggleGroup(ToggleGroup _group)
	{
		m_Toggle.group = _group;
	}
}