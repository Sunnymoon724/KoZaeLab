using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Actors
{
	public abstract class Actor<TState,TStat> : MonoBehaviour where TState : struct,Enum where TStat : struct,Enum
	{
		[SerializeField]
		protected ActorAgentController m_actorAgentCon = null;

		private ActorStatController<TStat> m_actorStatCon = null;
		protected ActorStateController<TState> m_actorStateCon = null;

		[SerializeField,HideInInspector]
		private bool m_showLog = false;

		[ShowInInspector]
		protected bool ShowLog
		{
			get => m_showLog;
			set
			{
				m_showLog = value;

				_OnSetShowLog(value);
			}
		}

		protected virtual void _OnSetShowLog(bool value)
		{
			m_actorStateCon?.SetShowLog(value);
		}

		protected int m_currentLevel = 1;
		protected float m_currentHp = 0.0f;
		protected float m_maxHp = 0.0f;

		[ShowInInspector]
		protected string HpInfo => $"Hp: {CurrentHp}/{MaxHp}";

		[ShowInInspector]
		public TState StateType => m_actorStateCon == null ? DefaultStateType : m_actorStateCon.StateType;
		protected abstract TState DefaultStateType { get; }
		protected string m_lastStateName = string.Empty;

		public float CurrentHp => m_currentHp;
		public float MaxHp => m_maxHp;
		public bool IsDead => m_currentHp <= 0.0f;

		public SideType MySide { get; protected set; } = SideType.None;

		public bool IsEnemy(Actor<TState,TStat> other)
		{
			return _GetRelation(other) == SideRelationType.Enemy;
		}
		public bool IsAlly(Actor<TState,TStat> other)
		{
			return _GetRelation(other) == SideRelationType.Ally;
		}

		private SideRelationType _GetRelation(Actor<TState,TStat> other)
		{
			return SideRelationManager.In.GetRelation(MySide,other.MySide);
		}

		public Vector3 Position => transform.position;

		protected void _InitializeStat(StatProfile<TStat> statProfile)
		{
			m_actorStatCon = new ActorStatController<TStat>(this,statProfile,ShowLog);

			var maxHp = _GetStat(HpStatType);

			m_maxHp = maxHp;
			m_currentHp = maxHp;
		}

		protected float _GetStat(TStat statType)
		{
			return m_actorStatCon.GetStat(statType,m_currentLevel);
		}

		protected abstract TStat HpStatType { get; }
		protected abstract TStat MoveSpeedStatType { get; }

		protected float MoveSpeed => _GetStat(MoveSpeedStatType);

		public virtual void TakeDamage(float damage)
		{
			if(IsDead)
			{
				return;
			}

			m_currentHp = Mathf.Clamp(m_currentHp-_CalculateDamage(damage),0.0f,m_maxHp);

			if(IsDead)
			{
				_OnDead();
			}
		}

		protected virtual float _CalculateDamage(float damage)
		{
			return damage;
		}

		protected virtual void _OnDead() { }

		public virtual void Release()
		{
			m_actorStateCon?.Dispose();
		}
	}
}