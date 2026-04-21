using System;

namespace KZLib.Actors
{
	public abstract class Unit<TState,TStat> : Actor<TState,TStat> where TState : struct,Enum where TStat : struct,Enum
	{
		protected override float _CalculateDamage(float damage)
		{
			return damage*(100.0f/(100.0f+Defense));
		}

		protected abstract TStat AttackStatType { get; }
		protected abstract TStat DefenseStatType { get; }
		protected abstract TStat AttackRangeStatType { get; }

		protected float Attack => _GetStat(AttackStatType);
		protected float Defense => _GetStat(DefenseStatType);
		protected float AttackRange => _GetStat(AttackRangeStatType);
	}
}