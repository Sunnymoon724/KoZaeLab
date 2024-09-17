using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TabButtonUI : BaseButtonUI
{
	[SerializeField,HideInInspector]
	private bool m_Select = false;

	[SerializeField,HideInInspector]
	private TabGroupUI m_TabGroup = null;

	[FoldoutGroup("기본 설정",Order = 0),ShowInInspector,LabelText("선택")]
	public bool Select { get => isActiveAndEnabled && m_Select; private set => SelectTap(value,true); }

	[FoldoutGroup("기본 설정",Order = 0),ShowInInspector,LabelText("탭 그룹")]
	private TabGroupUI TabGroup => m_TabGroup;

	private Action<bool> m_OnChanged = null;

	public event Action<bool> OnChanged
	{
		add { m_OnChanged -= value; m_OnChanged += value; }
		remove { m_OnChanged -= value; }
	}

	protected override void OnClickedButton()
	{
		Select = !Select;
	}

	public void SelectTapWithoutNotify(bool _value,bool _forceUpdate = false)
	{
		Set(_value,_forceUpdate,false);
	}

	public void SelectTap(bool _value,bool _forceUpdate = false)
	{
		Set(_value,_forceUpdate,true);
	}

	public void SetTabGroup(TabGroupUI _group)
	{
		m_TabGroup = _group;
	}

	private void Set(bool _value,bool _forceUpdate,bool _sendCallback)
	{
		if(!_forceUpdate && Select == _value)
		{
			return;
		}

		m_Select = _value;

		m_OnChanged?.Invoke(_value);

		if(_sendCallback)
		{
			if(TabGroup != null)
			{
				TabGroup.NotifyTab(this);
			}
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		SelectTap(Select);
	}
#endif
}