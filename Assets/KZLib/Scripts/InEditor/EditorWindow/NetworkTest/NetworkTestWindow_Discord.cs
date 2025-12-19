#if UNITY_EDITOR
using KZLib.KZNetwork;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class NetworkTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","Discord")]
		[HorizontalGroup("Network/Discord/0"),SerializeField]
		private string m_discordLink = null;

		private bool IsExistDiscord => !m_discordLink.IsEmpty();

		[HorizontalGroup("Network/Discord/1"),Button("Post Text",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
		protected void OnPostTextTest_Discord()
		{
			WebRequestManager.In.PostDiscordWebHook(m_discordLink,"Text Test",new MessageInfo[] { new("Test","Hello World") },null);
		}

		[HorizontalGroup("Network/Discord/1"),Button("Post Image",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
		protected void OnPostImageTest_Discord()
		{
			WebRequestManager.In.PostDiscordWebHook(m_discordLink,"Image Test",new MessageInfo[] { new("Test","Hello World") },CommonUtility.FindTestImage());
		}
	}
}
#endif