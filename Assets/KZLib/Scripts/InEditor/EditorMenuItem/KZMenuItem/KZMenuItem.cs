#if UNITY_EDITOR

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private const int c_pivotOrder = 10;
		private const int c_menuLine = 20;

		private enum MenuType
		{
			Option				= 0 << c_pivotOrder,

			Option_Delete		= Option+0*c_menuLine,
			Option_Find			= Option+1*c_menuLine,
			Option_Check		= Option+2*c_menuLine,
			Option_Etc			= Option+3*c_menuLine,


			Explorer			= 1 << c_pivotOrder,

			Explorer_Open		= Explorer+0*c_menuLine,


			Data				= 2 << c_pivotOrder,

			Data_GenerateAll	= Data+0*c_menuLine,
			Data_Generate		= Data+1*c_menuLine,
			Data_Open			= Data+2*c_menuLine,
			Data_Window			= Data+3*c_menuLine,


			Window				= 3 << c_pivotOrder,

			Window_Manual		= Window+0*c_menuLine,
			Window_Etc			= Window+1*c_menuLine,


			Scene				= 4 << c_pivotOrder,

			Scene_Quick			= Scene+0*c_menuLine,
			Scene_Core			= Scene+1*c_menuLine,
		}
	}
}
#endif