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

		[SerializeField] private RepositoryUI2D m_repository2D = null; // camera -> overlay
		[SerializeField] private RepositoryUI3D m_repository3D = null; // camera -> world space


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

				if(window.Is3D)
				{
					m_repository3D.Add(window);
				}
				else
				{
					m_repository2D.Add(window);
				}

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

		public void Close(WindowUI windowUI,bool isRelease = false)
		{
			var release = (isRelease || !windowUI.IsPooling) && !m_dontReleaseHashSet.Contains(windowUI.NameType);

			if(release)
			{
				m_registerWindowDict.Remove(windowUI.NameType);
			}

			windowUI.Close();

			if(windowUI.Is3D)
			{
				m_repository3D.Remove(windowUI,release);
			}
			else
			{
				m_repository2D.Remove(windowUI,release);
			}
		}

		public void CloseAllOpened(bool isRelease,bool isExcludeClose)
		{
			foreach(var (_,window) in m_registerWindowDict)
			{
				if(isExcludeClose)
				{
					continue;
				}

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
			foreach(var repository in m_repositoryList)
			{
				var window = repository.FindOpenedUI(nameType);

				if(window != null)
				{
					return window;
				}
			}

			return null;
		}
		#endregion Find

		#region Block Input
		public void BlockInput()
		{
			foreach(var repository in m_repositoryList)
			{
				repository.BlockInput();
			}
		}

		public void AllowInput()
		{
			foreach(var repository in m_repositoryList)
			{
				repository.AllowInput();
			}
		}
		#endregion Block Input
	}
}