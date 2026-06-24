#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS

using System.IO;
using UnityEditor.iOS.Xcode;

#endif

namespace KZLib.Build
{
	/// <summary>
	/// Patches the Xcode project after an iOS build.
	/// <see cref="KZLib.Natives.VibrationManager"/> uses Core Haptics, so this links <c>CoreHaptics.framework</c>
	/// on the UnityFramework target (Unity does not add it automatically).
	/// </summary>
	public static class KZPostProcessBuild
	{
		[PostProcessBuild]
		public static void _OnPostProcessBuild(BuildTarget buildTarget,string path)
		{
#if UNITY_IOS
			var projectPath = PBXProject.GetPBXProjectPath(path);
			var project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projectPath));

			// Native plugins live in UnityFramework, so link the framework on that target.
			var targetGUID = project.GetUnityFrameworkTargetGuid();
			project.AddFrameworkToProject(targetGUID,"CoreHaptics.framework",false);

			File.WriteAllText(projectPath,project.WriteToString());
#endif
		}
	}
}
#endif