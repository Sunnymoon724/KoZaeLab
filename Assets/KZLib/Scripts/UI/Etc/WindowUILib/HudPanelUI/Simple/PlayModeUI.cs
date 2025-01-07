using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class PlayModeUI : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_playModeText = null;

		private string m_currentMode = null;

		protected override void Initialize()
		{
			base.Initialize();

			if(m_currentMode.IsEmpty())
			{
				m_currentMode = GameSettings.In.GameMode;
			}

			m_playModeText.SetSafeTextMeshPro(m_currentMode);
		}
	}
}