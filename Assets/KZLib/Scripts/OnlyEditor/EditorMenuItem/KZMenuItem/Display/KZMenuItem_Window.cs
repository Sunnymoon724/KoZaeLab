#if UNITY_EDITOR
using KZLib.Data;
using KZLib.Windows;
using UnityEditor;
using UnityEngine;

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

		[MenuItem("KZMenu/Window/Open Editor Custom Window",false,MenuOrder.Display.CUSTOM_EDITOR)]
		private static void _OnOpenEditorCustomWindow()
		{
			EditorCustom.ShowCustom();
		}

		[MenuItem("KZMenu/Window/Open PlayerPrefs Window",false,MenuOrder.Display.CUSTOM_PLAYER_PREFS)]
		private static void _OnOpenPlayerPrefsWindow()
		{
			EditorWindow.GetWindow<PlayerPrefsWindow>().Show();
		}

		[MenuItem("KZMenu/Window/Open Graphic Quality Option Window",false,MenuOrder.Display.CUSTOM_GRAPHIC_QUALITY)]
		private static void _OnOpenGraphicQualityOptionWindow()
		{
			var resourcesPath = "ScriptableObject/GraphicQualityOption";
			var graphicQuality = Resources.Load<GraphicQualityOption>(resourcesPath);

			if(graphicQuality == null)
			{
				graphicQuality = ScriptableObject.CreateInstance<GraphicQualityOption>();

				KZAssetKit.CreateAsset($"Resources/{resourcesPath}.asset",graphicQuality,true);
			}

			var window = EditorWindow.GetWindow<ScriptableObjectWindow>("Graphic Quality Option");

			window.SetResource(graphicQuality);
			window.Show();
		}

		[MenuItem("KZMenu/Window/Open Network Test Window",false,MenuOrder.Display.TEST_NETWORK)]
		private static void _OnOpenNetworkTestWindow()
		{
			EditorWindow.GetWindow<WebhookTestWindow>().Show();
		}
		#endregion Window
	}
}
#endif