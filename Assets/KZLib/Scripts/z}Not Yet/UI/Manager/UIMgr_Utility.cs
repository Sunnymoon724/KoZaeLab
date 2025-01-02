using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		private static readonly UITag[] LIBRARY_UI_ARRAY = new UITag[] { UITag.TransitionPanelUI, UITag.HudPanelUI, UITag.VideoPanelUI };

		private string GetUIPath(UITag _tag)
		{
			return $"{(IsLibraryUI(_tag) ? "Resources/Common" : m_UIPrefabPath)}/{_tag}.prefab";
		}

		private bool IsLibraryUI(UITag _tag)
		{
			return LIBRARY_UI_ARRAY.Contains(_tag);
		}
	}
}