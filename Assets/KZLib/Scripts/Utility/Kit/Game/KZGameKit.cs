using KZLib;
using UnityEngine;
using KZLib.Utilities;
using System.IO;
using KZLib.Data;
using KZLib.Networks;
using KZLib.Webhooks;
using KZLib.Natives;


#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// Utility methods for game lifecycle control, manager teardown, and environment detection.
/// </summary>
public static partial class KZGameKit
{
	/// <summary>
	/// Pauses the game by setting time scale to zero and pausing the audio listener.
	/// </summary>
	public static void PauseGame()
	{
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
	}

	/// <summary>
	/// Resumes the game by restoring time scale and unpausing the audio listener.
	/// </summary>
	public static void UnPauseGame()
	{
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
	}

	/// <summary>
	/// Returns whether the game is currently paused (time scale is zero).
	/// </summary>
	public static bool IsPaused()
	{
		return Time.timeScale == 0.0f;
	}

	/// <summary>
	/// Stops play mode in the editor or quits the application at runtime.
	/// </summary>
	public static void Quit()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	/// <summary>
	/// Disposes all registered singleton managers in a defined release order.
	/// </summary>
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
		_ReleaseSingleton<TuneManager>();

		_ReleaseSingleton<ShaderManager>();
		_ReleaseSingleton<GameTimeManager>();
		_ReleaseSingleton<ServerClockManager>();

		_ReleaseSingleton<TimeManager>();
		_ReleaseSingleton<LuaManager>();

		_ReleaseSingleton<NetworkManager>();
		_ReleaseSingleton<WebhookManager>();

		_ReleaseSingleton<VibrationManager>();
		_ReleaseSingleton<PushManager>();

		_ReleaseSingleton<ContextManager>();

		_ReleaseSingleton<TimelineManager>();

#if KZLIB_PLAY_FAB
		_ReleaseSingleton<PlayFabManager>();
#endif
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

	/// <summary>
	/// Returns the current build environment type.
	/// In the editor, reads BranchInfo.txt from the project parent path; defaults to DEV otherwise.
	/// </summary>
	public static EnvironmentType GetCurrentEnvironmentType()
	{
#if UNITY_EDITOR
		var text = KZFileKit.ReadFileToText(Path.Combine(Global.ProjectParentPath,"BranchInfo.txt"));

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
