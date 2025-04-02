using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		private static readonly UITag[] s_library_ui_array = new UITag[] { UITag.TransitionPanelUI, UITag.HudPanelUI, UITag.VideoPanelUI };

		private string _GetUIPath(UITag uiTag)
		{
			return $"{(_IsLibraryUI(uiTag) ? "Resources/Common" : m_uiPrefabPath)}/{uiTag}.prefab";
		}

		private bool _IsLibraryUI(UITag uiTag)
		{
			return s_library_ui_array.Contains(uiTag);
		}
	}
}