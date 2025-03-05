#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMainScene()
		{
			OnOpenScene("MainScene");
		}

		[MenuItem("KZMenu/Scene/Open Motion Editor Scene",false,(int) MenuType.Scene_Core)]
		private static void OnOpenMotionEditorScene()
		{
			OnOpenScene("MotionEditorScene");
		}

		private static string GetScenePath(string sceneName)
		{
			var guidArray = AssetDatabase.FindAssets($"t:Scene {sceneName}");

			return guidArray.Length < 1 ? string.Empty : AssetDatabase.GUIDToAssetPath(guidArray[0]);
		}

		private static void OnOpenScene(string sceneName)
		{
			if(!EditorUtility.DisplayDialog($"Open {sceneName}",$"Do you want to open the {sceneName}?","Yes","No"))
			{
				return;
			}

			var scenePath = GetScenePath(sceneName);

			if(scenePath.IsEmpty())
			{
				return;
			}

			EditorSceneManager.OpenScene(scenePath);
		}
	}
}
#endif