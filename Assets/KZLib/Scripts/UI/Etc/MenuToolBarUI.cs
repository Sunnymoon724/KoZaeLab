using System;
using System.Collections.Generic;
using KZLib.Data;
using KZLib.Utilities;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// Toolbar that maps registered <see cref="CommonUIMenuTag"/> entries to pooled <see cref="MenuSlot"/> cells via
	/// <see cref="ReuseGridLayoutGroup"/>.
	/// </summary>
	/// <remarks>
	/// Inspector holds per-menu icon/sound (or shared assets). <see cref="SetMenu"/> selects visible items and binds
	/// runtime click handlers. Passing no valid entries clears the grid. Labels use localization keys
	/// <c>UIMenu_{tag}</c> and refresh on <see cref="LingoManager.OnChangedLanguage"/>.
	/// </remarks>
	public class MenuToolBarUI : MonoBehaviour
	{
		private const string c_menuLingoPrefix = "UIMenu_";

		#region MenuInfo
		[Serializable]
		private record MenuInfo
		{
			[SerializeField,ValueDropdown(nameof(MenuTagGroup))]
			private string m_menuName = null;
			public string MenuName => m_menuName;

			public CommonUIMenuTag MenuType => CustomTag.Parse<CommonUIMenuTag>(m_menuName);

			private List<CommonUIMenuTag> m_menuTagList = null;
			private System.Collections.IEnumerable MenuTagGroup => m_menuTagList ??= CustomTag.CollectCustomTagList<CommonUIMenuTag>();

			[SerializeField,HideIf(nameof(m_UseCommonSprite))]
			private Sprite m_MenuSprite = null;
			public Sprite MenuSprite => m_MenuSprite;

			[SerializeField,HideIf(nameof(m_UseCommonAudio))]
			private AudioClip m_MenuAudioClip = null;
			public AudioClip MenuAudioClip => m_MenuAudioClip;

			[SerializeField,HideInInspector]
			private bool m_UseCommonSprite = false;
			[SerializeField,HideInInspector]
			private bool m_UseCommonAudio = false;

			public MenuInfo(bool useCommonSprite,bool useCommonAudio)
			{
				SetCommon(useCommonSprite,useCommonAudio);
			}

			public void SetCommon(bool useCommonSprite,bool useCommonAudio)
			{
				m_UseCommonSprite = useCommonSprite;
				m_UseCommonAudio = useCommonAudio;
			}
		}
		#endregion MenuInfo

		public record Param(CommonUIMenuTag MenuType,Action<IEntryInfo> OnClicked);

		private static readonly List<IEntryInfo> s_emptyEntryList = new();

		[HorizontalGroup("0",Order = 0),SerializeField]
		private ReuseGridLayoutGroup m_gridLayout = null;
		[HorizontalGroup("1",Order = 1),SerializeField,ToggleLeft,OnValueChanged(nameof(_OnChangeCommon))]
		private bool m_useCommonIcon = false;
		[HorizontalGroup("1",Order = 1),SerializeField,HideLabel,ShowIf(nameof(m_useCommonIcon))]
		private Sprite m_menuIcon = null;
		[HorizontalGroup("2",Order = 2),SerializeField,ToggleLeft,OnValueChanged(nameof(_OnChangeCommon))]
		private bool m_useCommonSound = false;
		[HorizontalGroup("2",Order = 2),SerializeField,HideLabel,ShowIf(nameof(m_useCommonSound))]
		private AudioClip m_menuSound = null;

		[HorizontalGroup("3",Order = 3),SerializeField,ListDrawerSettings(ShowFoldout = false,CustomAddFunction = nameof(_OnAddMenu),DraggableItems = false)]
		private List<MenuInfo> m_menuInfoList = new();

		private Param[] m_cachedMenuParamArray = Array.Empty<Param>();
		private IDisposable m_languageSubscription = null;
		private List<IEntryInfo> m_pendingEntryInfoList = null;
		private bool m_hasPendingEntryInfoList = false;

#if UNITY_EDITOR
		private void OnValidate()
		{
			_SyncMenuInfoCommonFlags();
			_ValidateDuplicateMenuTypes();
			_ValidateCommonAssets();
		}
#endif

		private void OnEnable()
		{
			m_languageSubscription = LingoManager.In.OnChangedLanguage.Subscribe(_OnChangedLanguage);

			_TryApplyPendingEntryInfoList();
		}

		private void OnDisable()
		{
			m_languageSubscription?.Dispose();
			m_languageSubscription = null;
		}

		private void Start()
		{
			_TryApplyPendingEntryInfoList();
		}

		private void _OnChangedLanguage(Unit _)
		{
			SetMenu(m_cachedMenuParamArray);
		}

		private void _OnChangeCommon()
		{
			_SyncMenuInfoCommonFlags();
		}

		private void _SyncMenuInfoCommonFlags()
		{
			for(var i=0;i<m_menuInfoList.Count;i++)
			{
				m_menuInfoList[i].SetCommon(m_useCommonIcon,m_useCommonSound);
			}
		}

#if UNITY_EDITOR
		private void _ValidateDuplicateMenuTypes()
		{
			if(m_menuInfoList.IsNullOrEmpty())
			{
				return;
			}

			var seen = new HashSet<CommonUIMenuTag>();

			for(var i=0;i<m_menuInfoList.Count;i++)
			{
				var menuType = m_menuInfoList[i].MenuType;

				if(menuType == null)
				{
					continue;
				}

				if(!seen.Add(menuType))
				{
					LogChannel.UI.W($"{nameof(MenuToolBarUI)}: duplicate {nameof(CommonUIMenuTag)} '{menuType}' at index {i}.");
				}
			}
		}

		private void _ValidateCommonAssets()
		{
			if(m_useCommonIcon && !m_menuIcon)
			{
				LogChannel.UI.W($"{nameof(MenuToolBarUI)}: {nameof(m_useCommonIcon)} is enabled but {nameof(m_menuIcon)} is not assigned.");
			}

			if(m_useCommonSound && !m_menuSound)
			{
				LogChannel.UI.W($"{nameof(MenuToolBarUI)}: {nameof(m_useCommonSound)} is enabled but {nameof(m_menuSound)} is not assigned.");
			}
		}
#endif

		private void _OnAddMenu()
		{
			m_menuInfoList.Add(new MenuInfo(m_useCommonIcon,m_useCommonSound));
		}

		/// <summary>Binds visible toolbar slots. Clears the grid when no valid entries remain.</summary>
		public void SetMenu(params Param[] menuParamArray)
		{
			m_cachedMenuParamArray = menuParamArray ?? Array.Empty<Param>();

			if(!m_gridLayout)
			{
				LogChannel.UI.W($"{nameof(MenuToolBarUI)}: {nameof(m_gridLayout)} is not assigned.");

				return;
			}

			if(menuParamArray.IsNullOrEmpty())
			{
				_ApplyToGrid(s_emptyEntryList);

				return;
			}

			_WarnMissingCommonAssets();

			var entryInfoList = new List<IEntryInfo>();
			var seenMenuTypes = new HashSet<CommonUIMenuTag>();

			for(var i=0;i<menuParamArray.Length;i++)
			{
				var menuParam = menuParamArray[i];

				if(menuParam == null)
				{
					LogChannel.UI.E($"{nameof(MenuToolBarUI)}: {nameof(Param)} at index {i} is null.");

					continue;
				}

				if(menuParam.MenuType == null)
				{
					LogChannel.UI.E($"{nameof(MenuToolBarUI)}: {nameof(Param.MenuType)} at index {i} is null.");

					continue;
				}

				if(!seenMenuTypes.Add(menuParam.MenuType))
				{
					LogChannel.UI.W($"{nameof(MenuToolBarUI)}: duplicate {nameof(Param.MenuType)} '{menuParam.MenuType}' at index {i}.");

					continue;
				}

				bool _FindMenuInfo(MenuInfo menuInfo)
				{
					return menuInfo.MenuType != null && menuInfo.MenuType.Equals(menuParam.MenuType);
				}

				var menuInfo = m_menuInfoList.Find(_FindMenuInfo);

				if(menuInfo == null)
				{
					LogChannel.UI.E($"{menuParam.MenuType} is not registered.");

					continue;
				}

				if(menuParam.OnClicked == null)
				{
					LogChannel.UI.E($"{menuParam.MenuType} does not have {nameof(Param.OnClicked)}.");

					continue;
				}

				var menuIcon = m_useCommonIcon ? m_menuIcon : menuInfo.MenuSprite;
				var menuSound = m_useCommonSound ? m_menuSound : menuInfo.MenuAudioClip;

				entryInfoList.Add(new EntryInfo(_GetMenuLingoKey(menuInfo.MenuName),string.Empty,menuIcon,menuSound,menuParam.OnClicked));
			}

			_ApplyToGrid(entryInfoList.IsNullOrEmpty() ? s_emptyEntryList : entryInfoList);
		}

		private void _ApplyToGrid(List<IEntryInfo> entryInfoList)
		{
			if(m_gridLayout.IsReady)
			{
				m_gridLayout.SetEntryInfoList(entryInfoList);
				m_hasPendingEntryInfoList = false;
				m_pendingEntryInfoList = null;
			}
			else
			{
				m_pendingEntryInfoList = entryInfoList;
				m_hasPendingEntryInfoList = true;
			}
		}

		private void _TryApplyPendingEntryInfoList()
		{
			if(!m_hasPendingEntryInfoList || !m_gridLayout || !m_gridLayout.IsReady)
			{
				return;
			}

			m_gridLayout.SetEntryInfoList(m_pendingEntryInfoList ?? s_emptyEntryList);
			m_hasPendingEntryInfoList = false;
			m_pendingEntryInfoList = null;
		}

		private void _WarnMissingCommonAssets()
		{
			if(m_useCommonIcon && !m_menuIcon)
			{
				LogChannel.UI.W($"{nameof(MenuToolBarUI)}: {nameof(m_useCommonIcon)} is enabled but {nameof(m_menuIcon)} is not assigned.");
			}

			if(m_useCommonSound && !m_menuSound)
			{
				LogChannel.UI.W($"{nameof(MenuToolBarUI)}: {nameof(m_useCommonSound)} is enabled but {nameof(m_menuSound)} is not assigned.");
			}
		}

		private static string _GetMenuLingoKey(string menuName)
		{
			return $"{c_menuLingoPrefix}{menuName}";
		}
	}
}
