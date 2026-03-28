#if UNITY_EDITOR
using KZLib.Webs;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.Windows
{
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","Discord")]
		[HorizontalGroup("Network/Discord/0"),SerializeField]
		private string m_discordLink = null;

		private bool IsExistDiscord => !m_discordLink.IsEmpty();

		[HorizontalGroup("Network/Discord/1"),Button("Post Text",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
		protected void OnPostTextTest_Discord()
		{
			WebhookManager.In.PostDiscordWebHook(m_discordLink,"Text Test",_CreateTestMessageInfo(),null);
		}

		[HorizontalGroup("Network/Discord/1"),Button("Post Image",ButtonSizes.Large),EnableIf(nameof(IsExistDiscord))]
		protected void OnPostImageTest_Discord()
		{
			WebhookManager.In.PostDiscordWebHook(m_discordLink,"Image Test",_CreateTestMessageInfo(),KZEditorKit.FindTestImage());
		}
	}
}
#endif