using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupUI : BaseComponentUI
{
	[SerializeField,LabelText("토글")]
	protected ToggleGroup m_ToggleGroup = null;

	[SerializeField,LabelText("시작 인덱스")]
	private int m_StartIndex = -1;

	[SerializeField,LabelText("토글 리스트"),ListDrawerSettings(DraggableItems = false,ShowFoldout = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(OnRemoveToggleByIndex)),OnValueChanged(nameof(OnChangeList))]
	private List<BaseToggleUI> m_ToggleUIList = new();

	[PropertySpace(5)]

	[ShowInInspector,LabelText("선택된 토글"),ReadOnly]
	private Toggle SelectedToggle => m_ToggleGroup == null ? null : m_ToggleGroup.GetFirstActiveToggle();

	protected override void Initialize()
	{
		base.Initialize();

		m_ToggleGroup.allowSwitchOff = m_StartIndex < 0;

		m_ToggleGroup.SetAllTogglesOff();

		Select(m_StartIndex);
	}

	private void OnRemoveToggleByIndex(int _index)
	{
		if(!m_ToggleUIList.ContainsIndex(_index))
		{
			return;
		}

		var toggle = m_ToggleUIList[_index].GetComponent<Toggle>();

		toggle.group = null;

		m_ToggleUIList.Remove(m_ToggleUIList[_index]);
	}

	private void OnChangeList()
	{
		var toggleList = new List<BaseToggleUI>(m_ToggleUIList);
		m_ToggleUIList.Clear();

		foreach(var toggleUI in toggleList)
		{
			m_ToggleUIList.AddNotOverlap(toggleUI);

			var toggle = toggleUI.GetComponent<Toggle>();

			toggle.group = m_ToggleGroup;
		}
	}

	public void Select(BaseToggleUI _toggle)
	{
		Select(m_ToggleUIList.IndexOf(_toggle));
	}

	public void Select(int _index,bool _notification = true)
	{
		if(!m_ToggleUIList.ContainsIndex(_index))
		{
			return;
		}

		var toggle = m_ToggleUIList[_index].GetComponent<Toggle>();

		m_ToggleGroup.NotifyToggleOn(toggle,_notification);
	}

	public T GetToggleUI<T>(int _index) where T : BaseToggleUI
	{
		return m_ToggleUIList.TryGetValueByIndex(_index,out var toggle) ? (T) toggle : null;
	}

	public int GetToggleIndex(BaseToggleUI _toggle)
	{
		return m_ToggleUIList.IndexOf(_toggle);
	}

	public bool ContainToggleUI(BaseToggleUI _toggle)
	{
		return m_ToggleUIList.Contains(_toggle);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_ToggleGroup)
		{
			m_ToggleGroup = GetComponent<ToggleGroup>();
		}
	}
}