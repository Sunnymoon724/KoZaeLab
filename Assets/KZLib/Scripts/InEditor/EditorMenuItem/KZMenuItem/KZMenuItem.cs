#if UNITY_EDITOR

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private const int c_pivot_order = 10;
		private const int c_menu_line = 20;

		private enum MenuType
		{
			Option				= 0 << c_pivot_order,

			Option_Delete		= Option+0*c_menu_line,
			Option_Find			= Option+1*c_menu_line,
			Option_Check		= Option+2*c_menu_line,
			Option_Etc			= Option+3*c_menu_line,


			Explorer			= 1 << c_pivot_order,

			Explorer_Open		= Explorer+0*c_menu_line,


			Data				= 2 << c_pivot_order,

			Data_GenerateAll	= Data+0*c_menu_line,
			Data_Generate		= Data+1*c_menu_line,
			Data_Open			= Data+2*c_menu_line,
			Data_Window			= Data+3*c_menu_line,


			Window				= 3 << c_pivot_order,

			Window_Manual		= Window+0*c_menu_line,
			Window_Etc			= Window+1*c_menu_line,


			Scene				= 4 << c_pivot_order,

			Scene_Quick			= Scene+0*c_menu_line,
			Scene_Core			= Scene+1*c_menu_line,
		}
	}
}
#endif