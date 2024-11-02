using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public static readonly Vector3 HIDE_POS = new(0.0f,3000.0f,0.0f);
		public static readonly Vector3 DEFAULT_POS = Vector3.zero;

		//? Canvas List
		private readonly List<RepositoryUI> m_RepositoryList = new();

		[SerializeField] private RepositoryUI2D m_Repository2D = null;
		[SerializeField] private RepositoryUI3D m_Repository3D = null;


		[ShowInInspector,ListDrawerSettings(ShowFoldout = false),ReadOnly]
		private readonly HashSet<UITag> m_DontReleaseSet = new();

		[ShowInInspector,DictionaryDrawerSettings(IsReadOnly = true,DisplayMode = DictionaryDisplayOptions.Foldout),ReadOnly]
		private readonly Dictionary<UITag,WindowUI> m_RegisterDict = new();

		private string m_UIPrefabPath = null;

		protected override void Initialize()
		{
			if(m_Repository2D)
			{
				m_RepositoryList.Add(m_Repository2D);
			}

			if(m_Repository3D)
			{
				m_RepositoryList.Add(m_Repository3D);
			}

			ChangeScreenSize(new(Screen.width,Screen.height));

			m_UIPrefabPath = GameSettings.In.UIPrefabPath;
		}

		protected override void Release()
		{
			m_DontReleaseSet.Clear();
		}

		public void ChangeScreenSize(Vector2Int _size)
		{
			if(!m_Repository2D)
			{
				return;
			}

			var scaler = m_Repository2D.GetComponent<CanvasScaler>();

			scaler.referenceResolution = new Vector2(_size.x,_size.y);

			m_Repository2D.CanvasCamera.aspect = _size.x/_size.y;
			m_Repository2D.CanvasCamera.orthographicSize = _size.y/200.0f;
		}

		public void RegisterDontRelease(UITag _tag)
		{
			m_DontReleaseSet.AddNotOverlap(_tag);
		}

		#region Register
		/// <summary>
		/// Register -> Not open
		/// </summary>
		public WindowUI Register(UITag _tag,bool _disable = true)
		{
			if(!m_RegisterDict.ContainsKey(_tag))
			{
				m_RegisterDict.Add(_tag,MakeUI(_tag));

				if(_disable)
				{
					var data = m_RegisterDict[_tag] as MonoBehaviour;

					data.gameObject.SetActiveSelf(false);
				}
			}

			return m_RegisterDict[_tag];
		}
		#endregion Register

		#region Open
		public void DelayOpen<TBase>(UITag _tag,object _param,float _delayTime) where TBase : class,IWindowUI
		{
			R3Utility.DelayAction(() => { Open<TBase>(_tag,_param); },_delayTime);
		}

		public TBase Open<TBase>(UITag _tag,object _param = null) where TBase : class,IWindowUI
		{
			var window = GetOpened(_tag);

			if(window == null)
			{
				window = Register(_tag,false);

				if(window.Is3D)
				{
					m_Repository3D.Add(window);
				}
				else
				{
					m_Repository2D.Add(window);
				}

				window.Open(_param);
			}

			return window as TBase;
		}
		#endregion Open

		public bool IsOpened(UITag _tag)
		{
			return GetOpened(_tag) != null;
		}

		private WindowUI MakeUI(UITag _tag)
		{
			var result = ResMgr.In.GetObject(GetUIPath(_tag));

			if(!result)
			{
				throw new NullReferenceException($"{_tag} is not exist.");
			}

			if(!result.TryGetComponent<WindowUI>(out var window))
			{
				CommonUtility.DestroyObject(result);

				throw new NullReferenceException($"Component is not exist in {_tag}.");
			}

			if(window.Tag != _tag)
			{
				CommonUtility.DestroyObject(result);

				throw new NullReferenceException($"ui tag is not matched. [{window.Tag} != {_tag}]");
			}

			transform.SetUIChild(result.transform);

			if(IsLibraryUI(_tag))
			{
				RegisterDontRelease(_tag);
			}

			return window;
		}

		public TTag Get<TTag>(UITag _type) where TTag : class,IWindowUI
		{
			var window = GetOpened(_type);

			return (window != null) ? window as TTag : null;
		}

		private WindowUI GetOpened(UITag _tag)
		{
			foreach(var repository in m_RepositoryList)
			{
				var window = repository.GetOpenedUI(_tag);

				if(window != null)
				{
					return window;
				}
			}

			return null;
		}

		#region Close
		public void Close(UITag _tag,bool _release = false)
		{
			var data = GetOpened(_tag);

			if(data != null)
			{
				Close(data,_release);
			}
		}

		public void CloseAllOpened(bool _release,bool _excludeHide)
		{
			foreach(var window in new List<WindowUI>(m_RegisterDict.Values))
			{
				if(_excludeHide && window.IsHide)
				{
					continue;
				}

				Close(window,_release);
			}
		}

		private void Close(WindowUI _data,bool _release)
		{
			var release = (_release || !_data.IsPooling) && !m_DontReleaseSet.Contains(_data.Tag);

			if(release)
			{
				m_RegisterDict.Remove(_data.Tag);
			}

			_data.Close();

			if(_data.Is3D)
			{
				m_Repository3D.Remove(_data,release);
			}
			else
			{
				m_Repository2D.Remove(_data,release);
			}
		}
		#endregion Close

		#region Hide
		/// <summary>
		/// UI의 상태를 바꾼다.
		/// </summary>
		public UITag Hide(UITag _tag,bool _hide)
		{
			var window = GetOpened(_tag);

			if(window != null)
			{
				return null;
			}

			window.Hide(_hide);

			return window.Tag;
		}

		public bool? IsHide(UITag _tag)
		{
			var window = GetOpened(_tag);

			if(window == null)
			{
				return null;
			}

			return window.IsHide;
		}

		public IEnumerable<UITag> HideAll(bool _hide,IEnumerable<UITag> _includeGroup = null,IEnumerable<UITag> _excludeGroup = null)
		{
			var tagList = new List<UITag>();

			for(var i=0;i<m_RepositoryList.Count;i++)
			{
				tagList.AddRange(m_RepositoryList[i].SetHideAll(_hide,_includeGroup,_excludeGroup));
			}

			return tagList;
		}
		#endregion Hide

		public void BlockInput(bool _block)
		{
			foreach(var repository in m_RepositoryList)
			{
				repository.BlockInput(_block);
			}
		}
	}
}