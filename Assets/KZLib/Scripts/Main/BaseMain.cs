using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using KZLib.Attributes;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using KZLib.Data;
using KZLib.Scenes;
using MessagePipe;
using KZLib.Natives;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	/// <summary>
	/// Game entry point: bootstraps managers, loads data, enters the first scene.
	/// Editor: Test/Normal play presets (<see cref="c_mainPreset"/>), F5 in-play refresh without stopping the editor.
	/// Extend in game <c>Main</c> via <see cref="_InitializeNormalMode"/>, <see cref="ApplyTestModeSceneTransient"/>, etc.
	/// </summary>
	public abstract class BaseMain : MonoBehaviour
	{
		private const string c_titleScene = "TitleScene";

		[SerializeField,HideInInspector]
		private SystemLanguage? m_gameLanguage = null;

		[VerticalGroup("2",2),ShowInInspector,InlineButton(nameof(_OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.Reply)]
		public SystemLanguage GameLanguage
		{
			get
			{
				if(m_gameLanguage.HasValue)
				{
					return m_gameLanguage.Value;
				}

#if UNITY_EDITOR
				//? Edit-mode inspector: show TuneManager language before play (no m_gameLanguage cache yet).
				if(!Application.isPlaying)
				{
					return TuneManager.In.Fetch<LanguageTune>().CurrentLanguage;
				}
#endif

				return SystemLanguage.English;
			}

			set
			{
				if(m_gameLanguage == value)
				{
					return;
				}

				m_gameLanguage = value;

				_SyncGameLanguage();
			}
		}

		private void _OnChangeDefaultLanguage()
		{
			GameLanguage = SystemLanguage.English;
		}

		[SerializeField,HideInInspector]
		private bool m_isPlaying = false;

		[VerticalGroup("20",20),ShowInInspector,KZRichText]
		public bool IsPlaying => m_isPlaying;

		[SerializeField,HideInInspector]
		private Vector2Int m_screenResolution = Vector2Int.zero;

		[VerticalGroup("30",30),ShowInInspector,KZRichText("width : {0}, height : {1}")]
		public Vector2Int ScreenResolution { get => m_screenResolution; private set => m_screenResolution = value; }

		private enum PlayType { Test, Normal, }

#if UNITY_EDITOR
		[VerticalGroup("0",0),ShowInInspector]
		private PlayType GamePlayType
		{
			get
			{
				var presetInfo = _LoadPresetInfo();

				return presetInfo.GamePlayType;
			}
			set
			{
				var presetInfo = _LoadPresetInfo();

				if(presetInfo.GamePlayType == value)
				{
					return;
				}

				presetInfo.GamePlayType = value;

				_SavePresetInfo(presetInfo);
			}
		}
#endif

#if UNITY_EDITOR
		[VerticalGroup("0",0),ShowInInspector,ValueDropdown(nameof(SceneNameGroup)),EnableIf(nameof(IsTestMode)),InfoBox("TitleScene is not included in SceneArray.",InfoMessageType.Warning,nameof(IsTitleSceneMissingFromSceneArray))]
		private string StartSceneName
		{
			get
			{
				if(!IsTestMode)
				{
					return c_titleScene;
				}

				var presetInfo = _LoadPresetInfo();

				var sceneName = presetInfo.StartSceneName;

				return sceneName.IsEmpty() ? c_titleScene : sceneName;
			}
			set
			{
				var presetInfo = _LoadPresetInfo();

				if(presetInfo.StartSceneName == value)
				{
					return;
				}

				presetInfo.StartSceneName = value;

				_SavePresetInfo(presetInfo);
			}
		}
#else
		private string StartSceneName => c_titleScene;
#endif

#if UNITY_EDITOR
		protected bool IsTestMode => GamePlayType == PlayType.Test;
#else
		protected bool IsTestMode => false;
#endif

		protected CancellationTokenSource m_tokenSource = null;
		
		private ResolutionMonitor m_resolutionMonitor = null;

		protected virtual void Awake()
		{
			m_isPlaying = true;

			LogChannel.Game.I("Initialize Main");

			LogChannel.Game.I($"Current PlayType {(IsTestMode ? PlayType.Test : PlayType.Normal)}");
		}

		private void _SyncGameLanguage()
		{
			//? Push cached language to TuneManager, or seed cache from TuneManager before Lingo load.
			var lanTun = TuneManager.In.Fetch<LanguageTune>();

			if(m_gameLanguage.HasValue)
			{
				lanTun.SetLanguage(m_gameLanguage.Value);
			}
			else
			{
				m_gameLanguage = lanTun.CurrentLanguage;
			}
		}

		private void Start()
		{
			_StartAsync().Forget();
		}

		/// <summary>DOTween, frame/resolution setup, <see cref="ResolutionMonitor"/>, then <see cref="_StartMainAsync"/>.</summary>
		private async UniTask _StartAsync()
		{
			try
			{
				var gameCfg = ConfigManager.In.Fetch<GameConfig>();

#if UNITY_EDITOR
				if(gameCfg.UseHeadUpDisplay)
				{
					DebugOverlayManager.In.ShowOverlay();
				}
#else
				if(Debug.isDebugBuild && gameCfg.UseHeadUpDisplay)
				{
					DebugOverlayManager.In.ShowOverlay();
				}
#endif

				DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
				DOTween.SetTweensCapacity(1000,100);

				var stringBuilder = new StringBuilder();

				_InitializeFrame(stringBuilder);
				_InitializeResolution(stringBuilder);
				_InitializeRenderSetting(stringBuilder);
				_InitializeObject(stringBuilder);

				LogChannel.Game.I(stringBuilder.ToString());

				void _OnResolutionChanged(Vector2Int resolution)
				{
					ScreenResolution = resolution;
				}

				m_resolutionMonitor = new ResolutionMonitor(_OnResolutionChanged);

				m_resolutionMonitor.StartResolutionDetection();

				await _StartMainAsync();
			}
			catch(OperationCanceledException)
			{
				_StopResolutionMonitor();

				LogChannel.Game.I("Main Start cancelled.");
			}
			catch(Exception exception)
			{
				m_isPlaying = false;

				_StopResolutionMonitor();

				LogChannel.Game.E($"Main Start failed.\n{exception}");
			}
		}

		private void _StopResolutionMonitor()
		{
			m_resolutionMonitor?.StopResolutionDetection();
			m_resolutionMonitor = null;
		}


		/// <summary>Load Proto/Lingo, init <see cref="ContextManager"/>, await first scene transition.</summary>
		private async UniTask _StartMainAsync()
		{
			KZExternalKit.RecycleTokenSourceInMono(ref m_tokenSource,this);

			_SyncGameLanguage();

#if UNITY_EDITOR
			if(IsTestMode)
			{
				await _InitializeTestMode(m_tokenSource.Token);
			}
			else
#endif
			{
				await _InitializeNormalMode(m_tokenSource.Token);
			}

			ContextManager.In.InitializeContext();

			await SceneStateManager.In.AddSceneAsync(StartSceneName,new SceneChangeInfo(CommonUINameTag.None));
		}

#if UNITY_EDITOR
		//? PlayType / start scene for editor play; persisted in EditorPrefs (no in-memory cache).
		private const string c_mainPreset = "[Main] MainPreset";

		private class PresetInfo
		{
			public PlayType GamePlayType { get; set; }
			public string StartSceneName { get; set; }
			
			public PresetInfo()
			{
				GamePlayType = PlayType.Normal;
				StartSceneName = c_titleScene;
			}
		}

		private PresetInfo _LoadPresetInfo()
		{
			var text = EditorPrefs.GetString(c_mainPreset,"");

			return text.IsEmpty() ? new PresetInfo() : JsonConvert.DeserializeObject<PresetInfo>(text) ?? new PresetInfo();
		}

		private void _SavePresetInfo(PresetInfo presetInfo)
		{
			EditorPrefs.SetString(c_mainPreset,JsonConvert.SerializeObject(presetInfo));
		}
#endif

#if UNITY_EDITOR
		/// <summary>TestMode: store scene transient before the start scene is entered. Override in game Main.</summary>
		protected virtual void ApplyTestModeSceneTransient(string sceneName,TestModeConfig config) { }

		protected virtual async UniTask _InitializeTestMode(CancellationToken token)
		{
			await _InitializeManager(token);

			var testModeCfg = ConfigManager.In.Fetch<TestModeConfig>();

			ApplyTestModeSceneTransient(StartSceneName,testModeCfg);
		}
#endif
		protected virtual async UniTask _InitializeNormalMode(CancellationToken token)
		{
			await _InitializeManager(token);
		}
		
		protected virtual async UniTask _InitializeManager(CancellationToken token)
		{
			if(!await ProtoManager.In.TryLoadAsync(token))
			{
				throw new InvalidOperationException("Proto load failed.");
			}

			if(!await LingoManager.In.TryLoadAsync(token))
			{
				throw new InvalidOperationException("Lingo load failed.");
			}
		}

		protected virtual void _InitializeResolution(StringBuilder stringBuilder)
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR || UNITY_STANDALONE
			ScreenResolution = new Vector2Int(Screen.width,Screen.height);
#else
			ScreenResolution = new Vector2Int(Screen.currentResolution.width,Screen.currentResolution.height);
#endif

			stringBuilder.AppendFormat($"Current Screen Resolution {ScreenResolution.x}x{ScreenResolution.y}\n");
		}

		protected virtual void _InitializeFrame(StringBuilder stringBuilder)
		{
			_ = GraphicManager.In;

			var graphicTun = TuneManager.In.Fetch<GraphicTune>();

			stringBuilder.AppendFormat($"Current FPS {graphicTun.CurrentFrameRate}\n");
		}

		protected virtual void _InitializeRenderSetting(StringBuilder stringBuilder) { }

		protected virtual void _InitializeObject(StringBuilder stringBuilder) { }

		/// <summary>Editor refresh: cleanup not handled by <see cref="KZGameKit.ReleaseManager"/>. Override in game Main.</summary>
		protected virtual UniTask OnRefreshTeardownAsync() => UniTask.CompletedTask;

		protected virtual void OnDestroy()
		{
			LogChannel.Game.I("Release Main");

			m_isPlaying = false;

			KZExternalKit.KillTokenSource(ref m_tokenSource);

			_StopResolutionMonitor();

			KZGameKit.ReleaseManager();
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			//? Editor debug: LeftShift+C toggles the debug HUD overlay on/off (intentional hotkey).
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
			{
				DebugOverlayManager.In.ToggleOverlay();
			}

			//? Editor debug: F4 intentionally throws to test exception handlers / crash reporting.
			if(Input.GetKeyDown(KeyCode.F4))
			{
				throw new Exception("Force Exception");
			}

			//? Editor debug: F5 tears down managers and re-runs _StartAsync without exiting play mode.
			if(Input.GetKeyDown(KeyCode.F5))
			{
				_RefreshGame().Forget();
			}
#endif
		}

#if UNITY_EDITOR
		//? Guards F5 re-entry while teardown or bootstrap is in progress.
		private bool m_isRefreshing = false;

		/// <summary>Editor F5: Teardown → <see cref="_StartAsync"/>. Main GameObject stays alive; <see cref="OnDestroy"/> is not called.</summary>
		private async UniTask _RefreshGame()
		{
			if(m_isRefreshing)
			{
				LogChannel.Game.W("Refresh Game is already in progress.");

				return;
			}

			m_isRefreshing = true;

			var refreshInputLocked = false;

			try
			{
				LogChannel.Game.I("Refresh Game started.");

				KZInputKit.LockInput();

				refreshInputLocked = true;

				await _TeardownForRefreshAsync();

				//? ReleaseManager.Reset clears the input lock acquired above.
				refreshInputLocked = false;

				KZInputKit.LockInput();

				refreshInputLocked = true;

				m_isPlaying = true;

				await _StartAsync();
			}
			catch(Exception exception)
			{
				m_isPlaying = false;

				LogChannel.Game.E($"Refresh Game failed.\n{exception}");
			}
			finally
			{
				if(refreshInputLocked)
				{
					KZInputKit.UnLockInput();
				}

				m_isRefreshing = false;
			}
		}

		/// <summary>Cancel work, remove current scene when safe, kill tweens, <see cref="KZGameKit.ReleaseManager"/>.</summary>
		private async UniTask _TeardownForRefreshAsync()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);

			_StopResolutionMonitor();

			await OnRefreshTeardownAsync();

			//? Skip when no scene is loaded (bootstrap failed) or a transition is already running.
			if(SceneStateManager.HasInstance && !SceneStateManager.In.IsSceneChanging)
			{
				try
				{
					await SceneStateManager.In.RemoveSceneAsync(new SceneChangeInfo(CommonUINameTag.None));
				}
				catch(InvalidOperationException exception)
				{
					LogChannel.Game.I($"Refresh skip remove scene. {exception.Message}");
				}
			}

			DOTween.KillAll();

			KZGameKit.ReleaseManager();
		}

		private bool IsTitleSceneMissingFromSceneArray => !SceneNameList.Contains(c_titleScene);

		private readonly List<string> m_sceneNameList = new();
		private List<string> SceneNameList => SceneNameGroup as List<string>;

		private IEnumerable<string> SceneNameGroup
		{
			get
			{
				if(m_sceneNameList.IsNullOrEmpty())
				{
					var sceneArray = EditorBuildSettings.scenes;

					for(var i=0;i<sceneArray.Length;i++)
					{
						var sceneName = Path.GetFileNameWithoutExtension(sceneArray[i].path);

						if(sceneName == "MainScene")
						{
							continue;
						}

						m_sceneNameList.Add(sceneName);
					}
				}

				return m_sceneNameList;
			}
		}
#endif

		private void OnApplicationFocus(bool focus)
		{
			//? Pause resolution polling in background; resume on foreground (see ResolutionMonitor).
			if(focus)
			{
				LogChannel.Game.I("Move to foreground");

				m_resolutionMonitor?.StartResolutionDetection();

				if(PushManager.HasInstance)
				{
					PushManager.In.ClearNotification();
				}
			}
			else
			{
				LogChannel.Game.I("Move to background");

				m_resolutionMonitor?.StopResolutionDetection();
			}

			GlobalMessagePipe.GetPublisher<CommonNoticeTag,bool>().Publish(CommonNoticeTag.ChangedApplicationFocus,focus);
		}
	}
}