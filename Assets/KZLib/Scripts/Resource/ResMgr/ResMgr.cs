using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using KZLib.KZUtility;
using Object = UnityEngine.Object;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		private record LoadingData(string DataPath,bool IsFilePath,Transform Parent);

		private record CacheData
		{
			public Object[] CacheArray { get; }

			private readonly long m_duration = 0L;

			public CacheData(Object[] cacheArray,long duration)
			{
				CacheArray = cacheArray;
				m_duration = duration;
			}

			public bool IsOverdue => m_duration < DateTime.Now.Ticks;
		}

		private bool m_disposed = false;

		private const string RESOURCES = "Resources";
		private const float UPDATE_PERIOD = 0.1f;

		private const float POOL_LOOP_TIME = 30.0f;   // 30s
		private const double DEFAULT_DELETE_TIME = 60.0d;	// 60s

		private CancellationTokenSource m_tokenSource = null;

		private float m_PoolTimer = 0.0f;

		private readonly Queue<LoadingData> m_loadingQueue = new();

		private readonly Dictionary<string,List<CacheData>> m_cacheDataDict = new();

		private bool m_useServerResource = false;

		protected override void Initialize()
		{
			m_tokenSource = new();

			var gameConfig = ConfigManager.In.Access<ConfigData.GameConfig>();

			m_useServerResource = !gameConfig.IsLocalResource;

			LoopProcessAsync().Forget();
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			CommonUtility.KillTokenSource(ref m_tokenSource);

			if(disposing)
			{
				m_cacheDataDict.Clear();
				m_loadingQueue.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private async UniTaskVoid LoopProcessAsync()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(UPDATE_PERIOD),true,cancellationToken : m_tokenSource.Token);

				// Check ObjectPool 
				{
					m_PoolTimer += UPDATE_PERIOD;

					if(m_PoolTimer >= POOL_LOOP_TIME)
					{
						m_PoolTimer = 0.0f;

						var removeList = new List<string>();

						foreach(var pair in m_cacheDataDict)
						{
							pair.Value.RemoveAll(x => x.IsOverdue);

							if(pair.Value.Count == 0)
							{
								removeList.Add(pair.Key);
							}
						}

						foreach(var remove in removeList)
						{
							m_cacheDataDict.RemoveSafe(remove);
						}
					}
				}

				// Set Loading Queue
				if(m_loadingQueue.Count > 0)
				{
					var loadingData = m_loadingQueue.Dequeue();

					GetObject(loadingData.DataPath,loadingData.Parent,true);
				}
			}
		}

		private void AddLoadingQueue(string path,bool isFilePath,Transform parent = null)
		{
			m_loadingQueue.Enqueue(new LoadingData(path,isFilePath,parent));
		}

		private TObject GetCacheData<TObject>(string path) where TObject : Object
		{
			var dataArray = GetCacheDataArray<TObject>(path);

			return dataArray?.Length > 0 ? dataArray[0] : null;
		}

		private TObject[] GetCacheDataArray<TObject>(string path) where TObject : Object
		{
			if(m_cacheDataDict.TryGetValue(path,out var dataList) && dataList.Count > 0)
			{
				var data = dataList[0];

				if(data.CacheArray is TObject[] resultArray)
				{
					return resultArray;
				}
			}

			return null;
		}

		private void PutData<TObject>(string path,TObject cache) where TObject : Object
		{
			PutDataArray(path,new TObject[] { cache });
		}

		private void PutDataArray<TObject>(string path,TObject[] cacheArray) where TObject : Object
		{
			if(!m_cacheDataDict.TryGetValue(path,out var dataList))
			{
				dataList = new List<CacheData>();
				m_cacheDataDict.Add(path,dataList);
			}

			dataList.Add(new CacheData(cacheArray,DateTime.Now.AddSeconds(DEFAULT_DELETE_TIME).Ticks));
		}
	}
}