using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField]
	private string m_linkPath = null;

	protected override void Initialize()
	{
		if(m_linkPath.IsEmpty())
		{
			LogSvc.System.I("Link is empty");

			return;
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		Application.OpenURL(m_linkPath);
	}
}