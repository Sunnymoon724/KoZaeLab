#if UNITY_EDITOR
using KZLib.KZDevelop;
using KZLib.KZWindow;
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		#region Window
		[MenuItem("KZMenu/Window/Open Manual Window",false,(int) MenuType.Window_Manual)]
		private static void _OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Editor Custom Window",false,(int) MenuType.Window_Etc)]
		private static void _OnOpenEditorCustomWindow()
		{
			EditorWindow.GetWindow<EditorCustomWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open PlayerPrefs Window",false,(int) MenuType.Window_Etc)]
		private static void _OnOpenPlayerPrefsWindow()
		{
			EditorWindow.GetWindow<PlayerPrefsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Graphic Quality Option Window",false,(int) MenuType.Window_Etc)]
		private static void _OnOpenGraphicQualityOptionWindow()
		{
			var instance = GraphicQualityOption.In;

			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Graphic Quality Option");

			viewer.SetResource(instance);
			viewer.Show();
		}

		[MenuItem("KZMenu/Window/Open Network Test Window",false,(int) MenuType.Window_Etc)]
		private static void _OnOpenNetworkTestWindow()
		{
			EditorWindow.GetWindow<NetworkTestWindow>().Show();
		}
		#endregion Window
	}
}
#endif