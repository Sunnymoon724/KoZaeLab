using System.Collections.Generic;
using System.Linq;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public abstract class RepositoryUI : BaseComponentUI
{
	[InfoBox("canvas is null",InfoMessageType.Error,"@this.m_canvas == null")]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_canvas = null;

	public Camera CanvasCamera => m_canvas.worldCamera;

	//? Current Opened Window List
	[SerializeField,ListDrawerSettings(IsReadOnly = true)]
	protected List<WindowUI> m_openedWindowList = new();

	protected abstract bool IsValid(WindowUI windowUI);
	public abstract void Add(WindowUI windowUI);

	protected override void Initialize()
	{
		base.Initialize();

		if(CanvasCamera != null && CameraManager.HasInstance)
		{
			CameraManager.In.AddSubCamera(CanvasCamera,false);
		}
	}

	protected override void Release()
	{
		base.Release();

		RemoveAll(null,true);

		m_openedWindowList.Clear();
	}

	protected void _Add(WindowUI window)
	{
		window.transform.SetAsLastSibling();

		window.SetCanvas(m_canvas);

		m_openedWindowList.AddNotOverlap(window);
	}

	public void Remove(WindowUI windowUI,bool isRelease)
	{
		if(!IsValid(windowUI))
		{
			return;
		}

		m_openedWindowList.Remove(windowUI);

		if(isRelease)
		{
			if(ResourceManager.HasInstance)
			{
				windowUI.gameObject.DestroyObject();
			}
		}
	}

	public WindowUI FindOpenedUI(UITag uiTag)
	{
		return m_openedWindowList.Find(x=>x.Tag == uiTag);
	}

	public void RemoveAll(IEnumerable<UITag> excludeTagGroup,bool isRelease)
	{
		var tagHashSet = new HashSet<UITag>(excludeTagGroup ?? Enumerable.Empty<UITag>());

		foreach(var window in new List<WindowUI>(m_openedWindowList))
		{
			if(tagHashSet.Contains(window.Tag))
			{
				continue;
			}

			Remove(window,isRelease);
		}
	}

	public void ShowAllGroup(IEnumerable<UITag> includeTagGroup = null,IEnumerable<UITag> excludeTagGroup = null)
	{
		_SetVisibleGroup(false,includeTagGroup,excludeTagGroup);
	}

	public void HideAllGroup(IEnumerable<UITag> includeTagGroup = null,IEnumerable<UITag> excludeTagGroup = null)
	{
		_SetVisibleGroup(true,includeTagGroup,excludeTagGroup);
	}

	public void _SetVisibleGroup(bool isHidden,IEnumerable<UITag> includeTagGroup = null,IEnumerable<UITag> excludeTagGroup = null)
	{
		var includeTagHashSet = new HashSet<UITag>(includeTagGroup ?? Enumerable.Empty<UITag>());
		var excludeTagHashSet = new HashSet<UITag>(excludeTagGroup ?? Enumerable.Empty<UITag>())
		{
			// add default exclude tag
			UITag.HudPanelUI,
			UITag.TransitionPanelUI
		};

		foreach(var window in m_openedWindowList)
		{
			if(window == null || !includeTagHashSet.Contains(window.Tag) || excludeTagHashSet.Contains(window.Tag))
			{
				continue;
			}

			_SetVisible(window,isHidden);
		}
	}

	public void Show(UITag uiTag)
	{
		var window = FindOpenedUI(uiTag);

		_SetVisible(window,false);
	}

	public void Hide(UITag uiTag)
	{
		var window = FindOpenedUI(uiTag);

		_SetVisible(window,true);
	}

	private void _SetVisible(WindowUI window,bool isHidden)
	{
		if(window == null || window.IsIgnoreHide || window.IsHidden == isHidden)
		{
			return;
		}

		if(isHidden)
		{
			window.Hide();
		}
		else
		{
			window.Show();
		}
	}

	public void BlockInput()
	{
		foreach(var window in m_openedWindowList)
		{
			window.BlockInput();
		}
	}

	public void AllowInput()
	{
		foreach(var window in m_openedWindowList)
		{
			window.AllowInput();
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_canvas)
		{
			m_canvas = gameObject.GetComponent<Canvas>();
		}

		m_canvas.overrideSorting = true;
		m_canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

		if(gameObject.TryGetComponent<GraphicRaycaster>(out var raycaster))
		{
			raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		}
	}
}