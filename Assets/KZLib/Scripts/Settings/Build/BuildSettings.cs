#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEditor;
using UnityEngine;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private bool DisableNow => true;

	[Flags]
	private enum UploadType { None = 0, GoogleDrive = 1<<0, Azure = 1<<1, All = -1 };

	private readonly Queue<LogData> m_LogDataQueue = new();

	protected override void Initialize()
	{
		InitializeApp();

		InitializeBundle();
	}

	private string GetFullPath(string _type)
	{
		return CommonUtility.PathCombine(CommonUtility.GetProjectPath(),"Builds",_type,CurrentBuildTargetToString);
	}

	private BuildTarget CurrentBuildTarget => EditorUserBuildSettings.activeBuildTarget;
	private string CurrentBuildTargetToString => CurrentBuildTarget.ToString();
	private string CurrentBuildTargetToLower => CurrentBuildTarget.ToString().ToLowerInvariant();

	private async UniTaskVoid BuildAsync(Func<UniTask> _onBuildTask)
	{
		Application.logMessageReceived += OnGetLog;

		var currentTarget = CurrentBuildTarget;

		try
		{
			await _onBuildTask();
		}
		catch(Exception _exception)
		{
			Log.Build.E("빌드 실패\n{0}",_exception.Message);
		}
		finally
		{
			CommonUtility.ClearProgressBar();

			Application.logMessageReceived -= OnGetLog;

			if(CurrentBuildTarget != currentTarget)
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(currentTarget),currentTarget);
			}

			await CommonUtility.SendBuildReportAsync(m_LogDataQueue);
		}
	}

	private void OnGetLog(string _condition,string _stack,LogType _type)
	{
		m_LogDataQueue.Enqueue(new LogData(_type,_condition));
	}

	private (string,byte[]) ConvertToFileGroup(string _filePath)
	{
		if(CurrentBuildTarget == BuildTarget.Android)
		{
			return (CommonUtility.GetFileName(_filePath),CommonUtility.ReadFile(_filePath));
		}
		else
		{
			// 압축해야 함

			return (null,null);
		}
	}
}
#endif