#if UNITY_EDITOR
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Window/Open Settings Window",false,KZCategory.Window_Settings)]
		private static void OnOpenSettingsWindow()
		{
			EditorWindow.GetWindow<SettingsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Save Data Window",false,KZCategory.Window_Data)]
		private static void OnOpenSaveDataWindow()
		{
			EditorWindow.GetWindow<SaveDataWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Meta Data Window",false,KZCategory.Window_Data)]
		private static void OnOpenMetaDataWindow()
		{
			EditorWindow.GetWindow<MetaDataWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Manual Window",false,KZCategory.Window_Manual)]
		private static void OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}
	}
}
#endif