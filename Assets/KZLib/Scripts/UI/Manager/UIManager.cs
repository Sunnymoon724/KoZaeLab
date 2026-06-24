using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.Utilities;
using KZLib.Data;
using KZLib.UI;
using System;

namespace KZLib
{
	/// <summary>
	/// Central hub for UI window lifecycle: create, register, open, close, hide, and query.
	/// Supports separate 2D (screen overlay) and 3D (world space) repositories.
	/// </summary>
	[SingletonConfig(AutoCreate = true,PrefabPath = "Prefab/UIManager",DontDestroy = true)]
	public partial class UIManager : SingletonMB<UIManager>
	{
		// Active repositories used for iteration (Hide, Find, BlockUI, etc.).
		private readonly List<Repository> m_repositoryList = new();

		[SerializeField]
		private Repository2D m_repository2D = null; // Screen-space overlay canvas.
		[SerializeField]
		private Repository3D m_repository3D = null; // World-space canvas bound to a scene camera.

		// System windows that must survive Close even when isRelease is requested.
		[ShowInInspector,ListDrawerSettings(ShowFoldout = false),ReadOnly]
		private readonly HashSet<CommonUINameTag> m_dontReleaseHashSet = new();

		// Pooled / cached window instances keyed by name tag (registered but not necessarily opened).
		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<CommonUINameTag,Window> m_registerWindowDict = new();

		// Root prefab path for game-specific UI (common UI uses a fixed path; see UIManager_Utility).
		private string m_prefabPath = null;

		protected override void _Initialize()
		{
			base._Initialize();

			if(m_repository2D)
			{
				m_repositoryList.Add(m_repository2D);
			}

			if(m_repository3D)
			{
				m_repositoryList.Add(m_repository3D);
			}

			m_prefabPath = ConfigManager.In.Fetch<GameConfig>().UIPrefabPath;
		}

		protected override void _Release()
		{
			CloseAllOpenedIn2D(true);
			CloseAllOpenedIn3D(true);

			// Destroy registered-but-closed instances that were not removed by Close.
			foreach(var window in m_registerWindowDict.Values)
			{
				if(window)
				{
					window.gameObject.DestroyObject();
				}
			}

			m_registerWindowDict.Clear();
			m_dontReleaseHashSet.Clear();

			base._Release();
		}

		/// <summary>
		/// Assigns the camera used by the 3D world-space UI canvas.
		/// </summary>
		public void Set3DCamera(Camera camera)
		{
			if(m_repository3D)
			{
				m_repository3D.SetCamera(camera);
			}
		}

		/// <summary>
		/// Returns the camera currently bound to the 3D repository, or null when unavailable.
		/// </summary>
		public Camera Get3DCamera()
		{
			return m_repository3D != null ? m_repository3D.CanvasCamera : null;
		}

		#region Register
		/// <summary>
		/// Instantiates (or returns a cached instance of) a window without opening it.
		/// The window is deactivated and kept in storage until <see cref="Open"/> is called.
		/// </summary>
		public Window Register(CommonUINameTag nameTag)
		{
			if(!m_registerWindowDict.TryGetValue(nameTag,out var window))
			{
				window = _MakeWindow(nameTag);

				m_registerWindowDict.Add(nameTag,window);

				window.gameObject.EnsureActive(false);
			}

			return window;
		}

		/// <summary>
		/// Loads a window prefab, validates it, and parents it under the matching storage transform.
		/// </summary>
		private Window _MakeWindow(CommonUINameTag nameTag)
		{
			var prefab = ResourceManager.In.GetObject(_GetPrefabPath(nameTag));

			if(!prefab)
			{
				throw new InvalidOperationException($"{nameTag} does not exist.");
			}

			if(!prefab.TryGetComponent<Window>(out var window))
			{
				prefab.DestroyObject();

				throw new InvalidOperationException($"Component does not exist in {nameTag}.");
			}

			if(window.NameTag != nameTag)
			{
				prefab.DestroyObject();

				throw new InvalidOperationException($"UI nameTag does not match. [{window.NameTag} != {nameTag}]");
			}

			var storage = GetStorage(window.Is3D);

			storage.SetChild(prefab.transform,false);

			if(_IsDefinedWindow(nameTag))
			{
				RegisterDontRelease(nameTag);
			}

			return window;
		}

		/// <summary>
		/// Marks a window so that <see cref="Close"/> never destroys its cached instance.
		/// </summary>
		public void RegisterDontRelease(CommonUINameTag nameTag)
		{
			m_dontReleaseHashSet.AddIfAbsent(nameTag);
		}
		#endregion Register

		#region Open
		/// <summary>
		/// Schedules <see cref="Open"/> after the given delay.
		/// </summary>
		/// <returns>Dispose to cancel the scheduled open.</returns>
		public IDisposable DelayOpen(CommonUINameTag nameTag,object param,float delayTime)
		{
			void _Open()
			{
				Open(nameTag,param);
			}

			return KZExternalKit.DelayAction(_Open,delayTime);
		}

