using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	public partial class EffectMgr : AutoSingletonMB<EffectMgr>
	{
		private readonly Dictionary<string,List<PoolData>> m_PoolDataDict = new();

		private void PutClip(EffectClip _effect)
		{
			_effect.transform.ResetTransform(transform);
			_effect.gameObject.SetActiveSelf(false);

			m_PoolDataDict.AddOrCreate(_effect.name,new PoolData(_effect));
		}

		private EffectClip GetClip(string _name,Transform _parent)
		{
			if(m_PoolDataDict.TryGetValue(_name,out var dataList))
			{
				if(dataList.IsNullOrEmpty())
				{
					return null;
				}

				var data = dataList[0];

				dataList.RemoveAt(0);

				data.Effect.transform.parent = _parent;

				return data.Effect;
			}

			return null;
		}

		private record PoolData
		{
			private const float DEFAULT_DELETE_TIME = 60.0f;   // 60초

			public EffectClip Effect { get; }

			private readonly long m_Duration = 0L;

			public PoolData(EffectClip _effect)
			{
				m_Duration = DateTime.Now.AddSeconds(DEFAULT_DELETE_TIME).Ticks;
				Effect = _effect;
			}

			public bool IsOverdue()
			{
				return m_Duration < DateTime.Now.Ticks;
			}

			public void Release()
			{
				if(Effect)
				{
					UnityUtility.DestroyObject(Effect.gameObject);
				}
			}
		}
	}
}