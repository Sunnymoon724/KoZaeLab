#if UNITY_EDITOR
using KZLib.Development;
using KZLib.EditorInternal;
using KZLib.Windows;
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		#region Window
		[MenuItem("KZMenu/Window/Open Manual Window",false,MenuOrder.Display.MANUAL)]
		private static void _OnOpenManualWindow()
		{
			EditorWindow.GetWindow<ManualWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Editor Custom Window",false,MenuOrder.Display.CUSTOM)]
		private static void _OnOpenEditorCustomWindow()
		{
			EditorCustom.ShowCustom();
		}

		[MenuItem("KZMenu/Window/Open PlayerPrefs Window",false,MenuOrder.Display.CUSTOM)]
		private static void _OnOpenPlayerPrefsWindow()
		{
			EditorWindow.GetWindow<PlayerPrefsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Graphic Quality Option Window",false,MenuOrder.Display.CUSTOM)]
		private static void _OnOpenGraphicQualityOptionWindow()
		{
			var instance = GraphicQualityOption.In;

			var window = EditorWindow.GetWindow<ScriptableObjectWindow>("Graphic Quality Option");

			window.SetResource(instance);
			window.Show();
		}

		[MenuItem("KZMenu/Window/Open Network Test Window",false,MenuOrder.Display.TEST)]
		private static void _OnOpenNetworkTestWindow()
		{
			EditorWindow.GetWindow<NetworkTestWindow>().Show();
		}
		#endregion Window
	}
}
#endif