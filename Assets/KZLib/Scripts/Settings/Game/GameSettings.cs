using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using KZLib.KZAttribute;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.AddressableAssets;

#endif

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
#if UNITY_EDITOR
	protected override void Initialize()
	{
		base.Initialize();

		GameMode = DEVELOP_MODE;

		ScreenResolution = new Vector2Int(Global.BASE_WIDTH,Global.BASE_HEIGHT);
		FullScreen = true;
		GraphicQualityPreset = GraphicsQualityPresetType.QualityHighest;
		FrameRate = Global.FRAME_RATE_60;

		Screen.SetResolution(m_ScreenResolution.x,m_ScreenResolution.y,FullScreen);
		Application.targetFrameRate = m_FrameRate;
	}

#endif

	#region General

	#region Game Mode
	[SerializeField,HideInInspector]
	private string m_GameMode = DEVELOP_MODE;

	[BoxGroup("일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("게임 모드"),ValueDropdown(nameof(GameModeList))]
	public string GameMode
	{
		get => m_GameMode;
		private set
		{
			if(m_GameMode == value)
			{
				return;
			}

			m_GameMode = value;

			LockState = IsCustomMode;

			SetGameModeState(IsDevelopMode ? StateType.Local_State : StateType.Server_State);

#if UNITY_EDITOR
			var setting = AddressableAssetSettingsDefaultObject.Settings;

			if(!setting)
			{
				return;
			}

			if(BuildSettings.HasInstance)
			{
				BuildSettings.In.InitializeBundleSetting(setting);
			}

			//? 어드레서블 그룹에서 PlayModeScript 설정 변경
			setting.ActivePlayModeDataBuilderIndex = IsServerResource ? 2 : 0;
#endif
		}
	}

	private enum StateType { Local_State, Server_State }

	[SerializeField,HideInInspector]
	private StateType m_ResourceState;

	[BoxGroup("일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("리소스 상태"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
	private StateType ResourceState
	{
		get => m_ResourceState;
		set
		{
			if(m_ResourceState == value)
			{
				return;
			}

			m_ResourceState = value;
		}
	}

	[SerializeField,HideInInspector]
	private StateType m_SaveDataState;

	[BoxGroup("일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("세이브 상태"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
	private StateType SaveDataState
	{
		get => m_SaveDataState;
		set
		{
			if(m_SaveDataState == value)
			{
				return;
			}

			m_SaveDataState = value;
		}
	}

	private static IEnumerable GameModeList
	{
		get
		{
			return new ValueDropdownList<string>()
			{
				{ "개발자 모드", DEVELOP_MODE	},
				{ "커스텀 모드", CUSTOM_MODE	},
				{ "라이브 모드", LIVE_MODE		},
			};
		}
	}

	private void SetGameModeState(StateType _type)
	{
		ResourceState = _type;
		SaveDataState = _type;
	}

	private bool LockState = false;

	private static IEnumerable StateList
	{
		get
		{
			return new ValueDropdownList<StateType>()
			{
				{ "로컬 상태", StateType.Local_State	},
				{ "서버 상태", StateType.Server_State	},
			};
		}
	}

	private const string DEVELOP_MODE = "DevelopMode";
	private const string CUSTOM_MODE = "CustomMode";
	private const string LIVE_MODE = "LiveMode";

	public bool IsDevelopMode => GameMode.IsEqual(DEVELOP_MODE);
	public bool IsCustomMode => GameMode.IsEqual(CUSTOM_MODE);
	public bool IsLiveMode => GameMode.IsEqual(LIVE_MODE);

	public bool IsLocalResource => ResourceState == StateType.Local_State;
	public bool IsLocalSaveData => SaveDataState == StateType.Local_State;
	public bool IsServerResource => !IsLocalResource;
	public bool IsServerData => !IsLocalSaveData;
	#endregion Game Mode
	#endregion General

	#region Path
	[TitleGroup("경로 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("설정 창 경로"),KZFolderPath]
	private string m_SettingPath = "Scripts/InEditor/SettingsWindow";
	public string SettingPath => m_SettingPath;

	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("메타 데이터 스크립트 경로"),KZFolderPath]
	private string m_MetaScriptPath = "Scripts/Data/MetaData";
	public string MetaScriptPath => m_MetaScriptPath;

	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("메타 데이터 에셋 경로"),KZFolderPath]
	private string m_MetaAssetPath = "Resources/ScriptableObjects/MetaData";
	public string MetaAssetPath => m_MetaAssetPath;

	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("언어 데이터 에셋 경로"),KZFolderPath]
	private string m_LanguagePath = "Resources/Texts/Languages";
	public string LanguagePath => m_LanguagePath;

	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("UI 경로"),KZFolderPath]
	private string m_UIPrefabPath = "Resources/Prefabs/UI";
	public string UIPrefabPath => m_UIPrefabPath;
	
	[BoxGroup("경로 설정/0",ShowLabel = false),SerializeField,LabelText("이펙트 경로"),KZFolderPath]
	private string m_EffectPrefabPath = "Resources/Prefabs/Efx";
	public string EffectPrefabPath => m_EffectPrefabPath;
	#endregion Path

	#region Graphic
	[SerializeField,HideInInspector]
	private Vector2Int m_ScreenResolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT);

	[TitleGroup("그래픽 설정",BoldTitle = false,Order = 2)]
	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 해상도")]
	public Vector2Int ScreenResolution { get => m_ScreenResolution; private set => m_ScreenResolution = value; }

	[SerializeField,HideInInspector]
	private bool m_IsFullScreen = true;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("스크린 최대화")]
	public bool FullScreen { get => m_IsFullScreen; private set => m_IsFullScreen = value; }

	public float ScreenAspect => ScreenResolution.x / (float) ScreenResolution.y;

	[SerializeField,HideInInspector]
	private int m_FrameRate = Global.FRAME_RATE_60;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("프레임 레이트")]
	public int FrameRate
	{
		get => m_FrameRate;
		private set
		{
			m_FrameRate = value;
		}
	}

	[SerializeField,HideInInspector]
	private GraphicsQualityPresetType m_GraphicQualityPreset = GraphicsQualityPresetType.QualityHighest;

	[BoxGroup("그래픽 설정/0",ShowLabel = false),ShowInInspector,LabelText("그래픽 퀄리티 프리셋")]
	public GraphicsQualityPresetType GraphicQualityPreset { get => m_GraphicQualityPreset; private set => m_GraphicQualityPreset = value; }
	#endregion Graphic
}