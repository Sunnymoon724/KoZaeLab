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

	void Show();
	void Hide();
	bool IsHidden { get; }

	void BlockInput();
	void AllowInput();
	bool IsInputBlocked { get; }

	bool Is3D { get; }

	UILayerType LayerType { get; }
	UIPriorityType PriorityType { get; }

	UINameType NameType { get; }

	bool IsPooling { get; }

	bool IsIgnoreHide { get; }
}

public abstract class WindowUI : BaseComponentUI,IWindowUI
{
	[InfoBox("canvasGroup is null",InfoMessageType.Error,nameof(IsValidCanvasGroup))]
	[VerticalGroup("CanvasGroup",Order = -25),SerializeField]
	protected CanvasGroup m_canvasGroup = null;
	
	private bool IsValidCanvasGroup => m_canvasGroup == null;

	protected Canvas m_canvas = null;

	public bool IsOpen => gameObject.activeSelf;
	public abstract bool IsHidden { get; }
	public abstract bool IsInputBlocked { get; }

	public bool IsIgnoreHide { get; protected set; }

	public abstract bool IsPooling  { get; }

	public abstract UILayerType LayerType { get; }
	public abstract UIPriorityType PriorityType { get; }

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
		LogSvc.UI.I($"{NameType} is opened");

		gameObject.EnsureActive(true);
	}

	public virtual void Close()
	{
		BlockInput();

		gameObject.EnsureActive(false);

		LogSvc.UI.I($"{NameType} is closed");
	}

	protected override void Release() { }

	public virtual void Show()
	{
		LogSvc.UI.I($"{NameType} is shown");

		_SetCanvasGroupState(1,true,true);
	}

	public virtual void Hide()
	{
		LogSvc.UI.I($"{NameType} is hidden");

		_SetCanvasGroupState(0,false,false);
	}

	public void BlockInput()
	{
		LogSvc.UI.I($"{NameType} input is blocked");

		_SetCanvasGroupState(1,false,false);
	}

	public void AllowInput()
	{
		LogSvc.UI.I($"{NameType} input is allowed");

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
		UIManager.In.Close(NameType);
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