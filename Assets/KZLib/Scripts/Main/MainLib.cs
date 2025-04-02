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
using ExcelDataReader.Log;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public abstract class MainLib : SerializedMonoBehaviour
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
					var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

					m_gameLanguage = optionCfg.Language;
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

				var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

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

				_SavePresetData();
#endif
			}
		}

		[SerializeField,HideInInspector]
		private string m_startSceneName = "";

		[VerticalGroup("0",0),ShowInInspector,ValueDropdown(nameof(SceneNameList)),EnableIf(nameof(IsTestMode)),InfoBox("TitleScene is not include in SceneArray.",InfoMessageType.Warning,nameof(IsIncludeTitleName))]
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
				return c_titleScene;
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

				_SavePresetData();
#endif
			}
		}

		protected bool IsTestMode => GamePlayType == PlayType.Test;
		private bool IsIncludeTitleName => !SceneNameList.Contains(c_titleScene);

		protected CancellationTokenSource m_tokenSource = null;

		protected virtual void Awake()
		{
			m_isPlaying = true;

			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("Initialize Main");

			//? 에디터가 아니면 Normal Play
#if !UNITY_EDITOR
			GamePlayType = PlayType.Normal;
#endif
			stringBuilder.AppendLine($"Current PlayType {GamePlayType}");

			LogTag.System.I(stringBuilder.ToString());
		}

		private async void Start()
		{
			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

#if UNITY_EDITOR
			if(gameCfg.UseHeadUpDisplay)
			{
				UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
			}
#else
			if(Debug.isDebugBuild && gameCfg.UseHeadUpDisplay)
			{
				UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
			}
#endif

			if(m_safeWidth <= 0)
			{
				m_safeWidth = (int) Screen.safeArea.width;
			}

			DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
			DOTween.SetTweensCapacity(1000,100);

			var stringBuilder = new StringBuilder();

			InitializeResolution(stringBuilder);
			InitializeFrame(stringBuilder);
			InitializeRenderSetting(stringBuilder);
			InitializeObject(stringBuilder);

			LogTag.System.I(stringBuilder.ToString());

			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			if(IsTestMode)
			{
#if UNITY_EDITOR
				await StartTestMode(m_tokenSource.Token);
#else
				throw new Exception("This cannot be tested outside of the editor mode.");
#endif
			}
			else
			{
				await StartNormalMode(m_tokenSource.Token);
			}

			var editorCfg = ConfigMgr.In.Access<ConfigData.EditorConfig>();
			var paramText = editorCfg.GetSceneParamText(StartSceneName);

			if(!paramText.IsEmpty())
			{
				LogTag.System.I($"{StartSceneName} - [{paramText}]");
			}

			var paramName = StartSceneName.Replace("Scene","Param");
			var paramType = Type.GetType($"{StartSceneName}.{paramName}, Assembly-CSharp");

			var stateParam = JsonConvert.DeserializeObject(paramText,paramType) as SceneState.StateParam;

			SceneStateMgr.In.AddSceneNoLoading(StartSceneName,null,stateParam);
		}

#if UNITY_EDITOR
		private const string c_mainPreset = "[Main] MainPreset";

		private class PresetData
		{
			public PlayType GamePlayType { get; set; } = PlayType.Normal;
			public string StartSceneName { get; set; } = "TitleScene";
		}

		private PresetData m_mainPreset;

		private PresetData MainPreset
		{
			get
			{
				if(m_mainPreset == null)
				{
					var text = EditorPrefs.GetString(c_mainPreset,"");

					m_mainPreset = text.IsEmpty() ? new PresetData() : JsonConvert.DeserializeObject<PresetData>(text);
				}

				return m_mainPreset;
			}
		}

		private void _SavePresetData()
		{
			EditorPrefs.SetString(c_mainPreset,JsonConvert.SerializeObject(MainPreset));
		}
#endif

#if UNITY_EDITOR
		protected async virtual UniTask StartTestMode(CancellationToken token) { await ProtoMgr.In.TryLoadAsync(token); }
#endif
		protected async virtual UniTask StartNormalMode(CancellationToken token) { await ProtoMgr.In.TryLoadAsync(token); }

		protected virtual void InitializeResolution(StringBuilder stringBuilder)
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

		protected virtual void InitializeFrame(StringBuilder stringBuilder)
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

		protected virtual void InitializeRenderSetting(StringBuilder stringBuilder)
		{

		}

		protected virtual void InitializeObject(StringBuilder stringBuilder)
		{

		}

		protected virtual void OnDestroy()
		{
			m_isPlaying = false;

			CommonUtility.ReleaseManager();
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
			{
				if(UIMgr.In.IsOpened(UITag.HudPanelUI))
				{
					UIMgr.In.Close(UITag.HudPanelUI);
				}
				else
				{
					UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
				}
			}

			//? Create Exception
			if(Input.GetKeyDown(KeyCode.F4))
			{
				throw new Exception("Force Exception");
			}
#endif
		}

#if UNITY_EDITOR
		private static readonly List<string> s_sceneNameList = new();

		private static List<string> SceneNameList
		{
			get
			{
				if(s_sceneNameList.IsNullOrEmpty())
				{
					foreach(var scene in EditorBuildSettings.scenes)
					{
						var sceneName = Path.GetFileNameWithoutExtension(scene.path);

						if(sceneName == "MainScene")
						{
							continue;
						}

						s_sceneNameList.Add(sceneName);
					}
				}

				return s_sceneNameList;
			}
		}
#endif
	}
}