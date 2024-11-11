using System;

namespace KZLib
{
	#region NewAction
	public class NewAction
	{
		private Action m_OnAction = null;

		public int Count => m_OnAction?.GetInvocationList().Length ?? 0;

		public NewAction() { }

		public NewAction(Action _onAction)
		{
			m_OnAction = _onAction;
		}

		public void AddListener(Action _onAction,bool _overlap = false)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			if(!_overlap)
			{
				RemoveListener(_onAction);
			}

			m_OnAction += _onAction;
		}

		public void SetListener(Action _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			RemoveAllListeners();

			AddListener(_onAction,false);
		}

		public void AddListenerAtOnce(Action _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			void onAction()
			{
				_onAction.Invoke();

				RemoveListener(onAction);
			}

			AddListener(onAction);
		}

		public void RemoveListener(Action _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			m_OnAction -= _onAction;
		}

		public void RemoveAllListeners()
		{
			m_OnAction = null;
		}

		public void Invoke()
		{
			m_OnAction?.Invoke();
		}
	}
	#endregion NewAction

	// -----------------------------------------------------------------------------------------------

	#region NewAction<TObject>
	public class NewAction<TObject>
	{
		private Action<TObject> m_OnAction = null;

		public int Count => m_OnAction?.GetInvocationList().Length ?? 0;

		public NewAction() : this(null) { }

		public NewAction(Action<TObject> _onAction)
		{
			m_OnAction = _onAction;
		}

		public void AddListener(Action<TObject> _onAction,bool _overlap = false)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			if(!_overlap)
			{
				RemoveListener(_onAction);
			}

			m_OnAction += _onAction;
		}

		public void SetListener(Action<TObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			RemoveAllListeners();

			AddListener(_onAction,false);
		}

		public void AddListenerAtOnce(Action<TObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			void onAction(TObject objectT)
			{
				_onAction.Invoke(objectT);

				RemoveListener(onAction);
			}

			AddListener(onAction);
		}

		public void RemoveListener(Action<TObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			m_OnAction -= _onAction;
		}

		public void RemoveAllListeners()
		{
			m_OnAction = null;
		}

		public void Invoke(TObject _objectT)
		{
			m_OnAction?.Invoke(_objectT);
		}
	}
	#endregion NewAction<TObject>

	// -----------------------------------------------------------------------------------------------

	#region NewAction<TObject,UObject>
	public class NewAction<TObject,UObject>
	{
		private Action<TObject,UObject> m_OnAction = null;

		public int Count => m_OnAction?.GetInvocationList().Length ?? 0;

		public NewAction() : this(null) { }

		public NewAction(Action<TObject,UObject> _onAction)
		{
			m_OnAction = _onAction;
		}

		public void AddListener(Action<TObject,UObject> _onAction,bool _overlap = false)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			if(!_overlap)
			{
				RemoveListener(_onAction);
			}

			m_OnAction += _onAction;
		}

		public void SetListener(Action<TObject,UObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			RemoveAllListeners();

			AddListener(_onAction,false);
		}

		public void AddListenerAtOnce(Action<TObject,UObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			void onAction(TObject objectT,UObject objectU)
			{
				_onAction.Invoke(objectT,objectU);

				RemoveListener(onAction);
			}

			AddListener(onAction);
		}

		public void RemoveListener(Action<TObject,UObject> _onAction)
		{
			if(_onAction == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			m_OnAction -= _onAction;
		}

		public void RemoveAllListeners()
		{
			m_OnAction = null;
		}

		public void Invoke(TObject _objectT,UObject _objectU)
		{
			m_OnAction?.Invoke(_objectT,_objectU);
		}
	}
	#endregion NewAction

	// -----------------------------------------------------------------------------------------------

	#region NewFunc<TResult>
	public class NewFunc<TResult>
	{
		private Func<TResult> m_OnFunc = null;

		public int Count => m_OnFunc?.GetInvocationList().Length ?? 0;

		public NewFunc() : this(null) { }

		public NewFunc(Func<TResult> _onFunc)
		{
			m_OnFunc = _onFunc;
		}

		public void AddListener(Func<TResult> _onFunc,bool _overlap = false)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			if(!_overlap)
			{
				RemoveListener(_onFunc);
			}

			m_OnFunc += _onFunc;
		}

		public void SetListener(Func<TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			RemoveAllListeners();

			AddListener(_onFunc,false);
		}

		public void AddListenerAtOnce(Func<TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			TResult onFunc()
			{
				RemoveListener(onFunc);

				return _onFunc.Invoke();
			}

			AddListener(onFunc);
		}

		public void RemoveListener(Func<TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			m_OnFunc -= _onFunc;
		}

		public void RemoveAllListeners()
		{
			m_OnFunc = null;
		}

		public TResult Invoke()
		{
			return m_OnFunc != null ? m_OnFunc.Invoke() : default;
		}
	}
	#endregion NewFunc<TResult>

	// -----------------------------------------------------------------------------------------------

	#region NewFunc<TObject,TResult>
	public class NewFunc<TObject,TResult>
	{
		private Func<TObject,TResult>m_OnFunc = null;

		public int Count => m_OnFunc?.GetInvocationList().Length ?? 0;

		public NewFunc() : this(null) { }

		public NewFunc(Func<TObject,TResult> _onFunc)
		{
			m_OnFunc = _onFunc;
		}

		public void AddListener(Func<TObject,TResult> _onFunc,bool _overlap = false)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			if(!_overlap)
			{
				RemoveListener(_onFunc);
			}

			m_OnFunc += _onFunc;
		}

		public void SetListener(Func<TObject,TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			RemoveAllListeners();

			AddListener(_onFunc,false);
		}

		public void AddListenerAtOnce(Func<TObject,TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			TResult onFunc(TObject objectT)
			{
				RemoveListener(onFunc);

				return _onFunc.Invoke(objectT);
			}

			AddListener(onFunc);
		}

		public void RemoveListener(Func<TObject,TResult> _onFunc)
		{
			if(_onFunc == null)
			{
				LogTag.System.E("Callback is null");

				return;
			}

			m_OnFunc -= _onFunc;
		}

		public void RemoveAllListeners()
		{
			m_OnFunc = null;
		}

		public TResult Invoke(TObject _objectT)
		{
			return m_OnFunc != null ? m_OnFunc.Invoke(_objectT) : default;
		}
	}
	#endregion NewFunc<TObject,TResult>
}