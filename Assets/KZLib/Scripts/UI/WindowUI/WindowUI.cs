using DG.Tweening;
using KZLib;

public interface IWindowUI
{
	void Open(object _param);
	void Close();

	void Hide(bool _hide);
	bool IsHide { get; }

	bool Is3D { get; }

	UILayerType Layer { get; }
	UIPriorityType Priority { get; }

	UITag Tag { get; }

	bool IsPooling { get; }

	bool IsOpen { get; }

	bool IsIgnoreHide { get; }
}

public abstract class WindowUI : SortingLayerCanvas,IWindowUI
{
	public bool IsOpen { get; private set; }

	public abstract bool IsPooling  { get; }

	public abstract UILayerType Layer { get; }
	public abstract UIPriorityType Priority { get; }

	public abstract bool Is3D { get; }

	public bool IsHide { get; protected set; }
	public bool IsIgnoreHide { get; protected set; }

	public abstract UITag Tag { get; }

	protected Sequence m_OpenSequence = null;
	protected Sequence m_CloseSequence = null;

	protected override void Initialize() { }

	public virtual void Open(object _param)
	{
		LogTag.UI.I($"{Tag} is opened");

		gameObject.SetActiveSelf(true);

		IsOpen = true;
	}

	public virtual void Close()
	{
		gameObject.SetActiveSelf(false);

		IsOpen = false;

		LogTag.UI.I($"{Tag} is closed");
	}

	protected override void Release() { }

	public virtual void Hide(bool _hide)
	{
		if(_hide)
		{
			LogTag.UI.I($"{Tag} is hidden");
		}
		else
		{
			LogTag.UI.I($"{Tag} is shown");
		}
	}

	protected virtual void SelfClose()
	{
		UIMgr.In.Close(Tag);
	}

	protected void AddSequence(ref Sequence _sequence,Tween _tween)
	{
		if(_sequence == null)
		{
			_sequence = DOTween.Sequence().SetAutoKill(false);
			_sequence.Append(_tween);
			_sequence.SetUpdate(true);
			_sequence.Pause();
		}
		else
		{
			_sequence.Join(_tween);
		}
	}
}