using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IUnitState<TEnum> where TEnum : struct,Enum
{
	public TEnum Type { get; }
	public bool IsChangeable { get; }

	public void Enter();
	public void Exit();
	public void Update();
}

public abstract class UnitStateCon<TEnum> : MonoBehaviour where TEnum : struct,Enum
{
	public event Action<TEnum,TEnum> OnUnitStateChanged = null;

	[ShowInInspector] public TEnum StateType => m_state == null ? default : m_state.Type;

	private readonly Dictionary<TEnum,IUnitState<TEnum>> m_stateDict = new();

	protected IUnitState<TEnum> m_state = null;

	protected abstract bool _IsChangeable(TEnum newStateTag,bool isForce);

	public virtual void Initialize()
	{
		m_stateDict.Clear();
	}

	public virtual void Release()
	{
		m_stateDict.Clear();
	}

	public void EnterState(TEnum newType,bool isForce = false)
	{
		if(isActiveAndEnabled == false)
		{
			return;
		}

		if(!_IsChangeable(newType,isForce))
		{
			return;
		}

		if(!m_stateDict.TryGetValue(newType,out var state))
		{
			KZLogType.System.E($"{newType} state not found");

			return;
		}

		OnUnitStateChanged?.Invoke(StateType,newType);

		_ChangeState(state);
	}

	protected virtual void _ChangeState(IUnitState<TEnum> newState)
	{
		m_state?.Exit();

		m_state = newState;

		m_state.Enter();
	}

	private void Update()
	{
		m_state?.Update();
	}
}