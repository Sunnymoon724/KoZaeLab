using UnityEngine;

/// <summary>
/// Calculates preferred TMP text height when content wraps at a maximum width.
/// </summary>
public class PreferredHeightTextUI : BaseTextMeshUI
{
	private const string c_lineSampleText = "가";

	[Space(10)]
	[SerializeField]
	private float m_singleLineHeight = 0.0f;	// Uses TMP measurement when 0. Layout baseline for one line.
	[SerializeField]
	private float m_maxContentWidth = 0.0f;	// Maximum width before text wraps.
	[SerializeField]
	private float m_minHeight = 0.0f;			// Lower bound for the calculated height.

	/// <summary>
	/// Returns the preferred height of <paramref name="text"/> wrapped at <see cref="m_maxContentWidth"/>.
	/// </summary>
	public float CalculateHeight(string text)
	{
		if(m_maxContentWidth <= 0.0f)
		{
			LogChannel.UI.W("max content width must be greater than 0.");

			return m_minHeight;
		}

		var resolvedLineHeight = _ResolveSingleLineHeight(out var measuredHeight);

		if(text.IsEmpty(includeSpace: true))
		{
			return Mathf.Max(m_minHeight,resolvedLineHeight);
		}

		var preferredSize = m_textMesh.GetPreferredValues(text,m_maxContentWidth,0.0f);

		// Replace one measured line height with the layout baseline in TMP's total preferred height.
		var height = preferredSize.y-measuredHeight+resolvedLineHeight;

		return Mathf.Max(m_minHeight,height);
	}

	private float _ResolveSingleLineHeight(out float measuredHeight)
	{
		measuredHeight = m_textMesh.GetPreferredValues(c_lineSampleText).y;

		return m_singleLineHeight > 0.0f ? m_singleLineHeight : measuredHeight;
	}
}