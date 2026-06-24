using UnityEngine;
using TMPro;

/// <summary>Wrapper base for TextMeshPro <see cref="TMP_Text"/>.</summary>
[RequireComponent(typeof(TMP_Text))]
public abstract class BaseTextMeshUI : BaseComponent
{
	[SerializeField]
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
		if(!m_textMesh)
		{
			return;
		}

		m_textMesh.SetSafeTextMeshPro(string.Empty);
	}
}
