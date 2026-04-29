using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Actors
{
	public interface IActor
	{
		Vector3 Position { get; }
		bool IsDead { get; }
		void TakeDamage(float damage);
	}

	public abstract class Actor<TState,TStat,TAgent> : MonoBehaviour,IActor where TState : struct,Enum where TStat : struct,Enum where TAgent : ActorAgentController
	{
		[SerializeField]
		protected TAgent m_agentCon = null;

		private ActorStatController<TStat> m_statCon = null;
		protected ActorStateController<TState> m_stateCon = null;

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
			m_stateCon?.SetShowLog(value);
		}

		protected int m_currentLevel = 1;
		protected float m_currentHp = 0.0f;
		protected float m_maxHp = 0.0f;

		[ShowInInspector]
		protected string HpInfo => $"Hp: {CurrentHp}/{MaxHp}";

		[ShowInInspector]
		public TState CurrentStateType => m_stateCon == null ? DefaultStateType : m_stateCon.StateType;
		protected abstract TState DefaultStateType { get; }
		protected string m_lastStateName = string.Empty;

		public float CurrentHp => m_currentHp;
		public float MaxHp => m_maxHp;
		public bool IsDead => m_currentHp <= 0.0f;

		public SideType MySide { get; protected set; } = SideType.None;

		public bool IsEnemy(Actor<TState,TStat,TAgent> other)
		{
			return _GetRelation(other) == SideRelationType.Enemy;
		}
		public bool IsAlly(Actor<TState,TStat,TAgent> other)
		{
			return _GetRelation(other) == SideRelationType.Ally;
		}

		private SideRelationType _GetRelation(Actor<TState,TStat,TAgent> other)
		{
			return SideRelationManager.In.GetRelation(MySide,other.MySide);
		}

		public Vector3 Position => transform.position;

		protected void _InitializeStat(StatProfile<TStat> statProfile)
		{
			m_statCon = new ActorStatController<TStat>(this,statProfile,ShowLog);

			var maxHp = _GetStat(HpStatType);

			m_maxHp = maxHp;
			m_currentHp = maxHp;
		}

		protected float _GetStat(TStat statType)
		{
			return m_statCon.GetStat(statType,m_currentLevel);
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

		protected abstract void _InitializeAgent(bool updateRotation);

		public virtual void Release()
		{
			m_stateCon?.Dispose();
		}
	}
}