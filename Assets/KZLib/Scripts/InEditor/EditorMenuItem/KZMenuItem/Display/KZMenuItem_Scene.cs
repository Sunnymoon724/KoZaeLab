#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.KZMenu
{
	public static partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,MenuOrder.Display.MANUAL)]
		private static void _OnOpenMainScene()
		{
			CommonUtility.OpenSceneInEditor("MainScene");
		}
	}
}
#endif