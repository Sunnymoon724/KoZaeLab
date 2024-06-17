using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public partial class GameSettings : InnerBaseSettings<GameSettings>
{
	[TabGroup("게임 설정","프로젝트 설정",Order = -100)]
	[TitleGroup("게임 설정/프로젝트 설정/프리셋 설정",BoldTitle = false,Order = PRESET_ORDER)]
	[BoxGroup("게임 설정/프로젝트 설정/프리셋 설정/0",ShowLabel = false),SerializeField,LabelText("아이디 프리셋"),DictionaryDrawerSettings(KeyLabel = "디바이스 아이디",ValueLabel = "프리셋 이름")]
	private Dictionary<string,string> m_PresetNameDict = new();

	public string GetPresetOrDeviceID()
	{
		var deviceId = SystemInfo.deviceUniqueIdentifier;

		return m_PresetNameDict.TryGetValue(deviceId,out var preset) ? preset : deviceId;
	}
}