#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMainScene()
		{
			OnOpenScene("Main","MainScene");
		}

		[MenuItem("KZMenu/Scene/Open Motion Editor Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMotionEditorScene()
		{
			OnOpenScene("MotionEditor","MotionEditorScene");
		}

		private static string GetScenePath(string sceneName)
		{
			var pathGroup = CommonUtility.FindAssetPathGroup($"t:Scene {sceneName}");

			return pathGroup.IsNullOrEmpty() ? string.Empty : pathGroup.First();
		}

		private static void OnOpenScene(string title,string sceneName)
		{
			if(CommonUtility.DisplayCheck($"Open {title} scene",$"Open {title} scene?"))
			{
				var scenePath = GetScenePath(sceneName);

				if(scenePath.IsEmpty())
				{
					return;
				}

				EditorSceneManager.OpenScene(scenePath);
			}
		}
	}
}
#endif