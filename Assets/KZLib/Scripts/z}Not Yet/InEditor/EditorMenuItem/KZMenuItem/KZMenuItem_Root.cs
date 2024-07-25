#if UNITY_EDITOR

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		private const int PIVOT = 5;

		private partial struct KZCategory
		{
			//? Option
			private const int Option			= 0 << PIVOT;
			public const int Option_Delete		= Option+0*Global.MENU_LINE;
			public const int Option_Find		= Option+1*Global.MENU_LINE;
			public const int Option_Add			= Option+2*Global.MENU_LINE;
			public const int Option_Check		= Option+3*Global.MENU_LINE;

			//? Build
			private const int Build				= 1 << PIVOT;
			public const int Build_Quick		= Build+0*Global.MENU_LINE;

			//? Window
			private const int Window			= 2 << PIVOT;
			public const int Window_Settings	= Window+0*Global.MENU_LINE;
			public const int Window_Data		= Window+1*Global.MENU_LINE;
			public const int Window_Manual		= Window+2*Global.MENU_LINE;

			//? Scene
			private const int Scene				= 3 << PIVOT;
			public const int Scene_Core			= Scene+0*Global.MENU_LINE;
			public const int Scene_Test			= Scene+1*Global.MENU_LINE;

			//? Custom
			private const int Custom			= 4 << PIVOT;
		}
	}
}
#endif
