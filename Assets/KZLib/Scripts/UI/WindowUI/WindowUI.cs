using System;
using System.Collections.Generic;
using DG.Tweening;
using KZLib;
using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IWindowUI
{
	void Open(object param);
	void Close();
	bool IsOpen { get; }

	void Hide(bool isHidden);
	bool IsHidden { get; }

	void BlockInput(bool isBlocked);
	bool IsBlocked { get; }

	bool Is3D { get; }

	WindowUIType WindowType { get; }
	UIPriorityType PriorityType { get; }

	UINameType NameType { get; }

	bool IsPooling { get; }

	bool IsIgnoreHide { get; }
}

[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowUI : BaseComponentUI,IWindowUI
{
	[InfoBox("CanvasGroup is null",InfoMessageType.Error,nameof(IsExistCanvasGroup))]
	[VerticalGroup("CanvasGroup",Order = -25),SerializeField]
	protected CanvasGroup m_canvasGroup = null;

	private bool IsExistCanvasGroup => m_canvasGroup == null;

	protected Canvas m_canvas = null;

	public bool IsOpen => gameObject.activeSelf;
	public bool IsHidden => m_canvasGroup.alpha == 0 && IsBlocked;
	public abstract bool IsBlocked { get; }

	public bool IsIgnoreHide { get; protected set; }

	public abstract bool IsPooling  { get; }

	public abstract WindowUIType WindowType { get; }
	public abstract UIPriorityType PriorityType { get; }

	public abstract void BlockInput(bool isBlocked);

	private UINameType? m_nameType = null;
	public UINameType NameType
	{
		get
		{
			if(!m_nameType.HasValue)
			{
				var typeName = GetType().Name;

				if(typeName.TryToEnum<UINameType>(out var nameType))
				{
					m_nameType = nameType;
				}
				else
				{
					LogSvc.UI.E($"{typeName} is not defined UINameType");

					m_nameType = UINameType.None;
				}
			}

			return m_nameType.Value;
		}
	}

	public abstract bool Is3D { get; }

	private readonly HashSet<WindowUI2D> m_linkedHashSet = new();

	private Action<int> m_onClose = null;
	protected int m_result = -1;

	public void SetCanvas(Canvas canvas)
	{
		m_canvas = canvas;
	}

	protected override void Initialize()
	{
		base.Initialize();

		_SetCanvasGroupState(1,true,true);
	}

	public virtual void Open(object param)
	{
		LogSvc.UI.I($"{NameType} is opened");

		gameObject.EnsureActive(true);
	}

	public virtual void Close()
	{
		foreach(var window in m_linkedHashSet)
		{
			UIManager.In.Close(window.NameType);
		}

		m_linkedHashSet.Clear();

		m_onClose?.Invoke(m_result);

		gameObject.EnsureActive(false);

		LogSvc.UI.I($"{NameType} is closed");
	}

	protected override void Release() { }

	public virtual void Hide(bool isHidden)
	{
		if(isHidden)
		{
			LogSvc.UI.I($"{NameType} is hidden");

			_SetCanvasGroupState(0,false,false);
		}
		else
		{
			LogSvc.UI.I($"{NameType} is shown");

			_SetCanvasGroupState(1,true,true);
		}
	}

	protected void _SetCanvasGroupState(int alpha,bool interactable,bool blocksRaycasts)
	{
		m_canvasGroup.alpha = alpha;
		m_canvasGroup.interactable = interactable;
		m_canvasGroup.blocksRaycasts = blocksRaycasts;
	}

	protected virtual void SelfClose()
	{
		UIManager.In.Close(NameType);
	}

	public void AddLink(WindowUI2D windowUI2D)
	{
		m_linkedHashSet.Add(windowUI2D);
	}

	public void SetOnClose(Action<int> onClose)
	{
		m_onClose = onClose;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_canvasGroup)
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
		}
	}
}