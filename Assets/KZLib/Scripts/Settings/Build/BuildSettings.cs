#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEditor;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private const string BUILD_REPORT = "Build Report";

	private bool DisableNow => true;

	[Flags]
	private enum UploadType { None = 0, GoogleDrive = 1<<0, All = -1 };

	private string GetFullPath(string _type)
	{
		return CommonUtility.PathCombine(CommonUtility.GetProjectPath(),"Builds",_type,CurrentBuildTargetToString);
	}

	private BuildTarget CurrentBuildTarget => EditorUserBuildSettings.activeBuildTarget;
	private string CurrentBuildTargetToString => CurrentBuildTarget.ToString();
	private string CurrentBuildTargetToLower => CurrentBuildTarget.ToString().ToLowerInvariant();

	private async UniTaskVoid BuildAsync(Func<UniTask> _onBuildTask)
	{
		LogMgr.In.ClearLog();

		var currentTarget = CurrentBuildTarget;

		try
		{
			await _onBuildTask();
		}
		catch(Exception _exception)
		{
			LogTag.Build.E($"Build failed!\n{_exception.Message}");
		}
		finally
		{
			CommonUtility.ClearProgressBar();

			if(CurrentBuildTarget != currentTarget)
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(currentTarget),currentTarget);
			}

			await WebRequestUtility.PostWebHook_DiscordAsync(BUILD_REPORT,LogMgr.In.LogDataGroup);
		}
	}

	private (string FileName,byte[] FileData) ConvertToFileGroup(string _filePath)
	{
		return (CommonUtility.GetFileName(_filePath),CurrentBuildTarget == BuildTarget.Android ? CommonUtility.ReadFileToBytes(_filePath) : CommonUtility.CompressZip(_filePath));
	}
}
#endif