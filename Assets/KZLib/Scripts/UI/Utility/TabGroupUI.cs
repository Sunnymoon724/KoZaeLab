using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ITabView
{
	void Notify(TabButtonUI _button);
}

public class TabGroupUI : BaseComponentUI
{
	[FoldoutGroup("기본 설정",Order = 0),SerializeField,LabelText("시작 인덱스")]
	private int m_StartIndex = -1;

	[FoldoutGroup("기본 설정",Order = 0),SerializeField,LabelText("탭 버튼 리스트")]
	private List<TabButtonUI> m_TabButtonList = new();

	[FoldoutGroup("기본 설정",Order = 0),SerializeField,LabelText("탭 뷰 리스트")]
	private List<ITabView> m_TabViewList = new();

	[FoldoutGroup("기본 설정",Order = 0),SerializeField,LabelText("선택된 탭 버튼"),ReadOnly]
	private TabButtonUI m_SelectedTab = null;

	[HideInInspector]
	public TabButtonUI SelectedTab => m_SelectedTab;

	private bool m_Initialized = false;

	protected override void Initialize()
	{
		if(m_Initialized)
		{
			return;
		}

		base.Initialize();

		for(var i=m_TabButtonList.Count-1;i>=0;i--)
		{
			var button = m_TabButtonList[i];

			if(!button)
			{
				m_TabButtonList.RemoveAt(i);

				continue;
			}

			button.SetTabGroup(this);
		}

		ForceSelect(m_StartIndex);

		m_Initialized = true;
	}

	public int GetSelectedIndex()
	{
		return GetTabButtonIndex(m_SelectedTab);
	}

	public TabButtonUI GetTabButton(int _index)
	{
		return m_TabButtonList.TryGetValueByIndex(_index,out var tabButton) ? tabButton : null;
	}

	public ITabView GetTabView(int _index)
	{
		return m_TabViewList.TryGetValueByIndex(_index,out var tabView) ? tabView : null;
	}

	public int GetTabButtonIndex(TabButtonUI _button)
	{
		return m_TabButtonList.IndexOf(_button);
	}

	public int GetTabViewIndex(ITabView _view)
	{
		return m_TabViewList.IndexOf(_view);
	}

	public bool ContainTabButton(TabButtonUI _button)
	{
		return m_TabButtonList.Contains(_button);
	}

	public bool ContainTabView(ITabView _view)
	{
		return m_TabViewList.Contains(_view);
	}

	public void AddTabButton(TabButtonUI _button)
	{
		if(ContainTabButton(_button))
		{
			return;
		}

		_button.SelectTapWithoutNotify(false,true);

		m_TabButtonList.Add(_button);

		_button.SetTabGroup(this);
	}

	public void AddTabView(ITabView _view)
	{
		m_TabViewList.AddNotOverlap(_view);
	}

	public void ForceSelect(TabButtonUI _button)
	{
		ForceSelect(m_TabButtonList.IndexOf(_button));
	}

	public void ForceSelect(int _index)
	{
		var selected = GetSelectedIndex();

		SelectInner(_index,_index == selected);
	}

	public void Select(TabButtonUI _button)
	{
		Select(m_TabButtonList.IndexOf(_button));
	}

	public void Select(int _index)
	{
		SelectInner(_index,false);
	}

	public void SelectInner(int _index,bool _force)
	{
		if(!m_TabButtonList.ContainsIndex(_index))
		{
			return;
		}

		if(!m_Initialized)
		{
			m_StartIndex = _index;
		}

		m_TabButtonList[_index].SelectTap(true,_force);
	}

	public void UnSelect()
	{
		if(!m_Initialized)
		{
			m_StartIndex = -1;

			return;
		}

		if(!m_SelectedTab)
		{
			return;
		}

		m_SelectedTab.SelectTap(false);
		m_SelectedTab = null;
	}

	public void NotifyTab(TabButtonUI _button)
	{
		for(var i=0;i<m_TabButtonList.Count;i++)
		{
			var button = m_TabButtonList[i];

			if(button.Equals(_button))
			{
				m_SelectedTab = _button;

				continue;
			}

			button.SelectTapWithoutNotify(false);
		}

		for(var i=0;i<m_TabViewList.Count;i++)
		{
			var view = m_TabViewList[i];

			if(view == null)
			{
				continue;
			}

			view.Notify(m_SelectedTab);
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		m_Initialized = false;

		Initialize();
	}
#endif
}