#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,MenuOrder.Display.MANUAL)]
		private static void _OnOpenMainScene()
		{
			KZEditorKit.OpenSceneInEditor("MainScene");
		}
	}
}
#endif