using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IUnitStateParam { }

public interface IUnitState<TEnum> where TEnum : struct, Enum
{
	public TEnum Type { get; }
	public bool IsChangeAllowed { get; }

	public UniTask<TEnum> PlayStateAsync(IUnitStateParam param,CancellationToken token);
}

public abstract class UnitStateCon<TEnum> : MonoBehaviour where TEnum : struct, Enum
{
	private CancellationTokenSource m_tokenSource = null;

	public event Action<TEnum,TEnum> OnUnitStateChanged = null;

	[ShowInInspector] public TEnum StateType => m_state == null ? default : m_state.Type;

	private readonly Dictionary<TEnum,IUnitState<TEnum>> m_stateDict = new();
	private readonly Dictionary<TEnum,IUnitStateParam> m_paramDict = new();

	protected IUnitState<TEnum> m_state = null;

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

	public void EnterState(TEnum newType,bool isForce = false)
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

		if(!m_paramDict.TryGetValue(newType,out var param))
		{
			KZLogType.System.E($"{newType} state not found");

			return;
		}

		OnUnitStateChanged?.Invoke(StateType,newType);

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		m_state = state;

		_PlayStateAsync(state,param).Forget();
	}

	private async UniTask _PlayStateAsync(IUnitState<TEnum> state,IUnitStateParam param)
	{
		try
		{
			var nextState = await state.PlayStateAsync(param,m_tokenSource.Token);

			EnterState(nextState);
		}
		catch(OperationCanceledException)
		{
			// Force canceled. -> skip
		}
		catch(Exception exception)
		{
			KZLogType.System.E($"state exception : [{state.Type}] - {exception}");
		}
	}

	protected void _RegisterState(TEnum type,IUnitState<TEnum> state,IUnitStateParam param)
	{
		if(state == null)
		{
			return;
		}

		m_stateDict[type] = state;
		m_paramDict[type] = param;
	}
}