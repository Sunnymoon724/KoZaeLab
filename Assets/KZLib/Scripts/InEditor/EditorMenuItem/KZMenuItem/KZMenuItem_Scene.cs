#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMainScene()
		{
			CommonUtility.OpenSceneInEditor("MainScene");
		}

		[MenuItem("KZMenu/Scene/Open Motion Editor Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMotionEditorScene()
		{
			CommonUtility.OpenSceneInEditor("MotionEditorScene");
		}
	}
}
#endif