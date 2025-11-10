using System;
using KZLib;
using UnityEngine;
using KZLib.KZUtility;
using System.IO;
using KZLib.KZData;
using KZLib.KZNetwork;

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

	public static long MemoryCheck()
	{
		return GC.GetTotalMemory(true);
	}

	public static void LockInput()
	{
		_SetInput(true);
	}

	public static void UnLockInput()
	{
		_SetInput(false);
	}
	
	private static void _SetInput(bool isLock)
	{
		// TODO Check Input

		if(UIManager.HasInstance)
		{
			if(isLock)
			{
				UIManager.In.BlockInput();
			}
			else
			{
				UIManager.In.AllowInput();
			}
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
		_ReleaseSingletonMB<SceneStateManager>();
		_ReleaseSingletonMB<UIManager>();

		_ReleaseSingletonMB<TouchManager>();

		_ReleaseSingletonMB<EffectManager>();
		_ReleaseSingletonMB<SoundManager>();

		//? Release Singleton
		_ReleaseSingleton<ProtoManager>();
		_ReleaseSingleton<ConfigManager>();
		_ReleaseSingleton<AffixManager>();
		_ReleaseSingleton<LingoManager>();

		_ReleaseSingleton<PlayerPrefsManager>();

		_ReleaseSingleton<ResourceManager>();
		_ReleaseSingleton<AddressablesManager>();

		_ReleaseSingleton<CameraManager>();
		_ReleaseSingleton<InputManager>();

		_ReleaseSingleton<RouteManager>();

		_ReleaseSingleton<ShaderManager>();
		_ReleaseSingleton<GameTimeManager>();

		_ReleaseSingleton<TimeManager>();
		_ReleaseSingleton<LuaManager>();

		_ReleaseSingleton<NetworkManager>();
		_ReleaseSingleton<WebRequestManager>();

		_ReleaseSingleton<VibrationManager>();

#if KZLIB_PLAY_FAB
		_ReleaseSingleton<PlayFabManager>();
#endif

		ClearCacheData();

		StringExtension.ClearCacheData();
		ColorExtension.ClearCacheData();
	}

	private static void _ReleaseSingletonMB<TBehaviour>() where TBehaviour : AutoSingletonMB<TBehaviour>
	{
		if(AutoSingletonMB<TBehaviour>.HasInstance)
		{
			AutoSingletonMB<TBehaviour>.In.gameObject.DestroyObject();
		}
	}

	private static void _ReleaseSingleton<TClass>() where TClass : Singleton<TClass>,new()
	{
		if(Singleton<TClass>.HasInstance)
		{
			Singleton<TClass>.In.Dispose();
		}
	}

	public static EnvironmentType GetCurrentEnvironmentType()
	{
#if UNITY_EDITOR
		var text = FileUtility.ReadFileToText(Path.Combine(Global.PROJECT_PARENT_PATH,"BranchInfo.txt"));

		if(text.TryToEnum<EnvironmentType>(out var environment))
		{
			return environment;
		}

		return EnvironmentType.DEV;
#else
		return EnvironmentType.DEV;
#endif
	}

#if UNITY_EDITOR
	public static string FindTemplateFilePath(string fileName)
	{
		var packagePath = Path.Combine("Packages","com.bsheepstudio.kzlib","WorkResources","Templates",$"{fileName}");

		if(File.Exists(Path.Combine(Global.PROJECT_PATH,packagePath)))
		{
			return packagePath;
		}

		var assetPath = Path.Combine("Assets","KZLib","WorkResources","Templates",$"{fileName}");

		if(File.Exists(Path.GetFullPath(assetPath)))
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

		return templateFilePath.StartsWith(Global.ASSETS_TEXT) ? Path.GetFullPath(templateFilePath) : Path.Combine(Global.PROJECT_PATH,templateFilePath);
	}
#endif
}