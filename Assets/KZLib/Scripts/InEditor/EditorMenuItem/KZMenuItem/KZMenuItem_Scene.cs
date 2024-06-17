#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Scene/Open Main Scene",false,KZCategory.Scene_Core)]
		private static void OnOpenMainScene()
		{
			OnOpenScene("메인","MainScene");
		}

		[MenuItem("KZMenu/Scene/Open Schedule Test Scene",false,KZCategory.Scene_Test)]
		private static void OnOpenScheduleTestSample()
		{
			OnOpenScene("스케쥴 테스트","MainScene");
		}

		private static string GetScenePath(string _sceneName)
		{
			var pathGroup = CommonUtility.GetAssetPathGroup(string.Format("t:Scene {0}",_sceneName));

			return pathGroup.IsNullOrEmpty() ? string.Empty : pathGroup.First();
		}

		private static void OnOpenScene(string _title,string _sceneName)
		{
			if(CommonUtility.DisplayCheck(string.Format("{0} 씬 열기",_title),string.Format("{0} 씬을 여시겼습니까?",_title)))
			{
				var scenePath = GetScenePath(_sceneName);

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
