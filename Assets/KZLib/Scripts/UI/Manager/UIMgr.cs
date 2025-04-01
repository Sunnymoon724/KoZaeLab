using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		//? Canvas List
		private readonly List<RepositoryUI> m_repositoryList = new();

		[SerializeField] private RepositoryUI2D m_repository2D = null;
		[SerializeField] private RepositoryUI3D m_repository3D = null;


		[ShowInInspector,ListDrawerSettings(ShowFoldout = false),ReadOnly]
		private readonly HashSet<UITag> m_dontReleaseHashSet = new();

		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<UITag,WindowUI> m_registerDict = new();

		private string m_uiPrefabPath = null;

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

			m_uiPrefabPath = ConfigMgr.In.Access<ConfigData.GameConfig>().UIPrefabPath;
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
		public WindowUI Register(UITag uiTag,bool isActive = true)
		{
			if(!m_registerDict.ContainsKey(uiTag))
			{
				m_registerDict.Add(uiTag,MakeUI(uiTag));

				if(isActive)
				{
					var data = m_registerDict[uiTag] as MonoBehaviour;

					data.gameObject.EnsureActive(false);
				}
			}

			return m_registerDict[uiTag];
		}

		private WindowUI MakeUI(UITag uiTag)
		{
			var prefab = ResMgr.In.GetObject(GetUIPath(uiTag));

			if(!prefab)
			{
				LogTag.UI.E($"{uiTag} is not exist.");

				return null;
			}

			if(!prefab.TryGetComponent<WindowUI>(out var window))
			{
				prefab.DestroyObject();

				LogTag.UI.E($"Component is not exist in {uiTag}.");

				return null;
			}

			if(window.Tag != uiTag)
			{
				prefab.DestroyObject();

				LogTag.UI.E($"UI tag is not matched. [{window.Tag} != {uiTag}]");

				return null;
			}

			transform.SetUIChild(prefab.transform);

			if(IsLibraryUI(uiTag))
			{
				RegisterDontRelease(uiTag);
			}

			return window;
		}

		public void RegisterDontRelease(UITag uiTag)
		{
			m_dontReleaseHashSet.AddNotOverlap(uiTag);
		}
		#endregion Register

		#region Open
		public void DelayOpen<TBase>(UITag uiTag,object param,float delayTime) where TBase : class,IWindowUI
		{
			CommonUtility.DelayAction(() => { Open<TBase>(uiTag,param); },delayTime);
		}

		public TBase Open<TBase>(UITag uiTag,object param = null) where TBase : class,IWindowUI
		{
			var window = FindOpened(uiTag);

			if(window == null)
			{
				window = Register(uiTag,false);

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

		public bool IsOpened(UITag uiTag)
		{
			return FindOpened(uiTag) != null;
		}
		#endregion Open

		#region Close
		public void Close(UITag uiTag,bool isRelease = false)
		{
			var data = FindOpened(uiTag);

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
				m_registerDict.Remove(windowUI.Tag);
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
			foreach(var window in new List<WindowUI>(m_registerDict.Values))
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
		public void Show(UITag uiTag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Show(uiTag);
			}
		}

		public void ShowAllGroup(IEnumerable<UITag> includeTagGroup = null,IEnumerable<UITag> excludeTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].ShowAllGroup(includeTagGroup,excludeTagGroup);
			}
		}
		#endregion Show

		#region Hide
		public void Hide(UITag uiTag)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].Hide(uiTag);
			}
		}

		public void HideAllGroup(IEnumerable<UITag> includeTagGroup = null,IEnumerable<UITag> excludeTagGroup = null)
		{
			for(var i=0;i<m_repositoryList.Count;i++)
			{
				m_repositoryList[i].HideAllGroup(includeTagGroup,excludeTagGroup);
			}
		}

		public bool? IsHidden(UITag uiTag)
		{
			var window = FindOpened(uiTag);

			return window == null ? null : window.IsHidden;
		}
		#endregion Hide

		#region Find
		public TTag Find<TTag>(UITag uiTag) where TTag : class,IWindowUI
		{
			var window = FindOpened(uiTag);

			return (window != null) ? window as TTag : null;
		}

		private WindowUI FindOpened(UITag uiTag)
		{
			foreach(var repository in m_repositoryList)
			{
				var window = repository.FindOpenedUI(uiTag);

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