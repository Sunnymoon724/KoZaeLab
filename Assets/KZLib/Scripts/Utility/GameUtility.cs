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
		var version = 0.0f;//GameSettings.In.GameVersion;

		return version.CompareTo(_version) >= 0;
	}

	public static string CheckMetaData(IMetaData _metaData,int _metaId)
	{
		if(_metaData == null || !_metaData.IsExist)
		{
			//? 존재 X
			return string.Format("<color={0}>{1}</color>",Global.WRONG_HEX_COLOR,_metaId);
		}

		if(!CheckVersion(_metaData.Version))
		{
			//? 버전이 안맞음
			return string.Format("<color={0}>{1}</color>",Global.DISABLE_HEX_COLOR,_metaId);
		}

		return string.Format("{0}",_metaId);
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
		// TODO 인풋 체크하기

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
	/// 새로운 주의 월요일을 리턴한다. 
	/// </summary>
	public static DateTime GetNewWeekMonday()
	{
		// 다음주 월요일을 계산한다.
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

	/// <summary>
	/// 오늘 남은 시간
	/// </summary>
	public static TimeSpan GetRemainTime()
	{
		return GetNextDay().Date-DateTime.Now;
	}

	/// <summary>
	/// 다음 요일 12:00 정각을 리턴함
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
		//? SingletonMB 릴리즈

		ReleaseSingletonMB<SceneMgr>();
		ReleaseSingletonMB<UIMgr>();
		ReleaseSingletonMB<CameraMgr>();

		//? Singleton 릴리즈
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

		throw new NullReferenceException(string.Format("템플릿 폴더에 해당 파일이 없습니다. [{0}]",_fileName));
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