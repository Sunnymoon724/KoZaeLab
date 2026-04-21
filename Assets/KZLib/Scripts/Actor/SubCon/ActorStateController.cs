using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace KZLib.Actors
{
	public interface IActorStateParam { }

	public abstract class ActorStateController<TEnum> : IDisposable where TEnum : struct,Enum
	{
		private readonly MonoBehaviour m_actor = null;
		private bool m_showLog = false;

		protected bool m_changeAllowed = true;
		protected CancellationTokenSource m_stateTokenSource = null;
		private readonly Dictionary<TEnum,Func<CancellationToken,IActorStateParam,UniTask<TEnum>>> m_stateFuncDict = new();

		public TEnum StateType => m_stateType;
		protected TEnum m_stateType = default;

		private readonly Subject<ActorStateInfo> m_actorStateSubject = new();
		public Observable<ActorStateInfo> OnChangedActorState => m_actorStateSubject;

		protected abstract bool _CanChange(TEnum newStateTag,bool isForce);

		public ActorStateController(MonoBehaviour actor,bool showLog = false)
		{
			m_actor = actor;

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

		public async UniTask EnterStateAsync(TEnum newState,IActorStateParam param,bool isForce = false)
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

			KZExternalKit.RecycleTokenSourceInMono(ref m_stateTokenSource,m_actor);

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

				m_actorStateSubject.OnNext(new ActorStateInfo(m_stateType,curNextState));

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

		protected void _RegisterState(TEnum type,Func<CancellationToken,IActorStateParam,UniTask<TEnum>> stateFunc)
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
			return m_actor != null && m_actor.isActiveAndEnabled;
		}

		private void _ShowLog(string text)
		{
			LogChannel.Develop.I($"[ActorState] {m_actor.name} {text}");
		}
	}
}