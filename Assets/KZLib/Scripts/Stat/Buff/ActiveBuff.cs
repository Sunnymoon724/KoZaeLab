using System;
using KZLib.Data;

namespace KZLib
{
	public class ActiveBuff
	{
		private readonly IBuffProto m_buffProto = null;
		private readonly bool m_isPermanent = false;

		private int m_stackCount = 1;
		private float m_remainingTime = 0.0f;

		public string BuffName => m_buffProto.BuffName;
		public float Duration => m_buffProto.Duration;
		public int MaxStackCount => m_buffProto.MaxStackCount;
		public BuffEntry[] BuffEntryArray => m_buffProto.BuffEntryArray;

		public int CurrentStackCount => m_stackCount;
		public float CurrentRemainingTime => m_remainingTime;

		public bool IsExpired => !m_isPermanent && m_remainingTime <= 0.0f;

		public ActiveBuff(int buffNum)
		{
			m_buffProto = ProtoManager.In.GetProto<IBuffProto>(buffNum) ?? throw new NullReferenceException($"Buff Proto Not Found : {buffNum}");

			m_isPermanent = m_buffProto.Duration <= 0.0f;
			m_remainingTime = m_buffProto.Duration;
			m_stackCount = 1;
		}

		public bool TickTime(float deltaTime)
		{
			m_remainingTime = Math.Max(0.0f,m_remainingTime-deltaTime);

			return IsExpired;
		}

		public bool TryAddStack()
		{
			if(m_stackCount >= MaxStackCount)
			{
				return false;
			}

			m_stackCount++;

			return true;
		}

		public bool TryReduceStack()
		{
			if(m_stackCount <= 1)
			{
				return false;
			}

			m_stackCount--;

			return true;
		}
	}
}
