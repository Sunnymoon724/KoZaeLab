using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IUnitStateParam { }

public abstract class UnitStateCon<TEnum> : MonoBehaviour where TEnum : struct,Enum
{
	protected bool m_changeAllowed = false;
	protected CancellationTokenSource m_tokenSource = null;

	public event Action<TEnum,TEnum> OnUnitStateChanged = null;

	[ShowInInspector] public TEnum StateType => m_stateType;

	private readonly Dictionary<TEnum,Func<IUnitStateParam,UniTask<TEnum>>> m_stateFuncDict = new();

	protected TEnum m_stateType = default;

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

	public async UniTask EnterStateAsync(TEnum newType,IUnitStateParam param,bool isForce = false)
	{
		if(isActiveAndEnabled == false || !_CanChange(newType,isForce))
		{
			return;
		}

		var current = newType;

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		while(isActiveAndEnabled)
		{
			if(!m_stateFuncDict.TryGetValue(current, out var stateFunc))
			{
				Logger.System.E($"{current} state not found");

				return;
			}

			OnUnitStateChanged?.Invoke(m_stateType,current);

			m_stateType = current;

			_ReadyState();

			var token = m_tokenSource.Token;

			try
			{
				Logger.System.I($"{name} is entered {current}");

				var next = await stateFunc.Invoke(param);

				current = next;
				param = null;
			}
			catch(OperationCanceledException)
			{
				return;
			}

			if(!_CanChange(current,false))
			{
				break;
			}
		}
	}

	protected void _RegisterState(TEnum type,Func<IUnitStateParam,UniTask<TEnum>> stateFunc)
	{
		if(stateFunc == null)
		{
			return;
		}

		m_stateFuncDict[type] = stateFunc;
	}

	protected virtual void _ReadyState() { }
}