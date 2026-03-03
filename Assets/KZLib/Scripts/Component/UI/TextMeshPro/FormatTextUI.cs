using UnityEngine;

public class FormatTextUI : BaseTextMeshUI
{
	[SerializeField]
	private string m_formatText = null;

	public void SetFormatText(params object[] argumentArray)
	{
		m_textMesh.SetSafeTextMeshPro(argumentArray == null ? null : string.Format(m_formatText,argumentArray));
	}
}