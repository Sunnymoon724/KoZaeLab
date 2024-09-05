#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEditor;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private bool DisableNow => true;

	[Flags]
	private enum UploadType { None = 0, GoogleDrive = 1<<0, Azure = 1<<1, All = -1 };

	protected override void Initialize()
	{
		InitializeApp();

		InitializeBundle();
	}

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
			LogTag.Build.E("빌드 실패\n{0}",_exception.Message);
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
	private (string,byte[]) ConvertToFileGroup(string _filePath)
	{
		if(CurrentBuildTarget == BuildTarget.Android)
		{
			return (FileUtility.GetFileName(_filePath),FileUtility.ReadFile(_filePath));
		}
		else
		{
			// 압축해야 함

			return (null,null);
		}
	}
}
#endif