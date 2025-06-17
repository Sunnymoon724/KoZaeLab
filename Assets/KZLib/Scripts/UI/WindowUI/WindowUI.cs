using DG.Tweening;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IWindowUI
{
	void Open(object param);
	void Close();
	bool IsOpen { get; }

	void Show();
	void Hide();
	bool IsHidden { get; }

	void BlockInput();
	void AllowInput();
	bool IsInputBlocked { get; }

	bool Is3D { get; }

	UILayerType LayerType { get; }
	UIPriorityType PriorityType { get; }

	string Tag { get; }

	bool IsPooling { get; }

	bool IsIgnoreHide { get; }
}

public abstract class WindowUI : BaseComponentUI,IWindowUI
{
	[InfoBox("canvasGroup is null",InfoMessageType.Error,"@this.m_canvasGroup == null")]
	[VerticalGroup("CanvasGroup",Order = -25),SerializeField]
	protected CanvasGroup m_canvasGroup = null;

	protected Canvas m_canvas = null;

	public bool IsOpen => gameObject.activeSelf;
	public abstract bool IsHidden { get; }
	public abstract bool IsInputBlocked { get; }

	public bool IsIgnoreHide { get; protected set; }

	public abstract bool IsPooling  { get; }

	public abstract UILayerType LayerType { get; }
	public abstract UIPriorityType PriorityType { get; }
	public abstract string Tag { get; }

	public abstract bool Is3D { get; }


	protected Sequence m_openSequence = null;
	protected Sequence m_closeSequence = null;

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
		Logger.UI.I($"{Tag} is opened");

		gameObject.EnsureActive(true);
	}

	public virtual void Close()
	{
		BlockInput();

		gameObject.EnsureActive(false);

		Logger.UI.I($"{Tag} is closed");
	}

	protected override void Release() { }

	public virtual void Show()
	{
		Logger.UI.I($"{Tag} is shown");

		_SetCanvasGroupState(1,true,true);
	}

	public virtual void Hide()
	{
		Logger.UI.I($"{Tag} is hidden");

		_SetCanvasGroupState(0,false,false);
	}

	public void BlockInput()
	{
		Logger.UI.I($"{Tag} input is blocked");

		_SetCanvasGroupState(1,false,false);
	}

	public void AllowInput()
	{
		Logger.UI.I($"{Tag} input is allowed");

		_SetCanvasGroupState(1,true,true);
	}

	private void _SetCanvasGroupState(int alpha,bool interactable,bool blocksRaycasts)
	{
		m_canvasGroup.alpha = alpha;
		m_canvasGroup.interactable = interactable;
		m_canvasGroup.blocksRaycasts = blocksRaycasts;
	}

	protected virtual void SelfClose()
	{
		UIMgr.In.Close(Tag);
	}

	protected void AddSequence(ref Sequence sequence,Tween tween)
	{
		if(sequence == null)
		{
			sequence = DOTween.Sequence().SetAutoKill(false);
			sequence.Append(tween);
			sequence.SetUpdate(true);
			sequence.Pause();
		}
		else
		{
			sequence.Join(tween);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_canvasGroup)
		{
			m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
	}
}