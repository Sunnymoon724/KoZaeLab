using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using KZLib.KZDevelop;

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
		private readonly HashSet<UINameType> m_dontReleaseHashSet = new();

		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<UINameType,WindowUI> m_registerWindowDict = new();

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
		public WindowUI Register(UINameType nameType,bool isActive = true)
		{
			if(!m_registerWindowDict.TryGetValue(nameType,out var window))
			{
				window = _MakeUI(nameType);

				m_registerWindowDict.Add(nameType,window);

				if(isActive)
				{
					var behaviour = window as MonoBehaviour;

					behaviour.gameObject.EnsureActive(false);
				}
			}

			return window;
		}

		private WindowUI _MakeUI(UINameType nameType)
		{
			var prefab = ResourceManager.In.GetObject(_GetUIPath(nameType));

			if(!prefab)
			{
				LogSvc.UI.E($"{nameType} is not exist.");

				return null;
			}

			if(!prefab.TryGetComponent<WindowUI>(out var window))
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"Component is not exist in {nameType}.");

				return null;
			}

			if(window.NameType != nameType)
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"UI nameType is not matched. [{window.NameType} != {nameType}]");

				return null;
			}

			transform.SetUIChild(prefab.transform);

			if(_IsDefinedUI(nameType))
			{
				RegisterDontRelease(nameType);
			}

			return window;
		}

		public void RegisterDontRelease(UINameType nameType)
		{
			m_dontReleaseHashSet.AddNotOverlap(nameType);
		}
		#endregion Register

		#region Open
		public void DelayOpen(UINameType nameType,object param,float delayTime)
		{
			void _Open()
			{
				Open(nameType,param);
			}

			CommonUtility.DelayAction(_Open,delayTime);
		}

		public IWindowUI Open(UINameType nameType,object param = null)
		{
			var window = _FindOpened(nameType);

			if(window == null)
			{
				window = Register(nameType,false);

				var repository = _GetRepository(window.Is3D);

				repository.Add(window);
				window.Open(param);
			}

			return window;
		}

		public bool IsOpened(UINameType nameType)
		{
			return _FindOpened(nameType) != null;
		}
		#endregion Open

		#region Close
		public void Close(UINameType nameType,bool isRelease = false)
		{
			var window = _FindOpened(nameType);

			if(window != null)
			{
				Close(window,isRelease);
			}
		}

		public void Close(WindowUI window,bool isRelease = false)
		{
			if(!window)
			{
				return;
			}

			var release = (isRelease || !window.IsPooling) && !m_dontReleaseHashSet.Contains(window.NameType);

			if(release)
			{
				m_registerWindowDict.Remove(window.NameType);
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

		#region Show
		public void Show(UINameType nameType)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Show(nameType);
			}
		}

		public void ShowAllGroup(IEnumerable<UINameType> includeNameTypeGroup = null,IEnumerable<UINameType> excludeNameTypeGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].ShowAllGroup(includeNameTypeGroup,excludeNameTypeGroup);
			}
		}
		#endregion Show

		#region Hide
		public void Hide(UINameType nameType)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Hide(nameType);
			}
		}

		public void HideAllGroup(IEnumerable<UINameType> includeNameTypeGroup = null,IEnumerable<UINameType> excludeNameTypeGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].HideAllGroup(includeNameTypeGroup,excludeNameTypeGroup);
			}
		}

		public bool? IsHidden(UINameType nameType)
		{
			var window = _FindOpened(nameType);

			return window == null ? null : window.IsHidden;
		}
		#endregion Hide

		#region Find
		public TNameType Find<TNameType>(UINameType nameType) where TNameType : class,IWindowUI
		{
			var window = _FindOpened(nameType);

			return (window != null) ? window as TNameType : null;
		}

		private WindowUI _FindOpened(UINameType nameType)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				var window = m_repositoryList[i].FindOpenedUI(nameType);

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
			var window = repository.TopOpenedWindow as WindowUI2D;

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