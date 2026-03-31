using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace KZLib
{
    public interface IUnitStateParam { }

	public abstract class UnitStateController<TEnum> : IDisposable where TEnum : struct,Enum
	{
		private readonly MonoBehaviour m_unitController = null;
		private bool m_showLog = false;

		protected bool m_changeAllowed = true;
		protected CancellationTokenSource m_stateTokenSource = null;
		private readonly Dictionary<TEnum,Func<CancellationToken,IUnitStateParam,UniTask<TEnum>>> m_stateFuncDict = new();

		public TEnum StateType => m_stateType;

		protected TEnum m_stateType = default;

		private readonly Subject<UnitStateInfo> m_unitStateSubject = new();
		public Observable<UnitStateInfo> OnChangedUnitState => m_unitStateSubject;

		protected abstract bool _CanChange(TEnum newStateTag,bool isForce);

		public UnitStateController(MonoBehaviour unitController,bool showLog = false)
		{
			m_unitController = unitController;

			SetShowLog(showLog);
			m_stateFuncDict.Clear();
		}

		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void _Dispose(bool disposing)
		{
			if(!disposing)
			{
				return;
			}

			m_stateFuncDict.Clear();

			KZExternalKit.KillTokenSource(ref m_stateTokenSource);
		}

		public void SetShowLog(bool showLog)
		{
			m_showLog = showLog;
		}

		public async UniTask EnterStateAsync(TEnum newState,IUnitStateParam param,bool isForce = false)
		{
			if(!_IsActiveAndEnabled())
			{
				return;
			}

			if(!_CanChange(newState,isForce))
			{
				if(m_showLog)
				{
					_ShowLog($"cannot change to {newState} (Force: {isForce})");
				}

				return;
			}

			KZExternalKit.RecycleTokenSourceInMono(ref m_stateTokenSource,m_unitController);

            var token = m_stateTokenSource.Token;
			var curNextState = newState;
			var curParam = param;

			while(!token.IsCancellationRequested && _IsActiveAndEnabled())
			{
				if(!m_stateFuncDict.TryGetValue(curNextState,out var stateFunc))
				{
					LogChannel.Develop.E($"{curNextState} state not found");

					return;
				}

				m_unitStateSubject.OnNext(new UnitStateInfo(m_stateType,curNextState));

				m_stateType = curNextState;

				_ReadyState();

				if(m_showLog)
				{
					_ShowLog($"is entered {curNextState}");
				}

				var (isCanceled,nextState) = await stateFunc.Invoke(m_stateTokenSource.Token,curParam).SuppressCancellationThrow();

				if(isCanceled)
				{
					return;
				}

				if(!_CanChange(nextState,false))
				{
					var isYieldCanceled = await UniTask.Yield(cancellationToken: token).SuppressCancellationThrow();

					if(isYieldCanceled)
					{
						return;
					}
				}
				else
				{
					curNextState = nextState;
					curParam = null;
				}
			}
		}

		protected void _RegisterState(TEnum type,Func<CancellationToken,IUnitStateParam,UniTask<TEnum>> stateFunc)
		{
			if(stateFunc == null)
			{
				return;
			}

			m_stateFuncDict[type] = stateFunc;
		}

		protected async UniTask<TEnum> _ExecuteWithLockAsync(CancellationToken token,Func<CancellationToken,UniTask> stateFunc,TEnum nextState)
		{
			try
			{
				m_changeAllowed = false;

				await stateFunc(token).SuppressCancellationThrow();
			}
			finally
			{
				m_changeAllowed = true;
			}

			return nextState;
		}

		protected virtual void _ReadyState() { }

		private bool _IsActiveAndEnabled()
		{
			return m_unitController != null && m_unitController.isActiveAndEnabled;
		}

		private void _ShowLog(string text)
		{
			LogChannel.Develop.I($"[UnitState] {m_unitController.name} {text}");
		}
	}
}