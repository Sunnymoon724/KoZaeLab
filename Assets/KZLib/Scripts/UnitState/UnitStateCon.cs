using System;
using System.Collections.Generic;
using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitStateTag : CustomTag
{
	public UnitStateTag(string name) : base(name) { }
}

public interface IUnitState
{
	public UnitStateTag StateTag { get; }

	public void Enter();
	public void Exit();
	public void Update();
}

public abstract class UnitStateCon : MonoBehaviour
{
	public event Action<UnitStateTag,UnitStateTag> OnUnitStateChanged = null;

	[ShowInInspector] public UnitStateTag StateTag => m_state?.StateTag;

	private readonly Dictionary<UnitStateTag,IUnitState> m_stateDict = new();

	protected IUnitState m_state = null;

	protected abstract bool IsChangeable(UnitStateTag newStateTag,bool isForce);

	public virtual void Initialize()
	{
		m_stateDict.Clear();

		foreach(var stateTag in CustomTag.CollectCustomTagList<UnitStateTag>(true))
		{
			if(Activator.CreateInstance(Type.GetType(stateTag.Name)) is not IUnitState state)
			{
				continue;
			}

			m_stateDict.Add(state.StateTag,state);
		}
	}

	public virtual void Release()
	{
		m_stateDict.Clear();
	}

	public void EnterState(UnitStateTag newStateTag,bool isForce = false)
	{
		if(isActiveAndEnabled == false)
		{
			return;
		}

		if(!IsChangeable(newStateTag,isForce))
		{
			return;
		}

		if(!m_stateDict.TryGetValue(newStateTag,out var state))
		{
			LogTag.System.E($"{newStateTag} state not found");

			return;
		}

		OnUnitStateChanged?.Invoke(StateTag,newStateTag);

		_ChangeState(state);
	}

	protected virtual void _ChangeState(IUnitState newState)
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