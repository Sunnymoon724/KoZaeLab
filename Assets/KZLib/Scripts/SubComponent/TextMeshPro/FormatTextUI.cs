using Sirenix.OdinInspector;
using UnityEngine;

namespace HudPanel
{
	public class FormatTextUI : BaseTextUI
	{
		[SerializeField,LabelText("Format Text")]
		private string m_FormatText = null;

		public void SetFormatText(params object[] _argumentArray)
		{
			m_TextMesh.SetSafeTextMeshPro(_argumentArray == null ? null : string.Format(m_FormatText,_argumentArray));
		}
	}
}