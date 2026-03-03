using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopup : Window2D
{
	public override WindowPrefabType WindowType => WindowPrefabType.PopUp;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private Transform m_popUpTransform = null;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private Button m_closeButton = null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(m_popUpTransform)
		{
			AddSequence(ref m_openSequence,m_popUpTransform.DOScale(0.5f,0.2f).From().SetEase(Ease.OutBack,2.0f));
		}
	}

	protected override void _OnEnable()
	{
		base._OnEnable();

		if(m_closeButton)
		{
			m_closeButton.onClick.AddAction(Close);
		}
	}

	protected override void _OnDisable()
	{
		base._OnDisable();

		if(m_closeButton)
		{
			m_closeButton.onClick.RemoveAction(Close);
		}
	}
}