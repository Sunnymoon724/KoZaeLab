using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopupUI : WindowUI2D
{
	public override WindowUIType WindowType => WindowUIType.PopUp;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private Transform m_popUpTransform = null;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private Button m_closeButton = null;

	protected override void Initialize()
	{
		base.Initialize();

		if(m_popUpTransform)
		{
			AddSequence(ref m_openSequence,m_popUpTransform.DOScale(0.5f,0.2f).From().SetEase(Ease.OutBack,2.0f));
		}

		if(m_closeButton)
		{
			m_closeButton.onClick.AddAction(Close);
		}
	}

	protected override void Release()
	{
		base.Release();

		if(m_closeButton)
		{
			m_closeButton.onClick.RemoveAction(Close);
		}
	}
}