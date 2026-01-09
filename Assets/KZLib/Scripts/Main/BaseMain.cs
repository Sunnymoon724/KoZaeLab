using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using KZLib.KZAttribute;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using KZLib.KZData;
using System.Collections;
using MessagePipe;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public abstract class BaseMain : SerializedMonoBehaviour
	{
		private const string c_titleScene = "TitleScene";

		[SerializeField,HideInInspector]
		private SystemLanguage? m_gameLanguage = null;

		[VerticalGroup("2",2),ShowInInspector,InlineButton(nameof(_OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.Reply)]
		public SystemLanguage GameLanguage
		{
			get
			{
				if(!m_gameLanguage.HasValue)
				{
					var optionCfg = ConfigManager.In.Access<OptionConfig>();

					m_gameLanguage = optionCfg.CurrentLanguage;
				}

				return m_gameLanguage.Value;
			}

			set
			{
				if(m_gameLanguage == value)
				{
					return;
				}

				m_gameLanguage = value;

				var optionCfg = ConfigManager.In.Access<OptionConfig>();

				optionCfg.SetLanguage(m_gameLanguage.Value);
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
		private int m_safeWidth = 0;

		[SerializeField,HideInInspector]
		private Vector2Int m_screenResolution = Vector2Int.zero;

		[VerticalGroup("30",30),ShowInInspector,KZRichText("width : {0}, height : {1}")]
		public Vector2Int ScreenResolution { get => m_screenResolution; private set => m_screenResolution = value; }

		private enum PlayType { Test, Normal, }

		private PlayType? m_gamePlayType = null;

		[VerticalGroup("0",0),ShowInInspector]
		private PlayType GamePlayType
		{
			get
			{
#if UNITY_EDITOR
				if(!m_gamePlayType.HasValue)
				{
					m_gamePlayType = MainPreset.GamePlayType;
				}

				return m_gamePlayType.Value;
#else
				return PlayType.Normal;
#endif
			}
			set
			{
#if UNITY_EDITOR
				if(m_gamePlayType == value)
				{
					return;
				}

				m_gamePlayType = MainPreset.GamePlayType = value;

				_SavePresetInfo();
#endif
			}
		}

		[SerializeField,HideInInspector]
		private string m_startSceneName = "";

		[VerticalGroup("0",0),ShowInInspector,ValueDropdown(nameof(SceneNameGroup)),EnableIf(nameof(IsTestMode)),InfoBox("TitleScene is not include in SceneArray.",InfoMessageType.Warning,nameof(IsIncludeTitleName))]
		private string StartSceneName
		{
			get
			{
#if UNITY_EDITOR
				if(m_startSceneName.IsEmpty())
				{
					m_startSceneName = IsTestMode ? MainPreset.StartSceneName : c_titleScene;
				}

				return IsTestMode ? m_startSceneName : c_titleScene;
#else
				return c_TitleScene;
#endif
			}
			set
			{
#if UNITY_EDITOR
				if(m_startSceneName == value)
				{
					return;
				}

				m_startSceneName = MainPreset.StartSceneName = value;

				_SavePresetInfo();
#endif
			}
		}

		protected bool IsTestMode => GamePlayType == PlayType.Test;
		private bool IsIncludeTitleName => !SceneNameList.Contains(c_titleScene);

		protected CancellationTokenSource m_tokenSource = null;
		
		private ResolutionMonitor m_resolutionMonitor = null;

		protected virtual void Awake()
		{
			m_isPlaying = true;

			LogSvc.System.I("Initialize Main");

			//? 에디터가 아니면 Normal Play
#if !UNITY_EDITOR
			GamePlayType = PlayType.Normal;
#endif
			LogSvc.System.I($"Current PlayType {GamePlayType}");
		}

		private async void Start()
		{
			var gameCfg = ConfigManager.In.Access<GameConfig>();

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

			if(m_safeWidth <= 0)
			{
				m_safeWidth = (int) Screen.safeArea.width;
			}

			DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
			DOTween.SetTweensCapacity(1000,100);

			var stringBuilder = new StringBuilder();

			_InitializeResolution(stringBuilder);
			_InitializeFrame(stringBuilder);
			_InitializeRenderSetting(stringBuilder);
			_InitializeObject(stringBuilder);

			LogSvc.System.I(stringBuilder.ToString());

			m_resolutionMonitor = new ResolutionMonitor();

			await _StartMainAsync();
		}

		private async UniTask _StartMainAsync()
		{
			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			if(IsTestMode)
			{
#if UNITY_EDITOR
				await _InitializeTestMode(m_tokenSource.Token);

				SceneStateManager.In.AddSceneNoLoading(StartSceneName,CommonUINameTag.None);
#else
				throw new Exception("This cannot be tested outside of the editor mode.");
#endif
			}
			else
			{
				await _InitializeNormalMode(m_tokenSource.Token);

				SceneStateManager.In.AddSceneNoLoading(StartSceneName,CommonUINameTag.None);
			}
		}

#if UNITY_EDITOR
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

		private PresetInfo m_mainPreset;

		private PresetInfo MainPreset
		{
			get
			{
				if(m_mainPreset == null)
				{
					var text = EditorPrefs.GetString(c_mainPreset,"");

					m_mainPreset = text.IsEmpty() ? new PresetInfo() : JsonConvert.DeserializeObject<PresetInfo>(text);
				}

				return m_mainPreset;
			}
		}

		private void _SavePresetInfo()
		{
			EditorPrefs.SetString(c_mainPreset,JsonConvert.SerializeObject(MainPreset));
		}
#endif

#if UNITY_EDITOR
		protected async virtual UniTask _InitializeTestMode(CancellationToken token) { await _InitializeManager(token); }
#endif
		protected async virtual UniTask _InitializeNormalMode(CancellationToken token) { await _InitializeManager(token); }
		
		protected async UniTask _InitializeManager(CancellationToken token)
		{
			await ProtoManager.In.TryLoadAsync(token);
			await LingoManager.In.TryLoadAsync(token);
		}

		protected virtual void _InitializeResolution(StringBuilder stringBuilder)
		{
			//? 모바일에서 화면잠김을 방지하기 위한 값.
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
#if UNITY_ANDROID || UNITY_IOS
			QualitySettings.vSyncCount	= 0;
			Application.targetFrameRate = Global.FRAME_RATE_30;
#elif UNITY_STANDALONE
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = Global.FRAME_RATE_60;
#endif
			stringBuilder.AppendFormat($"Current FPS {Application.targetFrameRate}\n");
		}

		protected virtual void _InitializeRenderSetting(StringBuilder stringBuilder) { }

		protected virtual void _InitializeObject(StringBuilder stringBuilder) { }

		protected virtual void OnDestroy()
		{
			LogSvc.System.I("Release Main");

			m_isPlaying = false;

			CommonUtility.ReleaseManager();
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
			{
				DebugOverlayManager.In.ToggleOverlay();
			}

			//? Create Exception
			if(Input.GetKeyDown(KeyCode.F4))
			{
				throw new Exception("Force Exception");
			}

			//? Refresh Game
			if(Input.GetKeyDown(KeyCode.F5))
			{
				LogSvc.System.I("Refresh Game");

				_RefreshGame().Forget();
			}
#endif
		}

#if UNITY_EDITOR
		private async UniTask _RefreshGame()
		{
			await SceneStateManager.In.RemoveSceneNoLoadingAsync(CommonUINameTag.None);

			await _StartMainAsync();
		}
#endif

		private void OnApplicationFocus( bool focus )
		{
			if (focus)
			{
				LogSvc.System.I("Move to foreground");

				m_resolutionMonitor?.StartResolutionDetection();
			}
			else
			{
				LogSvc.System.I("Move to background");

				m_resolutionMonitor?.StopResolutionDetection();
			}

			GlobalMessagePipe.GetPublisher<CommonNoticeTag,bool>().Publish(CommonNoticeTag.ChangedApplicationFocus,focus);
		}

		private readonly List<string> m_sceneNameList = new();
		private List<string> SceneNameList => SceneNameGroup as List<string>;

		private IEnumerable SceneNameGroup
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
	}
}