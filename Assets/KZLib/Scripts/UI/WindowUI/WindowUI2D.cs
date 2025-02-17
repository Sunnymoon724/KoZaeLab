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
	private UILayerType m_LayerType = UILayerType.Panel;
	public override UILayerType LayerType => m_LayerType;

	[VerticalGroup("UI/General",Order = 0),SerializeField,ShowIf(nameof(IsPopup))]
	private Transform m_popUpTransform = null;
	private bool IsPopup => LayerType == UILayerType.PopUp;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	protected RectTransform m_adjustRect = null;

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

#if UNITY_ANDROID || UNITY_IOS
		CheckSafeArea();
#endif

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
			UIMgr.In.Close(baseUI.Tag);
		}

		m_linkedHashSet.Clear();

		m_closeSequence?.Restart();

		base.Close();
	}

	public void AddLink(WindowUI2D windowUI2D)
	{
		m_linkedHashSet.Add(windowUI2D);
	}

	protected virtual void Update()
	{
#if UNITY_ANDROID || UNITY_IOS
		CheckSafeArea();
#endif
	}

#if UNITY_ANDROID || UNITY_IOS
	private void CheckSafeArea()
	{
		if(!m_adjustRect)
		{
			return;
		}

		var safeArea = Screen.safeArea;

		if(safeArea == m_safeArea)
		{
			return;
		}

		m_safeArea = safeArea;

		if(Screen.width <= 0 || Screen.height <= 0)
		{
			return;
		}

		var anchorMin = safeArea.position;
		var anchorMax = safeArea.position+safeArea.size;

		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;

		if(anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
		{
			m_adjustRect.anchorMin = anchorMin;
			m_adjustRect.anchorMax = anchorMax;
		}
	}
#endif

	protected override void Reset()
	{
		base.Reset();

		if(!m_adjustRect)
		{
			m_adjustRect = UIRectTransform;
		}
	}
}