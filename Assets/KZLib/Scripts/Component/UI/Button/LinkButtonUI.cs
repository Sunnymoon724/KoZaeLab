using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField]
	private string m_linkPath = null;

	protected override void _Initialize()
	{
		if(m_linkPath.IsEmpty())
		{
			LogChannel.System.I("Link is empty");

			return;
		}

		base._Initialize();
	}

	protected override void _OnClickedButton()
	{
		Application.OpenURL(m_linkPath);
	}
}