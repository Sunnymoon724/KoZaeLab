using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using KZLib.KZAttribute;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public abstract class KZMainMgr<TBehaviour> : SingletonMB<TBehaviour> where TBehaviour : SerializedMonoBehaviour
	{
		[SerializeField,HideInInspector]
		private string m_GameVersion = null;

		[ShowInInspector,LabelText("Game Version"),KZRichText]
		public string GameVersion { get => m_GameVersion; private set => m_GameVersion = value; }

		[SerializeField,HideInInspector]
		private SystemLanguage? m_GameLanguage = null;

		[ShowInInspector,LabelText("Game Language"),InlineButton(nameof(OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.Reply)]
		public SystemLanguage GameLanguage
		{
			get
			{
				if(!m_GameLanguage.HasValue)
				{
					var option = GameDataMgr.In.Access<GameData.LanguageOption>();

					m_GameLanguage = option.GameLanguage;
				}

				return m_GameLanguage.Value;
			}

			set
			{
				if(m_GameLanguage == value)
				{
					return;
				}

				m_GameLanguage = value;

				var option = GameDataMgr.In.Access<GameData.LanguageOption>();

				option.GameLanguage = m_GameLanguage.Value;
			}
		}

		private void OnChangeDefaultLanguage()
		{
			GameLanguage = SystemLanguage.English;
		}

		[SerializeField,HideInInspector]
		private bool m_Encrypt = true;

		[ShowInInspector,LabelText("Is Encrypt"),KZRichText]
		public bool Encrypt { get => m_Encrypt; private set => m_Encrypt = value; }

		[SerializeField,HideInInspector]
		private bool m_Playing = false;

		[ShowInInspector,LabelText("Is Playing"),KZRichText]
		public bool IsPlaying => m_Playing;

		[SerializeField,HideInInspector]
		private int m_SafeWidth = 0;

		[SerializeField,HideInInspector]
		private Vector2Int m_ScreenResolution = Vector2Int.zero;

		[ShowInInspector,LabelText("Screen Resolution"),KZRichText("width : {0}, height : {1}")]
		public Vector2Int ScreenResolution { get => m_ScreenResolution; private set => m_ScreenResolution = value; }

#if UNITY_EDITOR
		private const string MAIN_DATA = "[Main] MainData";

		private class MainData
		{
			public PlayType GamePlayType { get; set; } = PlayType.Normal;
		}
#endif

		private enum PlayType { Test, Normal, }

		private PlayType? m_GamePlayType = null;

		[ShowInInspector,LabelText("Current PlayType")]
		private PlayType GamePlayType
		{
			get
			{
				if(!m_GamePlayType.HasValue)
				{
#if UNITY_EDITOR
					var data = LoadData();

					m_GamePlayType = data.GamePlayType;
#else
					m_GamePlayType = PlayType.Normal;
#endif
				}

				return m_GamePlayType.Value;
			}
			set
			{
#if UNITY_EDITOR
				if(m_GamePlayType == value)
				{
					return;
				}

				var data = LoadData();

				m_GamePlayType = data.GamePlayType = value;

				SaveData(data);
#endif
			}
		}

		private bool IsTestMode => GamePlayType == PlayType.Test;

		protected override void Initialize()
		{
			base.Initialize();

			m_Playing = true;

			var builder = new StringBuilder();

			builder.AppendLine("Initialize Main");

			GameVersion = SetGameVersion();

			builder.AppendLine($"Current Version {GameVersion}");

			//? 에디터가 아니면 암호화 & Normal Play
#if !UNITY_EDITOR
			Encrypt = true;
#endif
			builder.AppendLine($"Current PlayType {GamePlayType}");

			LogTag.System.I(builder.ToString());
		}

		protected virtual string SetGameVersion()
		{
			return "0.1";
		}

		private async void Start()
		{
			if(!GameSettings.In.IsLiveMode && GameSettings.In.UseHeadUpDisplay)
			{
				UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
			}

			if(m_SafeWidth <= 0)
			{
				m_SafeWidth = (int) Screen.safeArea.width;
			}

			DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
			DOTween.SetTweensCapacity(1000,100);

			var builder = new StringBuilder();

			InitializeResolution(builder);
			InitializeFrame(builder);
			InitializeRenderSetting(builder);
			InitializeObject(builder);

			LogTag.System.I(builder.ToString());

			// TODO 메타 데이터 로드 위치 변경하기 (선택창으로 시작할때 로드 or 원할때 로드)
			await MetaDataMgr.In.LoadAllAsync();

			if(IsTestMode)
			{
#if UNITY_EDITOR
				InitializeTestMode();
#else
				throw new Exception("This cannot be tested outside of the editor mode.");
#endif
			}
			else
			{
				InitializeNormalMode();
			}
		}

#if UNITY_EDITOR
		private void SaveData(MainData _mainData)
		{
			EditorPrefs.SetString(MAIN_DATA,JsonConvert.SerializeObject(_mainData));
		}

		private MainData LoadData()
		{
			var text = EditorPrefs.GetString(MAIN_DATA,"");

			return text.IsEmpty() ? new MainData() : JsonConvert.DeserializeObject<MainData>(text);
		}

		protected virtual void InitializeTestMode() { }
#endif
		protected virtual void InitializeNormalMode() { }

		protected virtual void InitializeResolution(StringBuilder _builder)
		{
			//? 모바일에서 화면잠김을 방지하기 위한 값.
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR || UNITY_STANDALONE
			ScreenResolution = new Vector2Int(Screen.width,Screen.height);
#else
			ScreenResolution = new Vector2Int(Screen.currentResolution.width,Screen.currentResolution.height);
#endif

			_builder.AppendFormat($"Current Screen Resolution {ScreenResolution.x}x{ScreenResolution.y}\n");
		}

		protected virtual void InitializeFrame(StringBuilder _builder)
		{
#if UNITY_ANDROID || UNITY_IOS
			QualitySettings.vSyncCount	= 0;
			Application.targetFrameRate = Global.FRAME_RATE_30;
#elif UNITY_STANDALONE
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = Global.FRAME_RATE_60;
#endif

			_builder.AppendFormat($"Current FPS {Application.targetFrameRate}\n");
		}

		protected virtual void InitializeRenderSetting(StringBuilder _builder)
		{

		}

		protected virtual void InitializeObject(StringBuilder _builder)
		{

		}

		protected override void Release()
		{
			base.Release();

			m_Playing = false;

			CommonUtility.ReleaseManager();
		}

		protected virtual void Update()
		{
			CheckResolution();

#if UNITY_EDITOR
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C) && GameSettings.In.UseHeadUpDisplay)
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
	}
}