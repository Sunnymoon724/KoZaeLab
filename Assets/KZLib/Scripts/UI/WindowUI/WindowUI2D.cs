using UnityEngine;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using DG.Tweening;

public abstract class WindowUI2D : WindowUI
{
	[FoldoutGroup("UI ",Order = -10)]
	[VerticalGroup("UI/General",Order = 0),SerializeField,LabelText("Pooling")]
	private bool m_Pooling = true;
	public override bool IsPooling => m_Pooling;

	[VerticalGroup("UI/General",Order = 0),SerializeField,LabelText("Layer")]
	private UILayerType m_Layer = UILayerType.Panel;
	public override UILayerType Layer => m_Layer;

	[VerticalGroup("UI/General",Order = 0),SerializeField,LabelText("PopUp Transform"),ShowIf(nameof(IsPopup))]
	private Transform m_PopUpTransform = null;
	private bool IsPopup => Layer == UILayerType.PopUp;

	[VerticalGroup("UI/General",Order = 0),SerializeField,LabelText("Adjust Rect")]
	protected RectTransform m_AdjustRect = null;

	[VerticalGroup("UI/General",Order = 0),SerializeField,LabelText("Window Order")]
	private UIPriorityType m_Priority = UIPriorityType.Normal;
	public override UIPriorityType Priority => m_Priority;

	private readonly HashSet<WindowUI2D> m_LinkedHashSet = new();

	public override bool Is3D => false;

#if UNITY_ANDROID || UNITY_IOS
	private Rect m_SafeArea = new(0.0f,0.0f,0.0f,0.0f);
#endif

	protected override void Initialize()
	{
		base.Initialize();

#if UNITY_ANDROID || UNITY_IOS
		CheckSafeArea();
#endif

		if(Layer == UILayerType.PopUp && m_PopUpTransform)
		{
			AddSequence(ref m_OpenSequence,m_PopUpTransform.DOScale(0.5f,0.2f).From().SetEase(Ease.OutBack,2.0f));
		}
	}

	public override void Open(object _param)
	{
		base.Open(_param);

		m_OpenSequence?.Restart();
	}

	public override void Close()
	{
		foreach(var baseUI in m_LinkedHashSet)
		{
			UIMgr.In.Close(baseUI.Tag);
		}

		m_LinkedHashSet.Clear();

		m_CloseSequence?.Restart();

		base.Close();
	}

	public override void Hide(bool _hide)
	{
		base.Hide(_hide);

		transform.localPosition = _hide ? UIMgr.HIDE_POS : UIMgr.DEFAULT_POS;

		IsHide = _hide;
	}

	public void AddLink(WindowUI2D _window)
	{
		m_LinkedHashSet.Add(_window);
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
		if(!m_AdjustRect)
		{
			return;
		}

		var safeArea = Screen.safeArea;

		if(safeArea == m_SafeArea)
		{
			return;
		}

		m_SafeArea = safeArea;

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
			m_AdjustRect.anchorMin = anchorMin;
			m_AdjustRect.anchorMax = anchorMax;
		}
	}
#endif

	protected override void Reset()
	{
		base.Reset();

		if(!m_AdjustRect)
		{
			m_AdjustRect = UIRectTransform;
		}
	}
}