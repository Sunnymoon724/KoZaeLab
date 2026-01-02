using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		private static readonly CommonUINameTag[] s_definedTagArray = new CommonUINameTag[] { CommonUINameTag.CommonTransitionPanelUI, CommonUINameTag.HudPanelUI, CommonUINameTag.VideoPanelUI  };

		private string _GetUIPath(CommonUINameTag nameTag)
		{
			return $"{(_IsDefinedUI(nameTag) ? "Resources/Prefab/UI" : m_prefabPath)}/{nameTag}.prefab";
		}

		private bool _IsDefinedUI(CommonUINameTag nameTag)
		{
			return s_definedTagArray.Contains(nameTag);
		}
	}
}