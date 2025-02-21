using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		private static readonly UITag[] s_library_ui_array = new UITag[] { UITag.TransitionPanelUI, UITag.HudPanelUI, UITag.VideoPanelUI };

		private string GetUIPath(UITag uiTag)
		{
			return $"{(IsLibraryUI(uiTag) ? "Resources/Common" : m_uiPrefabPath)}/{uiTag}.prefab";
		}

		private bool IsLibraryUI(UITag uiTag)
		{
			return s_library_ui_array.Contains(uiTag);
		}
	}
}