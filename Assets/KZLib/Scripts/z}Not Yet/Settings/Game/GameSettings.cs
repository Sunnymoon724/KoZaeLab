using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using KZLib.KZAttribute;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.AddressableAssets;

#endif

public class GameSettings : InnerBaseSettings<GameSettings>
{
	#region Path
	#region Setting
	[TitleGroup("Path",BoldTitle = false,Order = 1)]
	[BoxGroup("Path/Setting",ShowLabel = false,Order = 0),SerializeField,LabelText("Settings Window Path"),KZFolderPath]
	private string m_SettingPath = "Scripts/InEditor/SettingsWindow";
	public string SettingPath => m_SettingPath;
	#endregion Setting
	#endregion Path

	#region Hud
	[TitleGroup("Hud",BoldTitle = false,Order = 3)]
	[BoxGroup("Hud/Option",ShowLabel = false),SerializeField,LabelText("Use Hud")]
	private bool m_UseHeadUpDisplay = false;

	//? Live Mode -> Do Not Use Hud
	public bool UseHeadUpDisplay => m_UseHeadUpDisplay;

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