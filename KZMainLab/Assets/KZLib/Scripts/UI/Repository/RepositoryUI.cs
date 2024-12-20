﻿using System.Collections.Generic;
using System.Linq;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class RepositoryUI : SortingLayerCanvas
{
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected CanvasGroup m_CanvasGroup = null;

	public Camera CanvasCamera => m_Canvas.worldCamera;

	//? Current Opened Window List
	[SerializeField,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout)]
	protected List<WindowUI> m_OpenedWindowList = new();

	protected abstract bool IsValid(WindowUI _window);

	public void BlockInput(bool _block)
	{
		m_CanvasGroup.interactable = !_block;
	}

	protected override void Initialize()
	{
		base.Initialize();

		m_Canvas.sortingOrder = SortingLayerOrder;

		if(CanvasCamera != null && CameraMgr.HasInstance)
		{
			CameraMgr.In.AddSubCamera(CanvasCamera,false);
		}
	}

	protected override void Release()
	{
		base.Release();

		RemoveAll(null,true,true);

		m_OpenedWindowList.Clear();
	}

	public void Add(WindowUI _window)
	{
		if(!IsValid(_window))
		{
			return;
		}

		transform.SetUIChild(_window.transform);

		_window.SetParentSortingLayer(this,GetUIOrder(_window.Tag,_window.Priority));
		_window.transform.SetAsLastSibling();

		m_OpenedWindowList.AddNotOverlap(_window);
	}

	public void Remove(WindowUI _window,bool _release)
	{
		if(!IsValid(_window))
		{
			return;
		}

		m_OpenedWindowList.Remove(_window);

		if(_release)
		{
			if(ResMgr.HasInstance)
			{
				CommonUtility.DestroyObject(_window.gameObject);
			}
		}
	}

	private int GetUIOrder(UITag _tag,UIPriorityType _priority)
	{
		var priority = (int) _priority;

		if(_tag == UITag.HudPanelUI)
		{
			return 3*priority;
		}
		else if(_tag == UITag.TransitionPanelUI)
		{
			return 2*priority;
		}

		var dataList = new List<WindowUI>(m_OpenedWindowList.Where(x=>x.Priority == _priority && x.Tag != UITag.HudPanelUI && x.Tag != UITag.TransitionPanelUI));

		if(dataList.Count < 1)
		{
			return priority;
		}

		return dataList.Max(x=>x.LocalSortingLayerOrder)+SORTING_INTERVAL;
	}

	public WindowUI GetOpenedUI(UITag _tag)
	{
		return m_OpenedWindowList.Find(x=>x.Tag == _tag);
	}

	public void RemoveAll(IEnumerable<UITag> _excludeGroup,bool _release,bool _includeHide)
	{
		var excludeSet = new HashSet<UITag>(_excludeGroup ?? Enumerable.Empty<UITag>());

		foreach(var window in new List<WindowUI>(m_OpenedWindowList))
		{
			if(excludeSet.Contains(window.Tag) && !_includeHide && window.IsHide)
			{
				continue;
			}

			Remove(window,_release);
		}
	}

	public IEnumerable<UITag> SetHideAll(bool _hide,IEnumerable<UITag> _includeGroup = null,IEnumerable<UITag> _excludeGroup = null)
	{
		var tagList = new List<UITag>();
		var excludeSet = new HashSet<UITag>(_excludeGroup ?? Enumerable.Empty<UITag>());

		excludeSet.AddNotOverlap(UITag.HudPanelUI);
        excludeSet.AddNotOverlap(UITag.TransitionPanelUI);

		foreach(var window in m_OpenedWindowList)
		{
			if(window == null || window.IsIgnoreHide || _hide == window.IsHide || (_includeGroup != null && !_includeGroup.Contains(window.Tag)) || excludeSet.Contains(window.Tag))
			{
				continue;
			}

			window.Hide(_hide);
			tagList.Add(window.Tag);
		}

		return tagList;
	}

	public UITag SetHide(UITag _tag,bool _hide)
	{
		var window = GetOpenedUI(_tag);

		if(window == null || window.IsIgnoreHide || _hide == window.IsHide)
		{
			return null;
		}

		window.Hide(_hide);

		return window.Tag;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_CanvasGroup)
		{
			m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
	}
}