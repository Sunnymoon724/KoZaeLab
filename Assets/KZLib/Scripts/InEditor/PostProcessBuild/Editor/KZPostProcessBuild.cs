using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS

using System.IO;
using UnityEditor.iOS.Xcode;

#endif

namespace KZLib.Build
{
	public static class KZPostProcessBuild
	{
		[PostProcessBuild]
		private static void OnPostProcessBuild(BuildTarget _buildTarget,string _path)
		{
#if UNITY_IOS
			var projectPath = PBXProject.GetPBXProjectPath(_path);
			var project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projectPath));
			var targetGUID = project.GetUnityFrameworkTargetGuid();
			project.AddFrameworkToProject(targetGUID,"CoreHaptics.framework",false);
			File.WriteAllText(projectPath,project.WriteToString());
#endif
		}
	}
}
