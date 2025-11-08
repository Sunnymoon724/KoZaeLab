using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace KZLib.KZDevelop
{
	public class CacheResolver<TCache>: IDisposable
	{
		private record CacheEntry
		{
			public TCache Cache { get; }
			public bool IsOverdue => m_duration < DateTime.Now.Ticks;

			private long m_duration = 0L;

			public CacheEntry(TCache cache,long duration)
			{
				Cache = cache;
				m_duration = duration;
			}
			
			public void UpdateDuration(long duration)
            {
                m_duration = duration;
            }
		}

		private readonly Dictionary<string,List<CacheEntry>> m_cacheListDict = new();
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

			m_cacheListDict.Clear();
			m_removeList.Clear();

			GC.SuppressFinalize(this);
		}

		public bool TryGetCache(string key,out TCache cache)
		{
			if(m_cacheListDict.TryGetValue(key,out var list) && list.Count > 0)
			{
				var entry = list[0];
				list.RemoveAt(0);

				if(list.Count == 0)
				{
					m_cacheListDict.Remove(key);
				}

				cache = entry.Cache;

				return true;
			}

			cache = default;

			return false;
		}

		public void StoreCache(string key,TCache cache,bool updateDuration)
		{
			long newDuration = DateTime.Now.AddSeconds(m_deleteTime).Ticks;

			if(!m_cacheListDict.TryGetValue(key, out var list))
			{
				list = new List<CacheEntry>();

				m_cacheListDict.Add(key, list);
			}
			else if(updateDuration)
			{
				foreach(var entry in list)
				{
					entry.UpdateDuration(newDuration);
				}
			}

			list.Add(new CacheEntry(cache,newDuration));
		}

		private async UniTaskVoid _LoopProcessAsync()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(m_updatePeriod),true,cancellationToken : m_tokenSource.Token);

				m_removeList.Clear();

				foreach(var pair in m_cacheListDict)
				{
					pair.Value.RemoveAll(x => x.IsOverdue);

					if(pair.Value.Count == 0)
					{
						m_removeList.Add(pair.Key);
					}
				}

				foreach(var remove in m_removeList)
				{
					m_cacheListDict.RemoveSafe(remove);
				}
			}
		}
	}
}