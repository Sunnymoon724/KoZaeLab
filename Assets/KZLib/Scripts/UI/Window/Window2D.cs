using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public abstract class Window2D : Window
{
	public override bool IsBlocked => m_blocked;

	[BoxGroup("UI",Order = -10)]
	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private bool m_pooling = true;
	public override bool IsPooling => m_pooling;

	[VerticalGroup("UI/General",Order = 0),SerializeField]
	private WindowPriorityType m_priorityType = WindowPriorityType.Middle;
	public override WindowPriorityType PriorityType => m_priorityType;

	public override bool Is3D => false;

	protected Sequence m_openSequence = null;
	protected Sequence m_closeSequence = null;

	private bool m_blocked = false;

	public override void Open(object param)
	{
		base.Open(param);

		if(m_openSequence != null)
		{
			m_openSequence.OnComplete(_OnOpenSequenceComplete);
			m_openSequence.Restart();
		}
	}

	public override void Close()
	{
		if(m_closeSequence != null)
		{
			m_openSequence.OnComplete(_OnCloseSequenceComplete);
			m_closeSequence.Restart();
		}
		else
		{
			base.Close();
		}
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

	protected virtual void _OnOpenSequenceComplete() { }

	protected virtual void _OnCloseSequenceComplete()
	{
		base.Close();
	}

	public virtual void PressBackButton()
	{
		SelfClose();
	}

	public override void BlockInput(bool isBlocked)
	{
		if(isBlocked)
		{
			LogSvc.UI.I($"{NameTag} input is blocked");

			_SetCanvasGroupState(1,false,false);

			m_blocked = true;
		}
		else
		{
			LogSvc.UI.I($"{NameTag} input is allowed");

			_SetCanvasGroupState(1,true,true);

			m_blocked = false;
		}
	}
}