using System;
using DG.Tweening;
using GameData;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

#if UNITY_EDITOR

using UnityEditor;

#endif

public partial class Main : SerializedMonoBehaviour
{
#if UNITY_EDITOR
	private const string MAIN_DATA = "[Main] MainData";

	private partial class MainData
	{
		public PlayType GamePlayType { get; set; } = PlayType.Normal;
	}
#endif

	private enum PlayType { Test, Normal, }

	private PlayType? m_GamePlayType = null;

	[ShowInInspector,LabelText("현재 플레이 타입")]
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

	private void Awake()
	{
		Log.System.I("메인 생성");

		Awake_Partial();
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

	partial void InitializeTestMode();
#endif

	partial void InitializeNormalMode();
	partial void Awake_Partial();
	partial void Release_Partial();
	partial void Update_Partial();
	
	private void Start()
	{
		if(!GameSettings.In.IsLiveMode && GameSettings.In.UseHeadUpDisplay)
		{
			UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
		}

		DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
		DOTween.SetTweensCapacity(1000,100);

		var option = GameDataMgr.In.Access<Option>().GraphicOption;
		var builder = new StringBuilder();

		InitializeResolution(builder,option);

#if UNITY_ANDROID || UNITY_IOS
		InitializeMobileFrame(builder,option);
#elif UNITY_STANDALONE
		InitializePcFrame(builder,option);
#endif
		InitializeRenderSetting(builder);

		Log.System.I(builder.ToString());

		MetaDataMgr.In.LoadAll();

		if(IsTestMode)
		{
#if UNITY_EDITOR
			InitializeTestMode();
#else
			throw new Exception("에디터가 아니면 테스트 모드를 할 수 없습니다.");
#endif
		}
		else
		{
			InitializeNormalMode();
		}
	}

	private void InitializeMobileFrame(StringBuilder _builder,Option.Graphic _option)
	{
		_option.FrameRate = Global.FRAME_RATE_30;

		// //? vSyncCount = 0
		_option.GraphicQuality &= ~GraphicQualityOption.VERTICAL_SYNC_ENABLE;
		_builder.AppendFormat(string.Format("현재 FPS {0}\n",_option.FrameRate));
	}

	private void InitializePcFrame(StringBuilder _builder,Option.Graphic _option)
	{
		_option.FrameRate = Global.FRAME_RATE_60;

		// //? vSyncCount = 1
		_option.GraphicQuality |= GraphicQualityOption.VERTICAL_SYNC_ENABLE;
		_builder.AppendFormat(string.Format("현재 FPS {0}\n",_option.FrameRate));
	}

	private void InitializeResolution(StringBuilder _builder,Option.Graphic _option)
	{
		//? 모바일에서 화면잠김을 방지하기 위한 값.
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		var resolution = _option.ScreenResolution;

		Screen.SetResolution(resolution.Width,resolution.Height,resolution.IsFull);

		_builder.AppendFormat(string.Format("현재 해상도 {0}x{1}\n",resolution.Width,resolution.Width));
	}

	private void InitializeRenderSetting(StringBuilder _builder) { }

	private void OnApplicationQuit()
	{
		CommonUtility.ReleaseManager();

		Release_Partial();
	}

	private void Update()
	{
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

		//? 강제 버그 만들기
		if(Input.GetKeyDown(KeyCode.F4))
		{
			throw new Exception("강제 에러 생성!!");
		}
#endif

		Update_Partial();
	}
}