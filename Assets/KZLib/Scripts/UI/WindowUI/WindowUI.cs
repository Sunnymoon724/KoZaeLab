using DG.Tweening;
using KZLib;

public interface IWindowUI
{
	//? 열려고 할 때
	void Open(object _param);
	//? 닫으려고 할 때
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
	private MenuToolBarUI m_MenuToolBar = null;

	protected MenuToolBarUI MenuToolBar
	{
		get
		{
			if(!m_MenuToolBar)
			{
				m_MenuToolBar = GetComponentInChildren<MenuToolBarUI>(true);
			}

			return m_MenuToolBar;
		}
	}

	protected override void Initialize() { }

	public virtual void Open(object _param)
	{
		LogTag.UI.I("{0}가 열렸습니다.",Tag);

		gameObject.SetActiveSelf(true);

		IsOpen = true;
	}

	public virtual void Close()
	{
		gameObject.SetActiveSelf(false);

		IsOpen = false;

		LogTag.UI.I("{0}가 닫혔습니다.",Tag);
	}

	protected override void Release() { }

	public virtual void Hide(bool _hide)
	{
		if(_hide)
		{
			LogTag.UI.I("{0}의 숨겨진 상태입니다.",Tag);
		}
		else
		{
			LogTag.UI.I("{0}의 보여진 상태입니다.",Tag);
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