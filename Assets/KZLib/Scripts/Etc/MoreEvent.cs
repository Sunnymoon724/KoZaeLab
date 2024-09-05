using System;

namespace KZLib
{
	/// <summary>
	/// 중복 실행 방지 델리게이트
	/// </summary>

	#region MoreAction
	public class MoreAction
	{
		private Action m_OnAction = null;

		public MoreAction() : this(null) { }

		public MoreAction(Action _onAction)
		{
			m_OnAction = _onAction;
		}

		public static MoreAction operator +(MoreAction _delegate,Action _onAction)
		{
			_delegate ??= new MoreAction();

			_delegate.m_OnAction -= _onAction;
			_delegate.m_OnAction += _onAction;

			return _delegate;
		}

		public static MoreAction operator -(MoreAction _delegate,Action _onAction)
		{
			_delegate ??= new MoreAction();

			_delegate.m_OnAction -= _onAction;

			return _delegate;
		}

		public void Invoke()
		{
			m_OnAction?.Invoke();
		}
	}
	#endregion MoreAction

	#region MoreAction<TObject>
	public class MoreAction<TObject>
	{
		private Action<TObject> m_OnAction = null;

		public MoreAction() : this(null) { }

		public MoreAction(Action<TObject> _onAction)
		{
			m_OnAction = _onAction;
		}

		public static MoreAction<TObject> operator +(MoreAction<TObject> _delegate,Action<TObject> _onAction)
		{
			_delegate ??= new MoreAction<TObject>();

			_delegate.m_OnAction -= _onAction;
			_delegate.m_OnAction += _onAction;

			return _delegate;
		}

		public static MoreAction<TObject> operator -(MoreAction<TObject> _delegate,Action<TObject> _onAction)
		{
			_delegate ??= new MoreAction<TObject>();

			_delegate.m_OnAction -= _onAction;

			return _delegate;
		}

		public void Invoke(TObject _value)
		{
			m_OnAction?.Invoke(_value);
		}
	}
	#endregion MoreAction<TObject>

	#region MoreFunc<TObject>
	public class MoreFunc<TObject>
	{
		private Func<TObject> m_OnFunc = null;

		public MoreFunc() : this(null) { }

		public MoreFunc(Func<TObject> _onFunc)
		{
			m_OnFunc = _onFunc;
		}

		public static MoreFunc<TObject> operator +(MoreFunc<TObject> _delegate,Func<TObject> _onFunc)
		{
			_delegate ??= new MoreFunc<TObject>();

			_delegate.m_OnFunc -= _onFunc;
			_delegate.m_OnFunc += _onFunc;

			return _delegate;
		}

		public static MoreFunc<TObject> operator -(MoreFunc<TObject> _delegate,Func<TObject> _onFunc)
		{
			_delegate ??= new MoreFunc<TObject>();

			_delegate.m_OnFunc -= _onFunc;

			return _delegate;
		}

		public TObject Invoke()
		{
			return m_OnFunc != null ? m_OnFunc.Invoke() : default;
		}
	}
	#endregion MoreFunc<TObject>
}