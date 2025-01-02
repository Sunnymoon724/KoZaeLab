using System;
using KZLib;
using UnityEngine;
using KZLib.KZUtility;

#if UNITY_EDITOR

using UnityEditor;

#endif

public static partial class CommonUtility
{
	public static void ClearUnloadedAssetMemory()
	{
		Resources.UnloadUnusedAssets();

		GC.Collect();
	}

	public static bool CheckVersion(string version)
	{
		return GameSettings.In.GameVersion.CompareTo(version) >= 0;
	}

	// public static string CheckMetaData(IMetaData _metaData,int _metaId)
	// {
	// 	if(_metaData == null || !_metaData.IsExist)
	// 	{
	// 		//? Not exist
	// 		return $"<color={Global.WRONG_HEX_COLOR}>{_metaId}</color>";
	// 	}

	// 	if(!CheckVersion(_metaData.Version))
	// 	{
	// 		//? Version error
	// 		return $"<color={Global.DISABLE_HEX_COLOR}>{_metaId}</color>";
	// 	}

	// 	return $"{_metaId}";
	// }

	public static void LockInput()
	{
		SetInput(true);
	}

	public static void UnLockInput()
	{
		SetInput(false);
	}
	
	private static void SetInput(bool isLock)
	{
		// TODO Check Input

		if(UIMgr.HasInstance)
		{
			UIMgr.In.BlockInput(isLock);
		}
	}

	public static long GetCurrentTimeToMilliSecond()
	{
		return DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
	}

	/// <summary>
	/// Get New Week Monday
	/// </summary>
	public static DateTime FindNewWeekMonday()
	{
		//? Calculate next Monday.
		var today = DateTime.Today;

		for(var i=1;i<8;i++)
		{
			today = DateTime.Today.AddDays(i);

			if(today.DayOfWeek == DayOfWeek.Monday)
			{
				break;
			}
		}

		return today;
	}

	public static TimeSpan GetRemainTime()
	{
		return GetNextDay().Date-DateTime.Now;
	}

	/// <summary>
	/// Get Next Day -> 12:00
	/// </summary>
	public static DateTime GetNextDay()
	{
		return DateTime.Today.AddDays(1);
	}

	public static void PauseGame()
	{
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
	}

	public static void UnPauseGame()
	{
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
	}

	public static bool IsPaused()
	{
		return Time.timeScale == 0.0f;
	}

	public static string GetDownloadSpeed(long tick)
	{
		var size = tick/1024.0d;

		return size > 0.0d ? $"{size / 1024.0d:f2} MB/s" : $"{tick} B/s";
	}

	public static void Quit()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public static void ReleaseManager()
	{
		//? Release SingletonMB
		ReleaseSingletonMB<SceneMgr>();
		ReleaseSingletonMB<UIMgr>();
		ReleaseSingletonMB<CameraMgr>();

		//? Release Singleton
		ReleaseSingleton<MetaDataMgr>();
		ReleaseSingleton<GameDataMgr>();
		ReleaseSingleton<SaveDataMgr>();
		ReleaseSingleton<ResMgr>();
		ReleaseSingleton<LogMgr>();
		ReleaseSingleton<AddressablesMgr>();
		ReleaseSingleton<SoundMgr>();
		ReleaseSingleton<ShaderMgr>();

		ClearCacheData();
		StringExtension.ClearCacheData();
		ColorExtension.ClearCacheData();
	}

	private static void ReleaseSingletonMB<TBehaviour>() where TBehaviour : AutoSingletonMB<TBehaviour>
	{
		if(AutoSingletonMB<TBehaviour>.HasInstance)
		{
			AutoSingletonMB<TBehaviour>.In.gameObject.DestroyObject();
		}
	}

	private static void ReleaseSingleton<TClass>() where TClass : Singleton<TClass>,new()
	{
		if(Singleton<TClass>.HasInstance)
		{
			Singleton<TClass>.In.Dispose();
		}
	}

#if UNITY_EDITOR
	public static string FindTemplateFilePath(string fileName)
	{
		var projectPath = GetProjectPath();
		var packagePath = $"Packages/com.bsheepstudio.kzlib/WorkResources/Templates/{fileName}";

		if(IsFileExist(PathCombine(projectPath,packagePath)))
		{
			return packagePath;
		}

		var assetPath = $"Assets/KZLib/WorkResources/Templates/{fileName}";

		if(IsFileExist(GetAbsolutePath(assetPath,true)))
		{
			return assetPath;
		}

		throw new NullReferenceException($"{fileName} is not exist in template folder.");
	}

	public static byte[] FindTestImageData()
	{
		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(FindTemplateFilePath("Ostrich.png"));

		return sprite != null ? sprite.texture.EncodeToPNG() : null;
	}

	public static string FindTemplateFileAbsolutePath(string fileName)
	{
		var templateFilePath = FindTemplateFilePath(fileName);

		return IsStartWithAssetsHeader(templateFilePath) ? GetAbsolutePath(templateFilePath,true) : PathCombine(GetProjectPath(),templateFilePath);
	}
#endif
}