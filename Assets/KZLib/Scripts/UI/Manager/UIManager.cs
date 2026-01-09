using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		//? Canvas List
		private readonly List<RepositoryUI> m_repositoryList = new();

		[SerializeField]
		private RepositoryUI2D m_repository2D = null; // camera -> overlay
		[SerializeField]
		private RepositoryUI3D m_repository3D = null; // camera -> world space


		[ShowInInspector,ListDrawerSettings(ShowFoldout = false),ReadOnly]
		private readonly HashSet<CommonUINameTag> m_dontReleaseHashSet = new();

		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<CommonUINameTag,Window> m_registerWindowDict = new();

		private string m_prefabPath = null;

		protected override void Initialize()
		{
			if(m_repository2D)
			{
				m_repositoryList.Add(m_repository2D);
			}

			if(m_repository3D)
			{
				m_repositoryList.Add(m_repository3D);
			}

			m_prefabPath = ConfigManager.In.Access<GameConfig>().UIPrefabPath;
		}

		protected override void Release()
		{
			m_dontReleaseHashSet.Clear();
		}

		public void Set3DCamera(Camera camera)
		{
			if(m_repository3D)
			{
				m_repository3D.SetCamera(camera);
			}
		}

		public Camera Get3DCamera()
		{
			return m_repository3D != null ? m_repository3D.CanvasCamera : null;
		}

		#region Register
		/// <summary>
		/// Register -> Not open
		/// </summary>
		public Window Register(CommonUINameTag nameTag,bool isActive = true)
		{
			if(!m_registerWindowDict.TryGetValue(nameTag,out var window))
			{
				window = _MakeUI(nameTag);

				m_registerWindowDict.Add(nameTag,window);

				if(isActive)
				{
					var behaviour = window as MonoBehaviour;

					behaviour.gameObject.EnsureActive(false);
				}
			}

			return window;
		}

		private Window _MakeUI(CommonUINameTag nameTag)
		{
			var prefab = ResourceManager.In.GetObject(_GetUIPath(nameTag));

			if(!prefab)
			{
				LogSvc.UI.E($"{nameTag} is not exist.");

				return null;
			}

			if(!prefab.TryGetComponent<Window>(out var window))
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"Component is not exist in {nameTag}.");

				return null;
			}

			if(window.NameTag != nameTag)
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"UI nameTag is not matched. [{window.NameTag} != {nameTag}]");

				return null;
			}

			transform.SetChild(prefab.transform,false);

			if(_IsDefinedUI(nameTag))
			{
				RegisterDontRelease(nameTag);
			}

			return window;
		}

		public void RegisterDontRelease(CommonUINameTag nameTag)
		{
			m_dontReleaseHashSet.AddNotOverlap(nameTag);
		}
		#endregion Register

		#region Open
		public void DelayOpen(CommonUINameTag nameTag,object param,float delayTime)
		{
			void _Open()
			{
				Open(nameTag,param);
			}

			CommonUtility.DelayAction(_Open,delayTime);
		}

		public IWindow Open(CommonUINameTag nameTag,object param = null)
		{
			var window = _FindOpened(nameTag);

			if(window == null)
			{
				window = Register(nameTag,false);

				var repository = _GetRepository(window.Is3D);

				repository.Add(window);
				window.Open(param);
			}

			return window;
		}

		public bool IsOpened(CommonUINameTag nameTag)
		{
			return _FindOpened(nameTag) != null;
		}
		#endregion Open

		#region Close
		public void Close(CommonUINameTag nameTag,bool isRelease = false)
		{
			var window = _FindOpened(nameTag);

			if(window != null)
			{
				Close(window,isRelease);
			}
		}

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

		public void CloseAllOpenedIn3D(bool isRelease = false)
		{
			_CloseAllOpened(true,isRelease);
		}

		public void CloseAllOpenedIn2D(bool isRelease = false)
		{
			_CloseAllOpened(false,isRelease);
		}

		private void _CloseAllOpened(bool is3D,bool isRelease = false)
		{
			var repository = _GetRepository(is3D);

			foreach(var window in repository.OpenedWindowGroup)
			{
				Close(window,isRelease);
			}
		}
		#endregion Close

		#region Hide
		public void Hide(CommonUINameTag nameTag,bool isHidden)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Hide(nameTag,isHidden);
			}
		}

		public void HideAllGroup(bool isHidden,IEnumerable<CommonUINameTag> includeNameTagGroup = null,IEnumerable<CommonUINameTag> excludeNameTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].HideAllGroup(isHidden,includeNameTagGroup,excludeNameTagGroup);
			}
		}

		public bool? IsHidden(CommonUINameTag nameTag)
		{
			var window = _FindOpened(nameTag);

			return window == null ? null : window.IsHidden;
		}
		#endregion Hide

		#region Find
		public IWindow Find(CommonUINameTag nameTag)
		{
			return _FindOpened(nameTag);
		}

		private Window _FindOpened(CommonUINameTag nameTag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				var window = m_repositoryList[i].FindOpenedUI(nameTag);

				if(window != null)
				{
					return window;
				}
			}

			return null;
		}
		#endregion Find

		public void BlockInput(bool isBlocked)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].BlockInput(isBlocked);
			}
		}

		public void PressBackButton()
		{
			var repository = _GetRepository(false);
			var window = repository.TopOpenedWindow as Window2D;

			if(window != null)
			{
				window.PressBackButton();
			}
		}

		private RepositoryUI _GetRepository(bool is3D)
		{
			return is3D ? m_repository3D : m_repository2D;
		}
	}
}