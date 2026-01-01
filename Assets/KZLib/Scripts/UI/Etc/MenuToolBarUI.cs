using System;
using System.Collections;
using System.Collections.Generic;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;

public class MenuToolBarUI : BaseComponentUI
{
	#region MenuInfo
	[Serializable]
	private record MenuInfo
	{
		[SerializeField,ValueDropdown(nameof(MenuTagGroup))]
		private string m_menuName = null;
		public string MenuName => m_menuName;

		public CommonUIMenuTag MenuType => CustomTag.Parse<CommonUIMenuTag>(m_menuName);

		private List<CommonUIMenuTag> m_menuTagList = null;
		private IEnumerable MenuTagGroup => m_menuTagList ??= CustomTag.CollectCustomTagList<CommonUIMenuTag>(true);

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

	[HorizontalGroup("0",Order = 0),SerializeField]
	private ReuseGridLayoutGroupUI m_gridLayout = null;
	[HorizontalGroup("1",Order = 1),SerializeField,ToggleLeft,OnValueChanged(nameof(_OnChangeCommon))]
	private bool m_useCommonIcon = false;
	[HorizontalGroup("1",Order = 1),SerializeField,HideLabel,ShowIf(nameof(m_useCommonIcon))]
	private Sprite m_menuIcon = null;
	[HorizontalGroup("2",Order = 2),SerializeField,ToggleLeft,OnValueChanged(nameof(_OnChangeCommon))]
	private bool m_useCommonSound = false;
	[HorizontalGroup("2",Order = 2),SerializeField,HideLabel,ShowIf(nameof(m_useCommonSound))]
	private AudioClip m_menuSound;

	[HorizontalGroup("3",Order = 3),SerializeField,ListDrawerSettings(ShowFoldout = false,CustomAddFunction = nameof(_OnAddMenu),DraggableItems = false)]
	private List<MenuInfo> m_menuInfoList = new();

	private void _OnChangeCommon()
	{
		for(var i=0;i<m_menuInfoList.Count;i++)
		{
			m_menuInfoList[i].SetCommon(m_useCommonIcon,m_useCommonSound);
		}
	}

	private void _OnAddMenu()
	{
		m_menuInfoList.Add(new MenuInfo(m_useCommonIcon,m_useCommonSound));
	}

	public void SetMenu(params Param[] menuParamArray)
	{
		var entryInfoList = new List<IEntryInfo>();

		for(var i=0;i<menuParamArray.Length;i++)
		{
			var menuParam = menuParamArray[i];

			bool _FindMenuInfo(MenuInfo menuInfo)
			{
				return menuInfo.MenuType.Equals(menuParam.MenuType);
			}

			var menuInfo = m_menuInfoList.Find(_FindMenuInfo);

			if(menuInfo == null)
			{
				LogSvc.UI.E($"{menuParam.MenuType} is not registered.");

				continue;
			}

			if(menuParam.OnClicked == null)
			{
				throw new ArgumentException($"{menuParam.MenuType} don't have OnClicked.");
			}
			
			var menuIcon = m_useCommonIcon ? m_menuIcon : menuInfo.MenuSprite;
			var menuSound = m_useCommonSound ? m_menuSound : menuInfo.MenuAudioClip;

			entryInfoList.Add(new EntryInfo(menuInfo.MenuName,string.Empty,menuIcon,menuSound,menuParam.OnClicked));
		}

		if(entryInfoList.IsNullOrEmpty())
		{
			return;
		}

		m_gridLayout.SetEntryInfoList(entryInfoList);
	}
}