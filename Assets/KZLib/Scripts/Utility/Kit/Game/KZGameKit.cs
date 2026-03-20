using KZLib;
using UnityEngine;
using KZLib.Utilities;
using System.IO;
using KZLib.Data;
using KZLib.Networking;

#if UNITY_EDITOR

using UnityEditor;

#endif

public static partial class KZGameKit
{
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

		_ReleaseSingletonMB<InputManager>();

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

		_ReleaseSingleton<RouteManager>();

		_ReleaseSingleton<ShaderManager>();
		_ReleaseSingleton<GameTimeManager>();

		_ReleaseSingleton<TimeManager>();
		_ReleaseSingleton<LuaManager>();

		_ReleaseSingleton<NetworkManager>();
		_ReleaseSingleton<WebRequestManager>();

		_ReleaseSingleton<VibrationManager>();

		_ReleaseSingleton<TimelineManager>();

#if KZLIB_PLAY_FAB
		_ReleaseSingleton<PlayFabManager>();
#endif

		KZReflectionKit.ClearCache();

		StringExtension.ClearCache();
		ColorExtension.ClearCache();
	}

	private static void _ReleaseSingletonMB<TBehaviour>() where TBehaviour : SingletonMB<TBehaviour>
	{
		if(SingletonMB<TBehaviour>.HasInstance)
		{
			SingletonMB<TBehaviour>.In.gameObject.DestroyObject();
		}
	}

	private static void _ReleaseSingleton<TClass>() where TClass : Singleton<TClass>
	{
		if(Singleton<TClass>.HasInstance)
		{
			Singleton<TClass>.In.Dispose();
		}
	}

	public static EnvironmentType GetCurrentEnvironmentType()
	{
#if UNITY_EDITOR
		var text = KZFileKit.ReadFileToText(Path.Combine(Global.PROJECT_PARENT_PATH,"BranchInfo.txt"));

		if(text.TryToEnum<EnvironmentType>(out var environment))
		{
			return environment;
		}

		return EnvironmentType.DEV;
#else
		return EnvironmentType.DEV;
#endif
	}
}