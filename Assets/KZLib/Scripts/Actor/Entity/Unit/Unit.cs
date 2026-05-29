using System;
using UnityEngine;

namespace KZLib.Actors
{
	public abstract class Unit<TState,TStat> : Actor<TState,TStat> where TState : struct,Enum where TStat : struct,Enum
	{
		[SerializeField]
		protected UnitAgentController m_agentCon = null;

		protected abstract TStat MoveSpeedStatType { get; }

		protected float MoveSpeed => _GetStat(MoveSpeedStatType);

		protected void _InitializeAgent(bool updateRotation = false)
		{
			m_agentCon.Initialize(AttackRange,MoveSpeed,updateRotation);
		}
	}
}