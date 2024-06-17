using System;
using System.Collections.Generic;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class UIMenuType : Enumeration
{
	public UIMenuType(string _name) : base(_name) { }
}

namespace KZLib
{
	public class MenuToolBarUI : BaseComponentUI
	{
		[Serializable]
		private class MenuData
		{
			[SerializeField,LabelText("메뉴 타입"),ValueDropdown(nameof(MenuList))]
			private string m_MenuName = null;
			public string MenuName => m_MenuName;

			public UIMenuType MenuType => Enumeration.Parse<UIMenuType>(m_MenuName);

			private static List<string> s_MenuList = null;
			private static List<string> MenuList => s_MenuList ??= Enumeration.GetNames<UIMenuType>();

			[SerializeField,LabelText("이미지"),HideIf(nameof(m_UseCommonSprite))]
			private Sprite m_MenuSprite = null;
			public Sprite MenuSprite => m_MenuSprite;

			[SerializeField,LabelText("사운드"),HideIf(nameof(m_UseCommonAudio))]
			private AudioClip m_MenuAudioClip = null;
			public AudioClip MenuAudioClip => m_MenuAudioClip;

			[SerializeField,HideInInspector]
			private bool m_UseCommonSprite = false;
			[SerializeField,HideInInspector]
			private bool m_UseCommonAudio = false;

			public MenuData(bool _useCommonSprite,bool _useCommonAudio)
			{
				ChangeCommon(_useCommonSprite,_useCommonAudio);
			}

			public void ChangeCommon(bool _useCommonSprite,bool _useCommonAudio)
			{
				m_UseCommonSprite = _useCommonSprite;
				m_UseCommonAudio = _useCommonAudio;
			}
		}

		public record MenuParam(UIMenuType MenuType,Action OnClicked);

		[HorizontalGroup("0",Order = 0),SerializeField,LabelText("레이아웃")]
		private GridLayoutUI m_GridLayout = null;
		[HorizontalGroup("1",Order = 1),SerializeField,ToggleLeft,LabelText("동일 이미지 사용"),OnValueChanged(nameof(OnChangeCommon))]
		private bool m_UseCommonSprite = false;
		[HorizontalGroup("1",Order = 1),SerializeField,HideLabel,ShowIf(nameof(m_UseCommonSprite))]
		private Sprite m_MenuSprite = null;
		[HorizontalGroup("2",Order = 2),SerializeField,ToggleLeft,LabelText("동일 사운드 사용"),OnValueChanged(nameof(OnChangeCommon))]
		private bool m_UseCommonAudio = false;
		[HorizontalGroup("2",Order = 2),SerializeField,HideLabel,ShowIf(nameof(m_UseCommonAudio))]
		private AudioClip m_MenuAudioClip;

		[HorizontalGroup("3",Order = 3),SerializeField,LabelText("메뉴 리스트"),ListDrawerSettings(ShowFoldout = false,CustomAddFunction = nameof(OnAddMenu),DraggableItems = false)]
		private List<MenuData> m_MenuDataList = new();

		private void OnChangeCommon()
		{
			foreach(var menuData in m_MenuDataList)
			{
				menuData.ChangeCommon(m_UseCommonSprite,m_UseCommonAudio);
			}
		}

		private void OnAddMenu()
		{
			m_MenuDataList.Add(new MenuData(m_UseCommonSprite,m_UseCommonAudio));
		}

		public void SetMenu(params MenuParam[] _dataArray)
		{
			var cellList = new List<ICellData>();
			
			foreach(var data in _dataArray)
			{
				var cell = m_MenuDataList.Find(x=>x.MenuType.Equals(data.MenuType));

				if(cell == null)
				{
					Log.UI.E("{0}은 정의되어 있지 않습니다.",data.MenuType);

					continue;
				}

				if(data.OnClicked == null)
				{
					throw new NullReferenceException(string.Format("{0}의 실행 함수가 없습니다.",data.MenuType));
				}

				cellList.Add(new CellData(cell.MenuName,null,m_UseCommonSprite ? m_MenuSprite : cell.MenuSprite,m_UseCommonAudio ? m_MenuAudioClip : cell.MenuAudioClip,data.OnClicked));
			}

			if(cellList.IsNullOrEmpty())
			{
				return;
			}

			m_GridLayout.SetCellList(cellList);
		}
	}
}