		/// <summary>
		/// Opens a window, adding it to the appropriate repository when it is not already open.
		/// Always invokes <c>window.Open(param)</c>, including when the window is already open.
		/// </summary>
		public IWindow Open(CommonUINameTag nameTag,object param = null)
		{
			var window = _FindOpened(nameTag);

			if(window == null)
			{
				window = Register(nameTag);

				var repository = _GetRepository(window.Is3D);

				repository.Add(window);
			}

			window.Open(param);

			return window;
		}

		/// <summary>
		/// Returns whether the window is currently present in an open repository.
		/// </summary>
		public bool IsOpened(CommonUINameTag nameTag)
		{
			return _FindOpened(nameTag) != null;
		}
		#endregion Open

		#region Close
		/// <summary>
		/// Closes an open window by name tag. No-op when the window is not open.
		/// </summary>
		/// <param name="isRelease">When true, destroys non-pooled instances and removes them from the register cache.</param>
		public void Close(CommonUINameTag nameTag,bool isRelease = false)
		{
			var window = _FindOpened(nameTag);

			if(window != null)
			{
				Close(window,isRelease);
			}
		}

		/// <summary>
		/// Closes a window and removes it from its repository.
		/// Pooling windows stay in <see cref="m_registerWindowDict"/> unless release conditions are met.
		/// </summary>
		public void Close(Window window,bool isRelease = false)
		{
			if(!window)
			{
				return;
			}

			var release = (isRelease || !window.IsPooling) && !m_dontReleaseHashSet.Contains(window.NameTag);

			if(release)
			{
				m_registerWindowDict.Remove(window.NameTag);
			}

			window.Close();

			var repository = _GetRepository(window.Is3D);

			repository.Remove(window,release);
		}

		/// <summary>
		/// Closes every open 3D window.
		/// </summary>
		public void CloseAllOpenedIn3D(bool isRelease = false)
		{
			_CloseAllOpened(true,isRelease);
		}

		/// <summary>
		/// Closes every open 2D window.
		/// </summary>
		public void CloseAllOpenedIn2D(bool isRelease = false)
		{
			_CloseAllOpened(false,isRelease);
		}

		// Snapshot before closing; Close mutates the repository window list.
		private void _CloseAllOpened(bool is3D,bool isRelease = false)
		{
			var repository = _GetRepository(is3D);
			var windowList = new List<Window>(repository.WindowGroup);

			for(var i=windowList.Count-1;i>=0;i--)
			{
				Close(windowList[i],isRelease);
			}
		}
		#endregion Close

		#region Hide
		/// <summary>
		/// Shows or hides an open window across all repositories (2D and 3D).
		/// </summary>
		public void Hide(CommonUINameTag nameTag,bool isHidden)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Hide(nameTag,isHidden);
			}
		}

		/// <summary>
		/// Shows or hides a filtered group of open windows.
		/// </summary>
		public void HideAllGroup(bool isHidden,IEnumerable<CommonUINameTag> includeNameTagGroup = null,IEnumerable<CommonUINameTag> excludeNameTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].HideAllGroup(isHidden,includeNameTagGroup,excludeNameTagGroup);
			}
		}

		/// <summary>
		/// Returns the hidden state of an open window, or null when the window is not open.
		/// </summary>
		public bool? IsHidden(CommonUINameTag nameTag)
		{
			var window = _FindOpened(nameTag);

			return window == null ? null : window.IsHidden;
		}
		#endregion Hide

		#region Find
		/// <summary>
		/// Returns an open window by name tag, or null when it is not open.
		/// </summary>
		public IWindow Find(CommonUINameTag nameTag)
		{
			return _FindOpened(nameTag);
		}

		private Window _FindOpened(CommonUINameTag nameTag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				var window = m_repositoryList[i].FindOpenedWindow(nameTag);

				if(window != null)
				{
					return window;
				}
			}

			return null;
		}
		#endregion Find

		/// <summary>
		/// Blocks or restores input on every open window in all repositories.
		/// </summary>
		public void BlockUI(bool isBlocked)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].BlockUI(isBlocked);
			}
		}

		/// <summary>
		/// Forwards the back-button action to the topmost open 2D window.
		/// 3D UI is intentionally excluded from back-button handling.
		/// </summary>
		public void PressBackButton()
		{
			var repository = _GetRepository(false);
			var window = repository.TopWindow as Window2D;

			if(window != null)
			{
				window.PressBackButton();
			}
		}

		private Repository _GetRepository(bool is3D)
		{
			return is3D ? m_repository3D : m_repository2D;
		}

		/// <summary>
		/// Returns the inactive storage transform for pooled / registered windows.
		/// </summary>
		public Transform GetStorage(bool is3D)
		{
			var repository = _GetRepository(is3D);

			return repository.Storage;
		}
	}
}
