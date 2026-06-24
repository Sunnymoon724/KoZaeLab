using UnityEngine;

/// <summary>Sets TMP text from a format string and optional arguments via <see cref="string.Format"/>.</summary>
public class FormatTextUI : BaseTextMeshUI
{
	[SerializeField]
	private string m_formatText = null;

	public void SetFormatText(params object[] argumentArray)
	{
		if(!m_textMesh)
		{
			return;
		}

		m_textMesh.SetSafeTextMeshPro(argumentArray == null ? null : string.Format(m_formatText,argumentArray));
	}
}