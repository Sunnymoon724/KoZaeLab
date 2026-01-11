using System;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IWindow
{
	void Open(object param);
	void Close();
	bool IsOpen { get; }

	void Hide(bool isHidden);
	bool IsHidden { get; }

	void BlockInput(bool isBlocked);
	bool IsBlocked { get; }

	bool Is3D { get; }

	WindowPrefabType WindowType { get; }
	WindowPriorityType PriorityType { get; }

	CommonUINameTag NameTag { get; }

	bool IsPooling { get; }

	bool IsIgnoreHide { get; }
}

[RequireComponent(typeof(CanvasGroup))]
public abstract class Window : BaseComponent,IWindow
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

	public abstract WindowPrefabType WindowType { get; }
	public abstract WindowPriorityType PriorityType { get; }

	public abstract void BlockInput(bool isBlocked);

	private CommonUINameTag m_nameTag = null;
	public CommonUINameTag NameTag
	{
		get
		{
			if(m_nameTag == null)
			{
				var typeName = GetType().Name;

				if(typeName.TryToCustomTag<CommonUINameTag>(out var nameTag))
				{
					m_nameTag = nameTag;
				}
				else
				{
					LogChannel.UI.E($"{typeName} is not defined UINameTag");

					m_nameTag = CommonUINameTag.None;
				}
			}

			return m_nameTag;
		}
	}

	public abstract bool Is3D { get; }

	private readonly HashSet<Window2D> m_linkedWindowHashSet = new();

	private Action<int> m_onClose = null;
	protected int m_result = -1;

	public void SetCanvas(Canvas canvas)
	{
		m_canvas = canvas;
	}

	protected override void _Initialize()
	{
		base._Initialize();

		_SetCanvasGroupState(1,true,true);
	}

	public virtual void Open(object param)
	{
		LogChannel.UI.I($"{NameTag} is opened");

		gameObject.EnsureActive(true);
	}

	public virtual void Close()
	{
		foreach(var window in m_linkedWindowHashSet)
		{
			UIManager.In.Close(window.NameTag);
		}

		m_linkedWindowHashSet.Clear();

		m_onClose?.Invoke(m_result);

		gameObject.EnsureActive(false);

		LogChannel.UI.I($"{NameTag} is closed");
	}

	protected override void _Release() { }

	public virtual void Hide(bool isHidden)
	{
		if(isHidden)
		{
			LogChannel.UI.I($"{NameTag} is hidden");

			_SetCanvasGroupState(0,false,false);
		}
		else
		{
			LogChannel.UI.I($"{NameTag} is shown");

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
		UIManager.In.Close(NameTag);
	}

	public void AddLink(Window2D window)
	{
		m_linkedWindowHashSet.Add(window);
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