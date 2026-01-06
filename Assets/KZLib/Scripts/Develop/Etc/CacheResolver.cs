using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace KZLib.KZDevelop
{
	public sealed class CacheResolver<TCache>: IDisposable
	{
		private record CacheInfo
		{
			public TCache Cache { get; }
			public bool IsOverdue => m_duration < GameTimeManager.In.GetCurrentTime(true).Ticks;

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

		private CancellationTokenSource m_tokenSource = null;

		private readonly float m_updatePeriod = 0.0f;
		private readonly float m_deleteTime = 0.0f;

		private bool m_disposed = false;

		public CacheResolver(float deleteTime = 60.0f,float updatePeriod = 30.0f)
		{
			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			m_deleteTime = deleteTime;

			m_updatePeriod = updatePeriod;

			_LoopProcessAsync(m_tokenSource.Token).Forget();
		}

		~CacheResolver()
		{
			_Dispose(false);
		}

		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void _Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				CommonUtility.KillTokenSource(ref m_tokenSource);

				m_cacheInfoListDict.Clear();
				m_removeList.Clear();
			}

			m_disposed = true;
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

		public void StoreCache(string key,TCache cache,bool isUpdate)
		{
			var currentTime = GameTimeManager.In.GetCurrentTime(true);
			var newDuration = currentTime.AddSeconds(m_deleteTime).Ticks;

			if(!m_cacheInfoListDict.TryGetValue(key,out var cacheInfoList))
			{
				cacheInfoList = new List<CacheInfo>();

				m_cacheInfoListDict.Add(key,cacheInfoList);
			}
			else if(isUpdate)
			{
				for(var i=0;i<cacheInfoList.Count;i++)
				{
					cacheInfoList[i].UpdateDuration(newDuration);
				}
			}

			cacheInfoList.Add(new CacheInfo(cache,newDuration));
		}

		private async UniTaskVoid _LoopProcessAsync(CancellationToken token)
		{
			void _CheckCache()
			{
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

				for(var i=0;i<m_removeList.Count;i++)
				{
					m_cacheInfoListDict.RemoveSafe(m_removeList[i]);
				}
			}

			await CommonUtility.LoopActionAndWaitForSecondAsync(_CheckCache,m_updatePeriod,true,-1,token);
		}
	}
}