using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IUnitStateParam { }

public interface IUnitState<TEnum> where TEnum : struct, Enum
{
	public bool IsChangeAllowed { get; }

	public UniTask<TEnum> PlayStateAsync(IUnitStateParam param,CancellationToken token);
}

public abstract class UnitStateCon<TEnum> : MonoBehaviour where TEnum : struct,Enum
{
	private CancellationTokenSource m_tokenSource = null;

	public event Action<TEnum,TEnum> OnUnitStateChanged = null;

	[ShowInInspector] public TEnum StateType => m_stateType;

	private readonly Dictionary<TEnum,IUnitState<TEnum>> m_stateDict = new();

	protected TEnum m_stateType = default;
	protected IUnitState<TEnum> m_currentState = null;

	protected abstract bool _CanChange(TEnum newStateTag,bool isForce);

	public void Initialize()
	{
		m_stateDict.Clear();
	}

	public void Release()
	{
		m_stateDict.Clear();

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	public void EnterState(TEnum newType,IUnitStateParam param,bool isForce = false)
	{
		if(isActiveAndEnabled == false || !_CanChange(newType,isForce))
		{
			return;
		}

		if(!m_stateDict.TryGetValue(newType,out var state))
		{
			KZLogType.System.E($"{newType} state not found");

			return;
		}

		OnUnitStateChanged?.Invoke(m_stateType,newType);

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		m_currentState = state;
		m_stateType = newType;

		_PlayStateAsync(state,param).Forget();
	}

	protected async UniTask _PlayStateAsync(IUnitState<TEnum> state,IUnitStateParam param)
	{
		try
		{
			_ReadyState();

			var nextState = await state.PlayStateAsync(param,m_tokenSource.Token);

			EnterState(nextState,null);
		}
		catch(OperationCanceledException)
		{
			// Force canceled. -> skip
		}
		catch(Exception exception)
		{
			KZLogType.System.E($"state exception : [{m_stateType}] - {exception}");
		}
	}

	protected void _RegisterState(TEnum type,IUnitState<TEnum> state)
	{
		if(state == null)
		{
			return;
		}

		m_stateDict[type] = state;
	}

	protected virtual void _ReadyState() { }
}