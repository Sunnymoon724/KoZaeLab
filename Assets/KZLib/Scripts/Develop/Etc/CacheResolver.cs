using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace KZLib.KZDevelop
{
	public class CacheResolver<TCache>: IDisposable
	{
		private record CacheInfo
		{
			public TCache Cache { get; }
			public bool IsOverdue => m_duration < DateTime.Now.Ticks;

			private long m_duration = 0L;

			public CacheInfo(TCache cache,long duration)
			{
				Cache = cache;
				m_duration = duration;
			}
			
			public void UpdateDuration(long duration)
            {
                m_duration = duration;
            }
		}

		private readonly Dictionary<string,List<CacheInfo>> m_cacheInfoListDict = new();
		private readonly List<string> m_removeList = new();

		private readonly CancellationTokenSource m_tokenSource = null;

		private readonly float m_updatePeriod = 0.0f;
		private readonly float m_deleteTime = 0.0f;

		public CacheResolver(float deleteTime = 60.0f,float updatePeriod = 30.0f)
		{
			m_tokenSource = new();

			m_deleteTime = deleteTime;

			m_updatePeriod = updatePeriod;

			_LoopProcessAsync().Forget();
		}

		public void Dispose()
		{
			m_tokenSource?.Cancel(); 
			m_tokenSource?.Dispose(); 

			m_cacheInfoListDict.Clear();
			m_removeList.Clear();

			GC.SuppressFinalize(this);
		}

		public bool TryGetCache(string key,out TCache cache)
		{
			if(m_cacheInfoListDict.TryGetValue(key,out var cacheInfoList) && cacheInfoList.Count > 0)
			{
				var cacheInfo = cacheInfoList[0];
				cacheInfoList.RemoveAt(0);

				if(cacheInfoList.Count == 0)
				{
					m_cacheInfoListDict.Remove(key);
				}

				cache = cacheInfo.Cache;

				return true;
			}

			cache = default;

			return false;
		}

		public void StoreCache(string key,TCache cache,bool updateDuration)
		{
			long newDuration = DateTime.Now.AddSeconds(m_deleteTime).Ticks;

			if(!m_cacheInfoListDict.TryGetValue(key,out var cacheInfoList))
			{
				cacheInfoList = new List<CacheInfo>();

				m_cacheInfoListDict.Add(key,cacheInfoList);
			}
			else if(updateDuration)
			{
				foreach(var cacheInfo in cacheInfoList)
				{
					cacheInfo.UpdateDuration(newDuration);
				}
			}

			cacheInfoList.Add(new CacheInfo(cache,newDuration));
		}

		private async UniTaskVoid _LoopProcessAsync()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(m_updatePeriod),true,cancellationToken : m_tokenSource.Token);

				m_removeList.Clear();

				foreach(var pair in m_cacheInfoListDict)
				{
					static bool _IsOverdue(CacheInfo cacheInfo)
					{
						return cacheInfo.IsOverdue;
					}

					pair.Value.RemoveAll(_IsOverdue);

					if(pair.Value.Count == 0)
					{
						m_removeList.Add(pair.Key);
					}
				}

				foreach(var remove in m_removeList)
				{
					m_cacheInfoListDict.RemoveSafe(remove);
				}
			}
		}
	}
}