// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class ToggleUISlot : SlotUI
// {
// 	protected const int TOGGLE_ORDER = 3;

// 	[FoldoutGroup("토글",Order = TOGGLE_ORDER),SerializeField]
// 	private UILayoutGroup m_LayoutGroup = null;

// 	[FoldoutGroup("토글",Order = TOGGLE_ORDER),SerializeField,LabelText("자식 리스트"),ListDrawerSettings(DraggableItems = false)]
// 	private List<ToggleUI> m_ChildList = new();

// 	public IReadOnlyCollection<ToggleUI> ChildCollection => m_ChildList.AsReadOnly();

// 	public virtual void SetToggle(bool _isOn)
// 	{
// 		foreach(var child in ChildCollection)
// 		{
// 			child.SetToggle(_isOn);
// 		}
// 	}

// 	protected override void SetOnClicked(Action<object> _onClicked)
// 	{
// 		base.SetOnClicked(_onClicked);

// 		m_OnClicked += (dummy) =>
// 		{
// 			m_LayoutGroup.NotifyAllToggle(this);
// 		};
// 	}
// }