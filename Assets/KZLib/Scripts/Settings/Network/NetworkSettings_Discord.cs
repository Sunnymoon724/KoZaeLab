using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

//! 디스코드는 로그 리포트 용도
public partial class NetworkSettings : InSideSingletonSO<NetworkSettings>
{
	[TabGroup("통신 설정","디스코드")]
	[TitleGroup("통신 설정/디스코드/사용 설정",BoldTitle = false,Order = 0),SerializeField,LabelText("디스코드 사용"),ToggleLeft]
	private bool m_UseDiscord = false;

	[TitleGroup("통신 설정/디스코드/기본 설정",BoldTitle = false,Order = 1)]
	[VerticalGroup("통신 설정/디스코드/기본 설정/0"),SerializeField,LabelText("디스코드 링크들"),DictionaryDrawerSettings(KeyLabel = "키 값",ValueLabel = "링크"),EnableIf(nameof(m_UseDiscord))]
	private Dictionary<string,string> m_DiscordLinkDict = new();

	private bool IsExistDiscord => m_UseDiscord && m_DiscordLinkDict.Count > 0;

	public string GetDiscordLink(string _key)
	{
		return IsExistDiscord ? m_DiscordLinkDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : null;
	}

#if UNITY_EDITOR
#pragma warning disable IDE0051
	[TitleGroup("통신 설정/디스코드/테스트",BoldTitle = false,Order = 2)]
	[HorizontalGroup("통신 설정/디스코드/테스트/0"),Button("텍스트 보내기",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
	private void OnPostTextTest_Discord()
	{
		CommonUtility.SendReportOnlyDiscord("Text Test",new MessageData[] { new("Test","Hello World") },null);
	}

	[HorizontalGroup("통신 설정/디스코드/테스트/0"),Button("이미지 보내기",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
	private void OnPostPhotoTest_Discord()
	{
		CommonUtility.SendReportOnlyDiscord("Photo Test",new MessageData[] { new("Test","Hello World") },CommonUtility.GetTestImageData());
	}
#pragma warning restore IDE0051
#endif
}