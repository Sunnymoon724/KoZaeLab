using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace KZLib
{
    public interface IUnitStateParam { }

	public abstract class UnitStateController<TEnum> : SerializedMonoBehaviour where TEnum : struct,Enum
	{
		protected bool m_changeAllowed = false;
		protected CancellationTokenSource m_tokenSource = null;
		private readonly Dictionary<TEnum,Func<CancellationToken,IUnitStateParam,UniTask<TEnum>>> m_stateFuncDict = new();

		[ShowInInspector] public TEnum StateType => m_stateType;

		protected TEnum m_stateType = default;

		public event Action<TEnum,TEnum> OnUnitStateChanged = null;

		protected abstract bool _CanChange(TEnum newStateTag,bool isForce);

		public void Initialize()
		{
			m_stateFuncDict.Clear();
		}

		public void Release()
		{
			m_stateFuncDict.Clear();

			CommonUtility.KillTokenSource(ref m_tokenSource);
		}

		public async UniTask EnterStateAsync(TEnum newState,IUnitStateParam param,bool isForce = false)
		{
			if(isActiveAndEnabled == false || !_CanChange(newState,isForce))
			{
				return;
			}

			var curState = newState;

			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			while(isActiveAndEnabled)
			{
				if(!m_stateFuncDict.TryGetValue(curState, out var stateFunc))
				{
					LogSvc.System.E($"{curState} state not found");

					return;
				}

				OnUnitStateChanged?.Invoke(m_stateType,curState);

				m_stateType = curState;

				_ReadyState();

				LogSvc.System.I($"{name} is entered {curState}");

				var nextState = await stateFunc.Invoke(m_tokenSource.Token,param);

				if(!_CanChange(nextState,false))
				{
					// wait one frame
					await UniTask.Yield();
				}
				else
				{
					curState = nextState;
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

		protected virtual void _ReadyState() { }
	}
}