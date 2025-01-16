#if UNITY_EDITOR
using KZLib.KZWindow;
using KZLib.Window;
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

			Build				= 1 << c_pivot_order,

			Build_Quick			= Build+0*Global.MENU_LINE,

			Config				= 2 << c_pivot_order,

			Config_Generate		= Config+0*Global.MENU_LINE,
			Config_Open			= Config+1*Global.MENU_LINE,

			Window				= 3 << c_pivot_order,

			Window_Settings		= Window+0*Global.MENU_LINE,
			Window_Manual		= Window+1*Global.MENU_LINE,
			Window_Etc			= Window+2*Global.MENU_LINE,

			Scene				= 4 << c_pivot_order,

			Scene_Quick			= Scene+0*Global.MENU_LINE,
			Scene_Core			= Scene+1*Global.MENU_LINE,
		}

		#region Window
		[MenuItem("KZMenu/Window/Open Settings Window",false,(int) MenuType.Window_Settings)]
		private static void OnOpenSettingsWindow()
		{
			EditorWindow.GetWindow<SettingsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Manual Window",false,(int) MenuType.Window_Manual)]
		private static void OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Local Storage Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenLocalStorageWindow()
		{
			EditorWindow.GetWindow<LocalStorageWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Proto Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenProtoWindow()
		{
			EditorWindow.GetWindow<ProtoWindow>().Show();
		}
		#endregion Window
	}
}
#endif