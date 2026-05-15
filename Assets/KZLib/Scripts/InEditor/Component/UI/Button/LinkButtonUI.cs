using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField]
	private string m_linkPath = null;

	private void Awake()
	{
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