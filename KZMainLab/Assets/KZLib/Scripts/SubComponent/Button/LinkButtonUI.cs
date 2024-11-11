using Sirenix.OdinInspector;
using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("Link URL")]
	private string m_LinkURL = null;

	protected override void Initialize()
	{
		if(m_LinkURL.IsEmpty())
		{
			LogTag.System.I("Link is empty");

			return;
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		Application.OpenURL(m_LinkURL);
	}
}