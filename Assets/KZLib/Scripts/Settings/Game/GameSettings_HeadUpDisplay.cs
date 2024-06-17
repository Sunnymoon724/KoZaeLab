using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameSettings : InSideSingletonSO<GameSettings>
{
	[TabGroup("게임 설정","프로젝트 설정",Order = -10)]
	[TitleGroup("게임 설정/프로젝트 설정/HUD 설정",BoldTitle = false,Order = HUD_ORDER)]
	[BoxGroup("게임 설정/프로젝트 설정/HUD 설정/0",ShowLabel = false),SerializeField,LabelText("HUD 사용")]
	private bool m_UseHeadUpDisplay = false;

	//? 라이브 모드에서는 HUD를 사용하지 않는다.
	public bool UseHeadUpDisplay => !IsLiveMode && m_UseHeadUpDisplay;

	public bool UseProfileSetting => UseFrameSetting || UseMemorySetting || UseAudioSetting;

	[BoxGroup("게임 설정/프로젝트 설정/HUD 설정/0/세부 설정")]
	[HorizontalGroup("게임 설정/프로젝트 설정/HUD 설정/0/세부 설정/프로파일",Order = 0),SerializeField,LabelText("프레임 설정"),ToggleLeft,EnableIf("UseHeadUpDisplay")]
	private bool m_UseFrameSetting = true;
	public bool UseFrameSetting => m_UseFrameSetting;

	[HorizontalGroup("게임 설정/프로젝트 설정/HUD 설정/0/세부 설정/프로파일",Order = 0),SerializeField,LabelText("메모리 설정"),ToggleLeft,EnableIf("UseHeadUpDisplay")]
	private bool m_UseMemorySetting = true;
	public bool UseMemorySetting => m_UseMemorySetting;

	[HorizontalGroup("게임 설정/프로젝트 설정/HUD 설정/0/세부 설정/프로파일",Order = 0),SerializeField,LabelText("오디오 설정"),ToggleLeft,EnableIf("UseHeadUpDisplay")]
	private bool m_UseAudioSetting = true;
	public bool UseAudioSetting => m_UseAudioSetting;

	[PropertySpace(5)]
	[HorizontalGroup("게임 설정/프로젝트 설정/HUD 설정/0/세부 설정/로그",Order = 1),SerializeField,LabelText("로그 설정"),ToggleLeft,EnableIf("UseHeadUpDisplay")]
	private bool m_UseLogSetting = true;
	public bool UseLogSetting => m_UseLogSetting;
}