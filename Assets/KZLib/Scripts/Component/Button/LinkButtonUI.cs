using UnityEngine;

public class LinkButtonUI : BaseButtonUI
{
	[SerializeField]
	private string m_LinkURL = null;

	protected override void Awake()
	{
		base.Awake();

		if(m_LinkURL.IsEmpty())
		{
			return;
		}

		m_Button.SetOnClickListener(()=>
		{
			Application.OpenURL(m_LinkURL);
		});
	}
}