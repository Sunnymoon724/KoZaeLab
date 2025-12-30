using UnityEngine;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using DG.Tweening;

public abstract class WindowUI2D : WindowUI
{
	public override bool IsHidden => m_canvasGroup.alpha == 0 && IsInputBlocked;
	public override bool IsInputBlocked => !m_canvasGroup.interactable && !m_canvasGroup.blocksRaycasts;

	[BoxGroup("UI",Order = -10)]
	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private bool m_pooling = true;
	public override bool IsPooling => m_pooling;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private UILayerType m_layerType = UILayerType.Panel;
	public override UILayerType LayerType => m_layerType;

	[VerticalGroup("UI/General",Order = 0),SerializeField,ShowIf(nameof(IsPopup))]
	private Transform m_popUpTransform = null;
	private bool IsPopup => LayerType == UILayerType.PopUp;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private UIPriorityType m_priorityType = UIPriorityType.Middle;
	public override UIPriorityType PriorityType => m_priorityType;

	private readonly HashSet<WindowUI2D> m_linkedHashSet = new();

	public override bool Is3D => false;

#if UNITY_ANDROID || UNITY_IOS
	private Rect m_safeArea = new(0.0f,0.0f,0.0f,0.0f);
#endif

	protected override void Initialize()
	{
		base.Initialize();

		if(LayerType == UILayerType.PopUp && m_popUpTransform)
		{
			AddSequence(ref m_openSequence,m_popUpTransform.DOScale(0.5f,0.2f).From().SetEase(Ease.OutBack,2.0f));
		}
	}

	public override void Open(object param)
	{
		base.Open(param);

		m_openSequence?.Restart();
	}

	public override void Close()
	{
		foreach(var baseUI in m_linkedHashSet)
		{
			UIManager.In.Close(baseUI.NameType);
		}

		m_linkedHashSet.Clear();

		m_closeSequence?.Restart();

		base.Close();
	}

	public void AddLink(WindowUI2D windowUI2D)
	{
		m_linkedHashSet.Add(windowUI2D);
	}
}