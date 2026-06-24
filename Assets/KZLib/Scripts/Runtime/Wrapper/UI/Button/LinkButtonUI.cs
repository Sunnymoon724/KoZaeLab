using UnityEngine;

/// <summary>Button that opens <see cref="m_linkPath"/> via <see cref="Application.OpenURL"/> on click.</summary>
public class LinkButtonUI : BaseButtonUI
{
	[SerializeField]
	private string m_linkPath = null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(m_linkPath.IsEmpty())
		{
			LogChannel.UI.W("Link is empty");

			return;
		}
	}

	protected override void _OnClickedButton()
	{
		Application.OpenURL(m_linkPath);
	}
}