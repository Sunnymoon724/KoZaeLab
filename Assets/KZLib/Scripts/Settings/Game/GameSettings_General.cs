using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor.AddressableAssets;
using UnityEditor;

#endif

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	#region Game Version
	[SerializeField,HideInInspector]
	private string m_GameVersion = null;

	[TabGroup("게임 설정","프로젝트 설정",Order = -10)]
	[TitleGroup("게임 설정/프로젝트 설정/일반 설정",BoldTitle = false,Order = GENERAL_ORDER)]
	[BoxGroup("게임 설정/프로젝트 설정/일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("게임 버전")]
	public string GameVersion
	{
		get => m_GameVersion;
		set
		{
#if UNITY_EDITOR
			if(m_GameVersion == value)
			{
				return;
			}

			m_GameVersion = value;
			PlayerSettings.bundleVersion = value;
#endif
		}
	}
	#endregion Game Version

	#region Game Language
	[SerializeField,HideInInspector]
	private SystemLanguage m_GameLanguage = SystemLanguage.English;

	[BoxGroup("게임 설정/프로젝트 설정/일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("게임 언어"),InlineButton(nameof(OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.ArrowRepeat)]
	public SystemLanguage GameLanguage
	{
		get
		{
#if UNITY_EDITOR
			return m_GameLanguage;
#else
			return Application.systemLanguage;
#endif
		}
		private set
		{
			if(m_GameLanguage == value)
			{
				return;
			}

			m_GameLanguage = value;
		}
	}

	public SystemLanguage DefaultLanguage => SystemLanguage.English;

	private void OnChangeDefaultLanguage()
	{
		GameLanguage = DefaultLanguage;
	}
	#endregion Game Language

	#region Game Mode
	[SerializeField,HideInInspector]
	private string m_GameMode = DEVELOP_MODE;

	[BoxGroup("게임 설정/프로젝트 설정/일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("게임 모드"),ValueDropdown(nameof(GameModeList))]
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

			if(IsDevelopMode)
			{
				SetGameModeState(StateType.Local_State);
			}
			else if(IsLiveMode)
			{
				SetGameModeState(StateType.Server_State);
			}

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

	partial void SetGameMode_Partial();

	private enum StateType { Local_State, Server_State }

	[SerializeField,HideInInspector]
	private StateType m_ResourceState;

	[BoxGroup("게임 설정/프로젝트 설정/일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("리소스 상태"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
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

	[BoxGroup("게임 설정/프로젝트 설정/일반 설정/0",ShowLabel = false),ShowInInspector,LabelText("세이브 상태"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
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

	private ValueDropdownList<string> m_GameModeList = null;

	private IEnumerable GameModeList
	{
		get
		{
			if(m_GameModeList == null)
			{
				m_GameModeList = new()
				{
					{ "개발자 모드",DEVELOP_MODE },
					{ "커스텀 모드",CUSTOM_MODE },
					{ "라이브 모드",LIVE_MODE },
				};

				AddGameMode_Partial();
			}

			return m_GameModeList;
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
				{ "로컬 상태",StateType.Local_State },
				{ "서버 상태",StateType.Server_State },
			};
		}
	}

	partial void AddGameMode_Partial();

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

#if UNITY_EDITOR
	private void InitializeGeneral()
	{
		GameVersion = PlayerSettings.bundleVersion = "0.1";
		m_GameMode = DEVELOP_MODE;
		GameLanguage = DefaultLanguage;
	}
#endif
}