#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS

using System.IO;
using UnityEditor.iOS.Xcode;

#endif

namespace KZLib.KZBuild
{
#pragma warning disable IDE0051
	public static class KZPostProcessBuild
	{
		[PostProcessBuild]
		private static void OnPostProcessBuild(BuildTarget buildTarget,string path)
		{
#if UNITY_IOS
			var projectPath = PBXProject.GetPBXProjectPath(path);
			var project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projectPath));
			var targetGUID = project.GetUnityFrameworkTargetGuid();
			project.AddFrameworkToProject(targetGUID,"CoreHaptics.framework",false);
			File.WriteAllText(projectPath,project.WriteToString());
#endif
		}
	}
#pragma warning restore IDE0051
}
#endif