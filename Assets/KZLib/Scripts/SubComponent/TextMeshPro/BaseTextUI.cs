using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

[RequireComponent(typeof(TMP_Text))]
public abstract class BaseTextUI : BaseComponentUI
{
	[SerializeField,LabelText("Text Mesh")]
	protected TMP_Text m_textMesh = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_textMesh)
		{
			m_textMesh = GetComponent<TMP_Text>();
		}
	}

	public void ClearText()
	{
		m_textMesh.SetSafeTextMeshPro(string.Empty);
	}
}