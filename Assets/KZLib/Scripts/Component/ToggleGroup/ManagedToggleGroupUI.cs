using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ManagedToggleGroupUI : BaseComponentUI
{
	[SerializeField]
	protected ToggleGroup m_toggleGroup = null;

	[SerializeField]
	private int m_startIndex = Global.INVALID_INDEX;

	[SerializeField,ListDrawerSettings(DraggableItems = false,ShowFoldout = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(_OnRemoveToggleByIndex)),OnValueChanged(nameof(_OnChangeList))]
	private List<BaseToggleUI> m_toggleUIList = new();

	[PropertySpace(5)]

	[ShowInInspector,ReadOnly]
	private Toggle SelectedToggle => m_toggleGroup == null ? null : m_toggleGroup.GetFirstActiveToggle();

	protected override void Initialize()
	{
		base.Initialize();

		m_toggleGroup.allowSwitchOff = m_startIndex < 0;

		m_toggleGroup.SetAllTogglesOff();

		Select(m_startIndex);
	}

	private void _OnRemoveToggleByIndex(int index)
	{
		if(!m_toggleUIList.ContainsIndex(index))
		{
			return;
		}

		var toggle = m_toggleUIList[index].GetComponent<Toggle>();

		toggle.group = null;

		m_toggleUIList.Remove(m_toggleUIList[index]);
	}

	private void _OnChangeList()
	{
		var toggleList = new List<BaseToggleUI>(m_toggleUIList);
		m_toggleUIList.Clear();

		foreach(var toggleUI in toggleList)
		{
			m_toggleUIList.AddNotOverlap(toggleUI);

			var toggle = toggleUI.GetComponent<Toggle>();

			toggle.group = m_toggleGroup;
		}
	}

	public void Select(BaseToggleUI toggleUI)
	{
		Select(m_toggleUIList.IndexOf(toggleUI));
	}

	public void Select(int index,bool isNotification = true)
	{
		if(!m_toggleUIList.ContainsIndex(index))
		{
			return;
		}

		var toggle = m_toggleUIList[index].GetComponent<Toggle>();

		m_toggleGroup.NotifyToggleOn(toggle,isNotification);
	}

	public T GetToggleUI<T>(int index) where T : BaseToggleUI
	{
		return m_toggleUIList.TryGetValueByIndex(index,out var toggle) ? (T) toggle : null;
	}

	public int GetToggleIndex(BaseToggleUI toggleUI)
	{
		return m_toggleUIList.IndexOf(toggleUI);
	}

	public bool ContainToggleUI(BaseToggleUI toggleUI)
	{
		return m_toggleUIList.Contains(toggleUI);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_toggleGroup)
		{
			m_toggleGroup = GetComponent<ToggleGroup>();
		}
	}
}