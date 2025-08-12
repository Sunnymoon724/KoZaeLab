using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		//? Canvas List
		private readonly List<RepositoryUI> m_repositoryList = new();

		[SerializeField] private RepositoryUI2D m_repository2D = null;
		[SerializeField] private RepositoryUI3D m_repository3D = null;


		[ShowInInspector,ListDrawerSettings(ShowFoldout = false),ReadOnly]
		private readonly HashSet<string> m_dontReleaseHashSet = new();

		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<string,WindowUI> m_registerWindowDict = new();

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

			m_prefabPath = ConfigMgr.In.Access<GameConfig>().UIPrefabPath;
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

		#region Register
		/// <summary>
		/// Register -> Not open
		/// </summary>
		public WindowUI Register(string tag,bool isActive = true)
		{
			if(!m_registerWindowDict.TryGetValue(tag,out var window))
			{
				window = _MakeUI(tag);

				m_registerWindowDict.Add(tag,window);

				if(isActive)
				{
					var behaviour = window as MonoBehaviour;

					behaviour.gameObject.EnsureActive(false);
				}
			}

			return window;
		}

		private WindowUI _MakeUI(string tag)
		{
			var prefab = ResMgr.In.GetObject(_GetUIPath(tag));

			if(!prefab)
			{
				LogSvc.UI.E($"{tag} is not exist.");

				return null;
			}

			if(!prefab.TryGetComponent<WindowUI>(out var window))
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"Component is not exist in {tag}.");

				return null;
			}

			if(window.Tag != tag)
			{
				prefab.DestroyObject();

				LogSvc.UI.E($"UI tag is not matched. [{window.Tag} != {tag}]");

				return null;
			}

			transform.SetUIChild(prefab.transform);

			if(_IsDefinedUI(tag))
			{
				RegisterDontRelease(tag);
			}

			return window;
		}

		public void RegisterDontRelease(string tag)
		{
			m_dontReleaseHashSet.AddNotOverlap(tag);
		}
		#endregion Register

		#region Open
		public void DelayOpen<TBase>(string tag,object param,float delayTime) where TBase : class,IWindowUI
		{
			CommonUtility.DelayAction(() => { Open<TBase>(tag,param); },delayTime);
		}

		public TBase Open<TBase>(string tag,object param = null) where TBase : class,IWindowUI
		{
			var window = _FindOpened(tag);

			if(window == null)
			{
				window = Register(tag,false);

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

			return window as TBase;
		}

		public bool IsOpened(string tag)
		{
			return _FindOpened(tag) != null;
		}
		#endregion Open

		#region Close
		public void Close(string tag,bool isRelease = false)
		{
			var data = _FindOpened(tag);

			if(data != null)
			{
				Close(data,isRelease);
			}
		}

		public void Close(WindowUI windowUI,bool isRelease = false)
		{
			var release = (isRelease || !windowUI.IsPooling) && !m_dontReleaseHashSet.Contains(windowUI.Tag);

			if(release)
			{
				m_registerWindowDict.Remove(windowUI.Tag);
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
		public void Show(string tag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Show(tag);
			}
		}

		public void ShowAllGroup(IEnumerable<string> includeTagGroup = null,IEnumerable<string> excludeTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].ShowAllGroup(includeTagGroup,excludeTagGroup);
			}
		}
		#endregion Show

		#region Hide
		public void Hide(string tag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Hide(tag);
			}
		}

		public void HideAllGroup(IEnumerable<string> includeTagGroup = null,IEnumerable<string> excludeTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].HideAllGroup(includeTagGroup,excludeTagGroup);
			}
		}

		public bool? IsHidden(string tag)
		{
			var window = _FindOpened(tag);

			return window == null ? null : window.IsHidden;
		}
		#endregion Hide

		#region Find
		public TTag Find<TTag>(string tag) where TTag : class,IWindowUI
		{
			var window = _FindOpened(tag);

			return (window != null) ? window as TTag : null;
		}

		private WindowUI _FindOpened(string tag)
		{
			foreach(var repository in m_repositoryList)
			{
				var window = repository.FindOpenedUI(tag);

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