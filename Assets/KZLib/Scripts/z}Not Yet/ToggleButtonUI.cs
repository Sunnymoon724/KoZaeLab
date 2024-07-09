using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

public class ToggleButtonUI : BaseButtonUI
{
	[SerializeField,HideInInspector]
	private bool m_IsOn = false;

	[FoldoutGroup("기본 설정",Order = 0)]
	[VerticalGroup("기본 설정/0",Order = 0),ShowInInspector,LabelText("토글 여부")]
	public bool IsOn { get => m_IsOn; set => Set(value,true); }

	[VerticalGroup("기본 설정/1",Order = 1),SerializeField,LabelText("자식 리스트"),ListDrawerSettings(DraggableItems = false,ShowFoldout = false)]
	private List<ToggleUI> m_ChildList = new();

	private Action<bool> m_OnChanged = null;

	public event Action<bool> OnChanged
	{
		add { m_OnChanged -= value; m_OnChanged += value; }
		remove { m_OnChanged -= value; }
	}

	public IEnumerable<ToggleUI> ChildGroup => m_ChildList;

	public void Toggle()
	{
		IsOn = !IsOn;
	}

	public void SetWithoutNotify(bool _value)
	{
		Set(_value,false);
	}

	private void Set(bool _value,bool _notify = false)
	{
		if(m_IsOn == _value)
		{
			return;
		}

		m_IsOn = _value;

		if(_notify)
		{
			Notify(false);
		}
	}

	private void Notify(bool _force)
	{
		m_OnChanged?.Invoke(m_IsOn);

		var iterator = ChildGroup.GetEnumerator();

		while(iterator.MoveNext())
		{
			var child = iterator.Current;

			child.SetToggle(m_IsOn,_force);
		}
	}

	protected override void OnClickButton()
	{
		Toggle();
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		Notify(true);
	}
#endif
}