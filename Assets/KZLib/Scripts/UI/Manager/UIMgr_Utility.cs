using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		private static readonly string[] s_definedTagArray = new string[] { Global.TRANSITION_PANEL_UI, Global.HUD_PANEL_UI, Global.VIDEO_PANEL_UI  };

		private string _GetUIPath(string tag)
		{
			return $"{(_IsDefinedUI(tag) ? "Resources/Prefab/UI" : m_prefabPath)}/{tag}.prefab";
		}

		private bool _IsDefinedUI(string tag)
		{
			return s_definedTagArray.Contains(tag);
		}
	}
}