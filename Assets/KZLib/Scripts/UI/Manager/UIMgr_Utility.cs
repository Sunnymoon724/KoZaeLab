using System;
using System.Linq;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		private static readonly UITag[] LIBRARY_UI_ARRAY = new UITag[] { UITag.TransitionPanelUI, UITag.HudPanelUI };

		private string GetUIPath(UITag _tag)
		{
			return string.Format("{0}/{1}.prefab",IsLibraryUI(_tag) ? "Resources/Common" : m_UIPrefabPath,_tag);
		}

		private bool IsLibraryUI(UITag _tag)
		{
			return LIBRARY_UI_ARRAY.Contains(_tag);
		}
	}
}