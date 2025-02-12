#if UNITY_EDITOR
using KZLib.KZDevelop;
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private const int c_pivot_order = 5;

		private enum MenuType
		{
			Option				= 0 << c_pivot_order,

			Option_Delete		= Option+0*Global.MENU_LINE,
			Option_Find			= Option+1*Global.MENU_LINE,
			Option_Add			= Option+2*Global.MENU_LINE,
			Option_Check		= Option+3*Global.MENU_LINE,

			Config				= 2 << c_pivot_order,

			Config_All			= Config+0*Global.MENU_LINE,
			Config_Generate		= Config+1*Global.MENU_LINE,
			Config_Open			= Config+2*Global.MENU_LINE,

			Window				= 3 << c_pivot_order,

			Window_Manual		= Window+0*Global.MENU_LINE,
			Window_Proto		= Window+1*Global.MENU_LINE,
			Window_Etc			= Window+2*Global.MENU_LINE,

			Scene				= 4 << c_pivot_order,

			Scene_Quick			= Scene+0*Global.MENU_LINE,
			Scene_Core			= Scene+1*Global.MENU_LINE,
		}
	}
}
#endif