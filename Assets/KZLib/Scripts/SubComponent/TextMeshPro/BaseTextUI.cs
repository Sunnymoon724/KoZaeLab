using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

[RequireComponent(typeof(TMP_Text))]
public abstract class BaseTextUI : BaseComponentUI
{
	[SerializeField,LabelText("Text Mesh")]
	protected TMP_Text m_TextMesh = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_TextMesh)
		{
			m_TextMesh = GetComponent<TMP_Text>();
		}
	}

	public void ClearText()
	{
		m_TextMesh.SetSafeTextMeshPro(string.Empty);
	}
}