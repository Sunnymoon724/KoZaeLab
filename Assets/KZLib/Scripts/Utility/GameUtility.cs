using System;
using KZLib;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public static class GameUtility
{
	public static void ClearUnloadedAssetMemory()
	{
		Resources.UnloadUnusedAssets();

		GC.Collect();
	}

	public static bool CheckVersion(string _version)
	{
		return GameSettings.In.GameVersion.CompareTo(_version) >= 0;
	}

	public static string CheckMetaData(IMetaData _metaData,int _metaId)
	{
		if(_metaData == null || !_metaData.IsExist)
		{
			//? Not exist
			return $"<color={Global.WRONG_HEX_COLOR}>{_metaId}</color>";
		}

		if(!CheckVersion(_metaData.Version))
		{
			//? Version error
			return $"<color={Global.DISABLE_HEX_COLOR}>{_metaId}</color>";
		}

		return $"{_metaId}";
	}

	public static void LockInput()
	{
		SetInput(true);
	}

	public static void UnLockInput()
	{
		SetInput(false);
	}
	
	private static void SetInput(bool _lock)
	{
		// TODO Check Input

		if(UIMgr.HasInstance)
		{
			UIMgr.In.BlockInput(_lock);
		}
	}

	public static long GetCurrentTimeToMilliSecond()
	{
		return DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
	}

	/// <summary>
	/// Get New Week Monday
	/// </summary>
	public static DateTime GetNewWeekMonday()
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

	public static string GetDownloadSpeed(long _tick)
	{
		var size = _tick/1024.0d;

		return size > 0.0d ? string.Format("{0:F2} MB/s",size/1024.0d) : string.Format("{0} B/s",_tick);
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

		ReflectionUtility.ClearCacheData();
		StringExtension.ClearCacheData();
		ColorExtension.ClearCacheData();
	}

	private static void ReleaseSingletonMB<TBehaviour>() where TBehaviour : AutoSingletonMB<TBehaviour>
	{
		if(AutoSingletonMB<TBehaviour>.HasInstance)
		{
			UnityUtility.DestroyObject(AutoSingletonMB<TBehaviour>.In.gameObject);
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
	public static string GetTemplateFilePath(string _fileName)
	{
		var projectPath = FileUtility.GetProjectPath();
		var packagePath = string.Format("Packages/com.bsheepstudio.kzlib/WorkResources/Templates/{0}",_fileName);

		if(FileUtility.IsExist(FileUtility.PathCombine(projectPath,packagePath)))
		{
			return packagePath;
		}

		var assetPath = string.Format("Assets/KZLib/WorkResources/Templates/{0}",_fileName);

		if(FileUtility.IsExist(FileUtility.GetAbsolutePath(assetPath,true)))
		{
			return assetPath;
		}

		throw new NullReferenceException($"{_fileName} is not exist in template folder.");
	}

	public static byte[] GetTestImageData()
	{
		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(GetTemplateFilePath("Ostrich.png"));

		return sprite != null ? sprite.texture.EncodeToPNG() : null;
	}

	public static string GetTemplateFileAbsolutePath(string _fileName)
	{
		var filePath = GetTemplateFilePath(_fileName);

		return FileUtility.IsStartWithAssetsHeader(filePath) ? FileUtility.GetAbsolutePath(filePath,true) : FileUtility.PathCombine(FileUtility.GetProjectPath(),filePath);
	}
#endif
}