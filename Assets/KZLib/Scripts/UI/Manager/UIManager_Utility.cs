using System;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		private static readonly CommonUINameTag[] s_definedTagArray = new CommonUINameTag[] { CommonUINameTag.CommonTransitionPanel, CommonUINameTag.DebugOverlayPanel, CommonUINameTag.VideoPanel  };

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