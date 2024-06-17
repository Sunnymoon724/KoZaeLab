using UnityEngine;

/// <summary>
/// 설정한 포맷으로 텍스트가 나온다
/// </summary>
public class FormatTextUI : BaseTextUI
{
	[SerializeField]
	private string m_Format = null;

	public void SetText(params object[] _argumentArray)
	{
		m_Text.SetSafeTextMeshPro(string.Format(m_Format,_argumentArray));
	}
}