#if UNITY_EDITOR

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private const int c_pivot_order = 5;
		private const int c_menu_line = 20;

		private enum MenuType
		{
			Option				= 0 << c_pivot_order,

			Option_Delete		= Option+0*c_menu_line,
			Option_Find			= Option+1*c_menu_line,
			Option_Add			= Option+2*c_menu_line,
			Option_Check		= Option+3*c_menu_line,


			Proto				= 1 << c_pivot_order,

			Proto_GenerateAll	= Proto+0*c_menu_line,
			Proto_Generate		= Proto+1*c_menu_line,
			Proto_Open			= Proto+2*c_menu_line,


			Config				= 2 << c_pivot_order,

			Config_GenerateAll	= Config+0*c_menu_line,
			Config_Generate		= Config+1*c_menu_line,
			Config_Open			= Config+2*c_menu_line,


			Window				= 3 << c_pivot_order,

			Window_Manual		= Window+0*c_menu_line,
			Window_Proto		= Window+1*c_menu_line,
			Window_Etc			= Window+2*c_menu_line,


			Scene				= 4 << c_pivot_order,

			Scene_Quick			= Scene+0*c_menu_line,
			Scene_Core			= Scene+1*c_menu_line,
		}
	}
}
#endif