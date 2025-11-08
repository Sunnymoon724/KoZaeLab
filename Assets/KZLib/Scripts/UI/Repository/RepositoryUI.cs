using System.Collections.Generic;
using System.Linq;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

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

	public WindowUI FindOpenedUI(string tag)
	{
		return m_openedWindowList.Find(x=>x.Tag == tag);
	}

	public void RemoveAll(IEnumerable<string> excludeTagGroup,bool isRelease)
	{
		var tagHashSet = new HashSet<string>(excludeTagGroup ?? Enumerable.Empty<string>());

		for(var i=m_openedWindowList.Count-1;i>=0;i--)
		{
			var window = m_openedWindowList[i];

			if(tagHashSet.Contains(window.Tag))
			{
				continue;
			}

			Remove(window,isRelease);
		}
	}

	public void ShowAllGroup(IEnumerable<string> includeTagGroup = null,IEnumerable<string> excludeTagGroup = null)
	{
		_SetVisibleGroup(false,includeTagGroup,excludeTagGroup);
	}

	public void HideAllGroup(IEnumerable<string> includeTagGroup = null,IEnumerable<string> excludeTagGroup = null)
	{
		_SetVisibleGroup(true,includeTagGroup,excludeTagGroup);
	}

	private void _SetVisibleGroup(bool isHidden,IEnumerable<string> includeTagGroup = null,IEnumerable<string> excludeTagGroup = null)
	{
		var includeTagHashSet = new HashSet<string>(includeTagGroup ?? Enumerable.Empty<string>());
		var excludeTagHashSet = new HashSet<string>(excludeTagGroup ?? Enumerable.Empty<string>())
        {
            Global.TRANSITION_PANEL_UI,
            Global.HUD_PANEL_UI,
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

	public void Show(string tag)
	{
		var window = FindOpenedUI(tag);

		_SetVisible(window,false);
	}

	public void Hide(string tag)
	{
		var window = FindOpenedUI(tag);

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