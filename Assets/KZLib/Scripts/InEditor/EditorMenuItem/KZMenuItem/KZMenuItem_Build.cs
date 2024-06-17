#if UNITY_EDITOR
using UnityEditor;

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		[MenuItem("KZMenu/Build/Quick App Build",false,KZCategory.Build_Quick)]
		private static void OnQuickAppBuild()
		{
			BuildSettings.In.OnBuildApp();
		}

		[MenuItem("KZMenu/Build/Quick Bundle Build",false,KZCategory.Build_Quick)]
		private static void OnQuickBundleBuild()
		{
			BuildSettings.In.OnBuildBundle();
		}

		[MenuItem("KZMenu/Build/Quick Bundle Build",true)]
		private static bool IsEnableBundleBuild()
		{
			return false;
		}
	}
}
#endif