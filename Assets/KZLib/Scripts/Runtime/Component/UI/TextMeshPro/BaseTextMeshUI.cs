using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public abstract class BaseTextMeshUI : MonoBehaviour
{
	[SerializeField]
	protected TMP_Text m_textMesh = null;

	private void Reset()
	{
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