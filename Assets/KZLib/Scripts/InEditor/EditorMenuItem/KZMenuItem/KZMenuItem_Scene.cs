#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,MenuOrder.Scene.CORE)]
		private static void _OnOpenMainScene()
		{
			CommonUtility.OpenSceneInEditor("MainScene");
		}

		[MenuItem("KZMenu/Scene/Open Motion Editor Scene",false,MenuOrder.Scene.CORE)]
		private static void _OnOpenMotionEditorScene()
		{
			CommonUtility.OpenSceneInEditor("MotionEditorScene");
		}
	}
}
#endif