using Sirenix.OdinInspector;
using UnityEngine;

namespace HudPanel
{
	public class FormatTextUI : BaseTextUI
	{
		[SerializeField,LabelText("포멧 텍스트")]
		private string m_FormatText = null;

		public void SetFormatText(params object[] _argumentArray)
		{
			m_Text.SetSafeTextMeshPro(_argumentArray == null ? null : string.Format(m_FormatText,_argumentArray));
		}
	}
}