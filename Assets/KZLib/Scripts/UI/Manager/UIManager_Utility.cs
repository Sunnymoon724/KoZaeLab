using System;
using System.Linq;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		private static readonly CommonUINameTag[] s_definedTagArray = new CommonUINameTag[] { CommonUINameTag.CommonTransitionPanel, CommonUINameTag.DebugOverlayPanel, CommonUINameTag.VideoPanel  };

		private string _GetPrefabPath(CommonUINameTag nameTag)
		{
			return $"{(_IsDefinedWindow(nameTag) ? "Resources/Prefab/UI" : m_prefabPath)}/{nameTag}.prefab";
		}

		private bool _IsDefinedWindow(CommonUINameTag nameTag)
		{
			return s_definedTagArray.Contains(nameTag);
		}
	}
}