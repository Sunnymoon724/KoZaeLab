using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class PlayModeUI : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_PlayModeText = null;

		private string m_CurrentMode = null;

		protected override void Initialize()
		{
			base.Initialize();

			if(m_CurrentMode.IsEmpty())
			{
				m_CurrentMode = GameSettings.In.GameMode;
			}

			m_PlayModeText.SetSafeTextMeshPro(m_CurrentMode);
		}
	}
}