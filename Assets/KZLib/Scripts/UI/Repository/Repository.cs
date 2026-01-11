using System.Collections.Generic;
using System.Linq;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class Repository : BaseComponent
{
	[InfoBox("canvas is null",InfoMessageType.Error,nameof(IsValidCanvas))]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_canvas = null;
	private bool IsValidCanvas => m_canvas == null;

	[SerializeField]
	private Transform m_storage = null;
	public Transform Storage => m_storage != null ? m_storage : transform;

	//? Current Opened Window List
	[SerializeField,ListDrawerSettings(IsReadOnly = true)]
	protected List<Window> m_windowList = new();
	public IEnumerable<Window> WindowGroup => m_windowList;
	public Window TopWindow => m_windowList.Count != 0 ? m_windowList[0] : null;

	protected abstract bool IsValid(Window window);
	public abstract void Add(Window window);

	protected override void _Release()
	{
		base._Release();

		RemoveAll(null,true);

		m_windowList.Clear();
	}

	protected void _Add(Window window)
	{
		window.transform.SetAsLastSibling();

		window.SetCanvas(m_canvas);

		m_windowList.AddNotOverlap(window);
	}

	public void Remove(Window window,bool isRelease)
	{
		if(!IsValid(window))
		{
			return;
		}

		m_windowList.Remove(window);

		if(isRelease)
		{
			window.gameObject.DestroyObject();
		}
	}

	public Window FindOpenedWindow(CommonUINameTag nameTag)
	{
		bool _FindOpened(Window openedWindow)
		{
			return openedWindow.NameTag == nameTag;
		}

		return m_windowList.Find(_FindOpened);
	}

	public void RemoveAll(IEnumerable<CommonUINameTag> excludeNameTagGroup,bool isRelease)
	{
		var nameTagHashSet = new HashSet<CommonUINameTag>(excludeNameTagGroup ?? Enumerable.Empty<CommonUINameTag>());

		for(var i=m_windowList.Count-1;i>=0;i--)
		{
			var window = m_windowList[i];

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
			CommonUINameTag.CommonTransitionPanel,
			CommonUINameTag.DebugOverlayPanel,
		};

		for(var i=0;i<m_windowList.Count;i++)
		{
			var window = m_windowList[i];

			if(window == null || !includeNameTagHashSet.Contains(window.NameTag) || excludeNameTagHashSet.Contains(window.NameTag))
			{
				continue;
			}

			_Hide(window,isHidden);
		}
	}

	public void Hide(CommonUINameTag nameTag,bool isHidden)
	{
		var window = FindOpenedWindow(nameTag);

		_Hide(window,isHidden);
	}

	private void _Hide(Window window,bool isHidden)
	{
		if(window == null || window.IsIgnoreHide || window.IsHidden == isHidden)
		{
			return;
		}

		window.Hide(isHidden);
	}

	public void BlockInput(bool isBlocked)
	{
		for(var i=0;i<m_windowList.Count;i++)
		{
			m_windowList[i].BlockInput(isBlocked);
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