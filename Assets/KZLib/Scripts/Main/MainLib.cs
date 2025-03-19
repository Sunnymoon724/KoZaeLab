using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using KZLib.KZAttribute;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public abstract class MainLib : SerializedMonoBehaviour
	{
		[SerializeField,HideInInspector]
		private SystemLanguage? m_gameLanguage = null;

		[ShowInInspector,InlineButton(nameof(OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.Reply)]
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

		private void OnChangeDefaultLanguage()
		{
			GameLanguage = SystemLanguage.English;
		}

		[SerializeField,HideInInspector]
		private bool m_isPlaying = false;

		[ShowInInspector,KZRichText]
		public bool IsPlaying => m_isPlaying;

		[SerializeField,HideInInspector]
		private int m_safeWidth = 0;

		[SerializeField,HideInInspector]
		private Vector2Int m_screenResolution = Vector2Int.zero;

		[ShowInInspector,KZRichText("width : {0}, height : {1}")]
		public Vector2Int ScreenResolution { get => m_screenResolution; private set => m_screenResolution = value; }

#if UNITY_EDITOR
		private const string c_main_data = "[Main] MainData";

		private class MainData
		{
			public PlayType GamePlayType { get; set; } = PlayType.Normal;
		}
#endif

		private enum PlayType { Test, Normal, }

		private PlayType? m_gamePlayType = null;

		[ShowInInspector,LabelText("Current PlayType")]
		private PlayType GamePlayType
		{
			get
			{
				if(!m_gamePlayType.HasValue)
				{
#if UNITY_EDITOR
					var data = LoadData();

					m_gamePlayType = data.GamePlayType;
#else
					m_gamePlayType = PlayType.Normal;
#endif
				}

				return m_gamePlayType.Value;
			}
			set
			{
#if UNITY_EDITOR
				if(m_gamePlayType == value)
				{
					return;
				}

				var data = LoadData();

				m_gamePlayType = data.GamePlayType = value;

				SaveData(data);
#endif
			}
		}

		protected bool IsTestMode => GamePlayType == PlayType.Test;

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
		}

#if UNITY_EDITOR
		private void SaveData(MainData mainData)
		{
			EditorPrefs.SetString(c_main_data,JsonConvert.SerializeObject(mainData));
		}

		private MainData LoadData()
		{
			var text = EditorPrefs.GetString(c_main_data,"");

			return text.IsEmpty() ? new MainData() : JsonConvert.DeserializeObject<MainData>(text);
		}

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
			CheckResolution();

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

		private void CheckResolution()
		{
			//? [Screen.width/height] is EDITOR or STANDALONE. / [Screen.currentResolution] is MOBILE.
#if UNITY_EDITOR || UNITY_STANDALONE
			if(Screen.width != ScreenResolution.x || Screen.height != ScreenResolution.y)
			{
				ScreenResolution = new(Screen.width,Screen.height);

				if(UIMgr.HasInstance)
				{
					UIMgr.In.ChangeScreenSize(ScreenResolution);
				}
			}
#else
			if(Screen.currentResolution.width != ScreenResolution.x || Screen.currentResolution.height != ScreenResolution.y)
			{
				ScreenResolution = new(Screen.width,Screen.height);

				if(UIMgr.HasInstance)
				{
					UIMgr.In.ChangeScreenSize(ScreenResolution);
				}
			}
#endif
		}

#if UNITY_EDITOR
		private static readonly List<string> s_sceneNameArray = new();

		protected static IEnumerable SceneNameArray
		{
			get
			{
				if(s_sceneNameArray.IsNullOrEmpty())
				{
					foreach(var scene in EditorBuildSettings.scenes)
					{
						var sceneName = Path.GetFileNameWithoutExtension(scene.path);

						if(sceneName == "MainScene")
						{
							continue;
						}

						s_sceneNameArray.Add(sceneName);
					}
				}

				return s_sceneNameArray;
			}
		}
#endif
	}
}