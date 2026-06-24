using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using KZLib.UI;

/// <summary>
/// Base canvas repository that tracks open windows, manages draw order, and handles hide / input-block operations.
/// Concrete subclasses place windows under dimension-specific hierarchies (2D priority layers or 3D world space).
/// </summary>
[RequireComponent(typeof(Canvas))]
public abstract class Repository : MonoBehaviour
{
	[InfoBox("canvas is null",InfoMessageType.Error,nameof(IsValidCanvas))]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_canvas = null;
	private bool IsValidCanvas => m_canvas == null;

	// Inactive parent for registered-but-closed windows (falls back to this transform).
	[SerializeField]
	private Transform m_storage = null;
	public Transform Storage => m_storage != null ? m_storage : transform;

	// Runtime-only open window stack; inspector-visible for debugging (not serialized to prefab).
	[ShowInInspector,ListDrawerSettings(IsReadOnly = true),ReadOnly]
	protected List<Window> m_windowList = new();
	public IEnumerable<Window> WindowGroup => m_windowList;

	/// <summary>Most recently opened window (top of the stack).</summary>
	public Window TopWindow => m_windowList.Count != 0 ? m_windowList[^1] : null;

	protected abstract bool IsValid(Window window);
	public abstract void Add(Window window);

	private void OnDestroy()
	{
		// UIManager._Release destroys window instances; clear references only.
		m_windowList.Clear();
	}

	/// <summary>
	/// Registers a window in the open list, brings it to the front, and binds the repository canvas.
	/// </summary>
	protected void _Add(Window window)
	{
		window.transform.SetAsLastSibling();

		window.SetCanvas(m_canvas);

		m_windowList.AddIfAbsent(window);
	}

	/// <summary>
	/// Removes a window from the open list and optionally destroys its GameObject.
	/// </summary>
	public void Remove(Window window,bool isRelease)
	{
		if(!window || !m_windowList.Remove(window))
		{
			return;
		}

		if(isRelease)
		{
			window.gameObject.DestroyObject();
		}
	}

	/// <summary>
	/// Returns an open window matching the given name tag, or null when it is not open in this repository.
	/// </summary>
	public Window FindOpenedWindow(CommonUINameTag nameTag)
	{
		bool _FindOpened(Window openedWindow)
		{
			return openedWindow != null && openedWindow.NameTag == nameTag;
		}

		return m_windowList.Find(_FindOpened);
	}

	/// <summary>
	/// Closes every open window except those whose name tags appear in <paramref name="excludeNameTagGroup"/>.
	/// </summary>
	public void RemoveAll(IEnumerable<CommonUINameTag> excludeNameTagGroup,bool isRelease)
	{
		var nameTagHashSet = new HashSet<CommonUINameTag>(excludeNameTagGroup ?? new CommonUINameTag[0]);

		for(var i=m_windowList.Count-1;i>=0;i--)
		{
			var window = m_windowList[i];

			if(!window)
			{
				m_windowList.RemoveAt(i);

				continue;
			}

			if(nameTagHashSet.Contains(window.NameTag))
			{
				continue;
			}

			Remove(window,isRelease);
		}
	}

	/// <summary>
	/// Shows or hides open windows whose name tags are listed in <paramref name="includeNameTagGroup"/>.
	/// When include is null or empty, no windows are affected.
	/// Transition and debug-overlay panels are always excluded.
	/// </summary>
	public void HideAllGroup(bool isHidden,IEnumerable<CommonUINameTag> includeNameTagGroup = null,IEnumerable<CommonUINameTag> excludeNameTagGroup = null)
	{
		var includeNameTagHashSet = new HashSet<CommonUINameTag>(includeNameTagGroup ?? new CommonUINameTag[0]);
		var excludeNameTagHashSet = new HashSet<CommonUINameTag>(excludeNameTagGroup ?? new CommonUINameTag[0])
		{
			CommonUINameTag.TransitionPanel,
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

	/// <summary>
	/// Shows or hides a single open window by name tag.
	/// </summary>
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

	/// <summary>
	/// Blocks or restores input on every open window in this repository.
	/// </summary>
	public void BlockUI(bool isBlocked)
	{
		for(var i=0;i<m_windowList.Count;i++)
		{
			var window = m_windowList[i];

			if(window == null)
			{
				continue;
			}

			window.BlockUI(isBlocked);
		}
	}

	private void Reset()
	{
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
