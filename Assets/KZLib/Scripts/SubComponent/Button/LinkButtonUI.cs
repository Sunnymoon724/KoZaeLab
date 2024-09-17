using Sirenix.OdinInspector;
using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("링크 URL")]
	private string m_LinkURL = null;

	protected override void Initialize()
	{
		if(m_LinkURL.IsEmpty())
		{
			return;
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		Application.OpenURL(m_LinkURL);
	}
}