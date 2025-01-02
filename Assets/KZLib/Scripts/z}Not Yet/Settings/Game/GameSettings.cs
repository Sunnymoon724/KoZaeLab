using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using KZLib.KZAttribute;
using System.Collections.Generic;
using KZLib.KZUtility;

#if UNITY_EDITOR

using UnityEditor.AddressableAssets;

#endif

public class GameSettings : InnerBaseSettings<GameSettings>
{
	#region General
	#region Game Mode
	[SerializeField,HideInInspector]
	private string m_GameMode = DEVELOP_MODE;

	[TitleGroup("General",BoldTitle = false,Order = 0)]
	[BoxGroup("General/GameMode",ShowLabel = false,Order = 0),ShowInInspector,LabelText("Game Mode"),ValueDropdown(nameof(GameModeList))]
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

			//? Change AddressableAssetSetting
			setting.ActivePlayModeDataBuilderIndex = IsServerResource ? 2 : 0;
#endif
		}
	}

	private enum StateType { Local_State, Server_State }

	[SerializeField,HideInInspector]
	private StateType m_ResourceState = StateType.Local_State;

	[BoxGroup("General/GameMode",ShowLabel = false,Order = 0),ShowInInspector,LabelText("Resource State"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
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
	private StateType m_SaveState;

	[BoxGroup("General/GameMode",ShowLabel = false,Order = 0),ShowInInspector,LabelText("Save State"),ValueDropdown(nameof(StateList)),EnableIf(nameof(LockState))]
	private StateType SaveState
	{
		get => m_SaveState;
		set
		{
			if(m_SaveState == value)
			{
				return;
			}

			m_SaveState = value;
		}
	}

	private IEnumerable GameModeList
	{
		get
		{
			return new ValueDropdownList<string>()
			{
				{ "Develop Mode",	DEVELOP_MODE	},
				{ "Custom Mode",	CUSTOM_MODE		},
				{ "Live Mode",		LIVE_MODE		},
			};
		}
	}

	private void SetGameModeState(StateType _type)
	{
		ResourceState = _type;
		SaveState = _type;
	}

	private bool LockState = false;

	private IEnumerable StateList
	{
		get
		{
			return new ValueDropdownList<StateType>()
			{
				{ "Local State",	StateType.Local_State	},
				{ "Server State",	StateType.Server_State	},
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
	public bool IsLocalSave => SaveState == StateType.Local_State;
	public bool IsServerResource => !IsLocalResource;
	public bool IsServerData => !IsLocalSave;
	#endregion Game Mode

	#region GameVersion
	[SerializeField,HideInInspector]
	private string m_GameVersion = "0.1";

	[BoxGroup("General/GameVersion",ShowLabel = false,Order = 1),ShowInInspector,LabelText("Game Version")]
	public string GameVersion
	{
		get => m_GameVersion;
		private set
		{
			if(m_GameVersion == value)
			{
				return;
			}

			m_GameVersion = value;
		}
	}
	#endregion GameVersion

	#region Game Language
	[SerializeField,HideInInspector]
	private SystemLanguage m_GameLanguage = SystemLanguage.English;

	[BoxGroup("General/GameLanguage",ShowLabel = false,Order = 2),ShowInInspector,LabelText("Game Language"),InlineButton(nameof(OnChangeDefaultLanguage),Label = "",Icon = SdfIconType.Reply)]
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

	private void OnChangeDefaultLanguage()
	{
		GameLanguage = SystemLanguage.English;
	}
	#endregion Game Language

	#region Game Graphic
	[SerializeField,HideInInspector]
	private Vector2Int m_ScreenResolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT);

	[BoxGroup("General/GameGraphic",ShowLabel = false,Order = 3),ShowInInspector,LabelText("Screen Resolution")]
	public Vector2Int ScreenResolution { get => m_ScreenResolution; private set => m_ScreenResolution = value; }

	public float ScreenAspect => ScreenResolution.x / (float) ScreenResolution.y;

	[SerializeField,HideInInspector]
	private int m_FrameRate = Global.FRAME_RATE_60;

	[BoxGroup("General/GameGraphic",ShowLabel = false,Order = 3),ShowInInspector,LabelText("Frame Rate")]
	public int FrameRate { get => m_FrameRate; private set => m_FrameRate = value; }

	[SerializeField,HideInInspector]
	private GraphicsQualityPresetType m_GraphicQualityPreset = GraphicsQualityPresetType.QualityHighest;

	[BoxGroup("General/GameGraphic",ShowLabel = false,Order = 3),ShowInInspector,LabelText("Graphic Quality Preset")]
	public GraphicsQualityPresetType GraphicQualityPreset { get => m_GraphicQualityPreset; private set => m_GraphicQualityPreset = value; }
	#endregion Game Graphic
	#endregion General

	#region Path
	#region Setting
	[TitleGroup("Path",BoldTitle = false,Order = 1)]
	[BoxGroup("Path/Setting",ShowLabel = false,Order = 0),SerializeField,LabelText("Settings Window Path"),KZFolderPath]
	private string m_SettingPath = "Scripts/InEditor/SettingsWindow";
	public string SettingPath => m_SettingPath;
	#endregion Setting

	#region Sheet
	[BoxGroup("Path/Sheet",ShowLabel = false,Order = 1),SerializeField,LabelText("MetaData Script Path"),KZFolderPath]
	private string m_MetaDataScriptPath = "Scripts/Data/MetaData";
	public string MetaDataScriptPath => m_MetaDataScriptPath;

	[BoxGroup("Path/Sheet",ShowLabel = false,Order = 1),SerializeField,LabelText("MetaData File Path"),KZFolderPath]
	private string m_MetaDataFilePath = "Resources/Texts/MetaData";
	public string MetaDataFilePath => m_MetaDataFilePath;

	[BoxGroup("Path/Sheet",ShowLabel = false,Order = 1),SerializeField,LabelText("Language File Path"),KZFolderPath]
	private string m_LanguageFilePath = "Resources/Texts/Languages";
	public string LanguageFilePath => m_LanguageFilePath;

	[BoxGroup("Path/Sheet",ShowLabel = false,Order = 1),SerializeField,LabelText("ConfigData File Path"),KZFolderPath]
	private string m_ConfigDataFilePath = "Resources/Texts/ConfigData";
	public string ConfigDataFilePath => m_ConfigDataFilePath;

	[BoxGroup("Path/Sheet",ShowLabel = false,Order = 1),SerializeField,LabelText("Custom ConfigData File Path"),KZFolderPath(true,false)]
	private string m_CustomConfigDataFilePath = "Documents/Custom/ConfigData";
	public string CustomConfigDataFilePath => m_CustomConfigDataFilePath;
	#endregion Sheet

	#region Resource
	[BoxGroup("Path/Resource",ShowLabel = false,Order = 2),SerializeField,LabelText("UI Prefab Path"),KZFolderPath]
	private string m_UIPrefabPath = "Resources/Prefabs/UI";
	public string UIPrefabPath => m_UIPrefabPath;
	
	[BoxGroup("Path/Resource",ShowLabel = false,Order = 2),SerializeField,LabelText("FX Prefab Path"),KZFolderPath]
	private string m_FXPrefabPath = "Resources/Prefabs/FX";
	public string FXPrefabPath => m_FXPrefabPath;
	#endregion Resource
	#endregion Path

	#region Hud
	[TitleGroup("Hud",BoldTitle = false,Order = 3)]
	[BoxGroup("Hud/Option",ShowLabel = false),SerializeField,LabelText("Use Hud")]
	private bool m_UseHeadUpDisplay = false;

	//? Live Mode -> Do Not Use Hud
	public bool UseHeadUpDisplay => !IsLiveMode && m_UseHeadUpDisplay;

	public bool UseProfileSetting => UseFrameSetting || UseMemorySetting || UseAudioSetting;

	#region Detail
	[BoxGroup("Hud/Detail")]
	[HorizontalGroup("Hud/Detail/Profile",Order = 0),SerializeField,LabelText("Use Frame"),ToggleLeft,ShowIf(nameof(UseHeadUpDisplay))]
	private bool m_UseFrameSetting = true;
	public bool UseFrameSetting => m_UseFrameSetting;

	[HorizontalGroup("Hud/Detail/Profile",Order = 0),SerializeField,LabelText("Use Memory"),ToggleLeft,ShowIf(nameof(UseHeadUpDisplay))]
	private bool m_UseMemorySetting = true;
	public bool UseMemorySetting => m_UseMemorySetting;

	[HorizontalGroup("Hud/Detail/Profile",Order = 0),SerializeField,LabelText("Use Audio"),ToggleLeft,ShowIf(nameof(UseHeadUpDisplay))]
	private bool m_UseAudioSetting = true;
	public bool UseAudioSetting => m_UseAudioSetting;

	[PropertySpace(5)]
	[HorizontalGroup("Hud/Detail/Log",Order = 1),SerializeField,LabelText("Use Log"),ToggleLeft,ShowIf(nameof(UseHeadUpDisplay))]
	private bool m_UseLogSetting = true;
	public bool UseLogSetting => m_UseLogSetting;
	#endregion Detail
	#endregion Hud

	#region Preset
	[TitleGroup("Preset",BoldTitle = false,Order = 4)]
	[BoxGroup("Preset/0",ShowLabel = false),SerializeField,LabelText("Preset Dict"),DictionaryDrawerSettings(KeyLabel = "Device Id",ValueLabel = "Preset Name")]
	private Dictionary<string,string> m_PresetNameDict = new();

	public string PresetNameOrDeviceId
	{
		get
		{
			var deviceId = SystemInfo.deviceUniqueIdentifier;

			return m_PresetNameDict.TryGetValue(deviceId,out var preset) ? preset : deviceId;
		}
	}
	#endregion Preset
}