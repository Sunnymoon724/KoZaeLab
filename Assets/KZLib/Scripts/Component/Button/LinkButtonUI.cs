using Sirenix.OdinInspector;
using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("URL Link")]
	private string m_linkPath = null;

	protected override void Initialize()
	{
		if(m_linkPath.IsEmpty())
		{
			LogTag.System.I("Link is empty");

			return;
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		Application.OpenURL(m_linkPath);
	}
}