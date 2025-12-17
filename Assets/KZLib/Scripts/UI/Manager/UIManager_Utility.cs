using System;
using System.Linq;
using KZLib.KZData;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		private static readonly UINameType[] s_definedTagArray = new UINameType[] { UINameType.CommonTransitionPanelUI, UINameType.HudPanelUI, UINameType.VideoPanelUI  };

		private string _GetUIPath(UINameType nameType)
		{
			return $"{(_IsDefinedUI(nameType) ? "Resources/Prefab/UI" : m_prefabPath)}/{nameType}.prefab";
		}

		private bool _IsDefinedUI(UINameType nameType)
		{
			return s_definedTagArray.Contains(nameType);
		}
	}
}