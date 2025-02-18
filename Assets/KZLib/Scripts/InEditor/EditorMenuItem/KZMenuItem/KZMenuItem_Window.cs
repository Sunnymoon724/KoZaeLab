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
		private static void OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Editor Custom Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenEditorCustomWindow()
		{
			EditorWindow.GetWindow<EditorCustomWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Local Storage Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenLocalStorageWindow()
		{
			EditorWindow.GetWindow<LocalStorageWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Graphic Quality Option Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenGraphicQualityOptionWindow()
		{
			var instance = GraphicQualityOption.In;

			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Graphic Quality Option");

			viewer.SetResource(instance);
			viewer.Show();
		}

		[MenuItem("KZMenu/Window/Open Network Test Window",false,(int) MenuType.Window_Etc)]
		private static void OnOpenNetworkTestWindow()
		{
			EditorWindow.GetWindow<NetworkTestWindow>().Show();
		}
		#endregion Window
	}
}
#endif