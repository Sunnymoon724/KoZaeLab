using System.Collections.Generic;
using System.Linq;
using KZLib;
using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class RepositoryUI : BaseComponentUI
{
	[InfoBox("canvas is null",InfoMessageType.Error,nameof(_IsValidCanvas))]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_canvas = null;

	private bool _IsValidCanvas()
	{
		return m_canvas == null;
	}

	//? Current Opened Window List
	[SerializeField,ListDrawerSettings(IsReadOnly = true)]
	protected List<WindowUI> m_openedWindowList = new();
	public IEnumerable<WindowUI> OpenedWindowGroup => m_openedWindowList;
	public WindowUI TopOpenedWindow => m_openedWindowList.Count != 0 ? m_openedWindowList[0] : null;

	protected abstract bool IsValid(WindowUI windowUI);
	public abstract void Add(WindowUI windowUI);

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

	public WindowUI FindOpenedUI(CommonUINameTag nameTag)
	{
		bool _FindOpened(WindowUI openedWindow)
		{
			return openedWindow.NameTag == nameTag;
		}

		return m_openedWindowList.Find(_FindOpened);
	}

	public void RemoveAll(IEnumerable<CommonUINameTag> excludeNameTagGroup,bool isRelease)
	{
		var nameTagHashSet = new HashSet<CommonUINameTag>(excludeNameTagGroup ?? Enumerable.Empty<CommonUINameTag>());

		for(var i=m_openedWindowList.Count-1;i>=0;i--)
		{
			var window = m_openedWindowList[i];

			if(nameTagHashSet.Contains(window.NameTag))
			{
				continue;
			}

			Remove(window,isRelease);
		}
	}

	public void HideAllGroup(bool isHidden,IEnumerable<CommonUINameTag> includeNameTagGroup = null,IEnumerable<CommonUINameTag> excludeNameTagGroup = null)
	{
		var includeNameTagHashSet = new HashSet<CommonUINameTag>(includeNameTagGroup ?? Enumerable.Empty<CommonUINameTag>());
		var excludeNameTagHashSet = new HashSet<CommonUINameTag>(excludeNameTagGroup ?? Enumerable.Empty<CommonUINameTag>())
		{
			CommonUINameTag.CommonTransitionPanelUI,
			CommonUINameTag.HudPanelUI,
		};

		for(var i=0;i<m_openedWindowList.Count;i++)
		{
			var window = m_openedWindowList[i];

			if(window == null || !includeNameTagHashSet.Contains(window.NameTag) || excludeNameTagHashSet.Contains(window.NameTag))
			{
				continue;
			}

			_Hide(window,isHidden);
		}
	}

	public void Hide(CommonUINameTag nameTag,bool isHidden)
	{
		var window = FindOpenedUI(nameTag);

		_Hide(window,isHidden);
	}

	private void _Hide(WindowUI window,bool isHidden)
	{
		if(window == null || window.IsIgnoreHide || window.IsHidden == isHidden)
		{
			return;
		}

		window.Hide(isHidden);
	}

	public void BlockInput(bool isBlocked)
	{
		for(var i=0;i<m_openedWindowList.Count;i++)
		{
			m_openedWindowList[i].BlockInput(isBlocked);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_canvas)
		{
			m_canvas = GetComponent<Canvas>();
		}

		m_canvas.overrideSorting = true;
		m_canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

		if(gameObject.TryGetComponent<GraphicRaycaster>(out var raycaster))
		{
			raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		}
	}
}