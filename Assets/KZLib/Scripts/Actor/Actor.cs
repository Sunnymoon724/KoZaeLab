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

		SideType MySide { get; }
	}

	public abstract class Actor<TState,TStat,TAgent> : MonoBehaviour,IActor where TState : struct,Enum where TStat : struct,Enum where TAgent : ActorAgentController
	{
		#region Debug
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
		#endregion Debug

		#region State
		protected ActorStateController<TState> m_stateCon = null;
		protected string m_lastStateName = string.Empty;

		[ShowInInspector]
		public TState CurrentStateType => m_stateCon == null ? DefaultStateType : m_stateCon.StateType;
		protected abstract TState DefaultStateType { get; }
		#endregion State

		#region Stat
		private ActorStatController<TStat> m_statCon = null;
		protected int m_currentLevel = 1;

		protected abstract TStat HpStatType { get; }
		protected abstract TStat MoveSpeedStatType { get; }

		protected float MoveSpeed => _GetStat(MoveSpeedStatType);

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

		protected float m_currentHp = 0.0f;
		protected float m_maxHp = 0.0f;

		public float CurrentHp => m_currentHp;
		public float MaxHp => m_maxHp;
		public bool IsDead => m_currentHp <= 0.0f;

		[ShowInInspector]
		protected string HpInfo => $"Hp: {CurrentHp}/{MaxHp}";

		public Vector3 Position => transform.position;

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
		#endregion Stat

		#region Side
		private SideType m_mySide = SideType.None;
		public SideType MySide => m_mySide;

		public void SetSideType(SideType sideType)
		{
			m_mySide = sideType;
		}

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
		#endregion Side

		#region Agent
		[SerializeField]
		protected TAgent m_agentCon = null;

		protected abstract void _InitializeAgent(bool updateRotation = false);
		#endregion Agent

		#region Lifecycle
		public virtual void Release()
		{
			m_stateCon?.Dispose();
		}
		#endregion Lifecycle
	}
}