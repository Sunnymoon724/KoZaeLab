#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEditor;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private bool DisableNow => true;

	[Flags]
	private enum UploadType { None = 0, GoogleDrive = 1<<0, All = -1 };

	private string GetFullPath(string _type)
	{
		return FileUtility.PathCombine(FileUtility.GetProjectPath(),"Builds",_type,CurrentBuildTargetToString);
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
			UnityUtility.ClearProgressBar();

			if(CurrentBuildTarget != currentTarget)
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(currentTarget),currentTarget);
			}

			await WebRequestUtility.SendBuildReportAsync(LogMgr.In.LogDataGroup);
		}
	}

	private (string FileName,byte[] FileData) ConvertToFileGroup(string _filePath)
	{
		return (FileUtility.GetFileName(_filePath),CurrentBuildTarget == BuildTarget.Android ? FileUtility.ReadFileToBytes(_filePath) : FileUtility.CompressZip(_filePath));
	}
}
#endif