using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[TabGroup("Network","Discord")]
	[TitleGroup("Network/Discord/General",BoldTitle = false,Order = 0)]
	[VerticalGroup("Network/Discord/General/0",Order = 0),SerializeField,LabelText("Use Discord"),ToggleLeft]
	private bool m_UseDiscord = false;

	[VerticalGroup("Network/Discord/General/1",Order = 1),SerializeField,LabelText("Discord Link Dict"),DictionaryDrawerSettings(KeyLabel = "Name",ValueLabel = "Link"),ShowIf(nameof(m_UseDiscord))]
	private Dictionary<string,string> m_DiscordLinkDict = new();

	private bool IsExistDiscord => m_UseDiscord && m_DiscordLinkDict.Count > 0;

	public string GetDiscordLink(string _key)
	{
		return IsExistDiscord ? m_DiscordLinkDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : null;
	}

#if UNITY_EDITOR
	[TitleGroup("Network/Discord/Test",BoldTitle = false,Order = 2)]
	[HorizontalGroup("Network/Discord/Test/0"),Button("Post Text",ButtonSizes.Large),ShowIf(nameof(m_UseDiscord)),EnableIf(nameof(IsExistDiscord))]
	protected void OnPostTextTest_Discord()
	{
		WebRequestUtility.PostWebHook_Discord("Text Test",new MessageData[] { new("Test","Hello World") },null);
	}

	[HorizontalGroup("Network/Discord/Test/0"),Button("Post Image",ButtonSizes.Large),ShowIf(nameof(m_UseDiscord)),EnableIf(nameof(IsExistDiscord))]
	protected void OnPostImageTest_Discord()
	{
		WebRequestUtility.PostWebHook_Discord("Image Test",new MessageData[] { new("Test","Hello World") },CommonUtility.GetTestImageData());
	}
#endif
}