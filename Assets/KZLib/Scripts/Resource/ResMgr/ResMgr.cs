﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using KZLib.KZUtility;
using Object = UnityEngine.Object;
using KZLib.KZData;

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

		private const float c_update_period = 0.1f;

		private const float c_pool_loop_time = 30.0f;   // 30s
		private const double c_delete_time = 60.0d;	// 60s

		private CancellationTokenSource m_tokenSource = null;

		private float m_poolTimer = 0.0f;

		private readonly Queue<LoadingData> m_loadingQueue = new();

		private readonly Dictionary<string,List<CacheData>> m_cacheDataDict = new();
		private readonly List<string> m_removeList = new();

		private bool m_useServerResource = false;

		protected override void Initialize()
		{
			m_tokenSource = new();

			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			m_useServerResource = !gameCfg.IsLocalResource;

			_LoopProcessAsync().Forget();
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

		private async UniTaskVoid _LoopProcessAsync()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(c_update_period),true,cancellationToken : m_tokenSource.Token);

				// Check ObjectPool 
				{
					m_poolTimer += c_update_period;

					if(m_poolTimer >= c_pool_loop_time)
					{
						m_poolTimer = 0.0f;

						m_removeList.Clear();

						foreach(var pair in m_cacheDataDict)
						{
							pair.Value.RemoveAll(x => x.IsOverdue);

							if(pair.Value.Count == 0)
							{
								m_removeList.Add(pair.Key);
							}
						}

						foreach(var remove in m_removeList)
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

		private void _AddLoadingQueue(string path,bool isFilePath,Transform parent = null)
		{
			m_loadingQueue.Enqueue(new LoadingData(path,isFilePath,parent));
		}

		private TObject _GetCacheData<TObject>(string path) where TObject : Object
		{
			var dataArray = _GetCacheDataArray<TObject>(path);

			return dataArray?.Length > 0 ? dataArray[0] : null;
		}

		private TObject[] _GetCacheDataArray<TObject>(string path) where TObject : Object
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

		private void _PutData<TObject>(string path,TObject cache) where TObject : Object
		{
			_PutDataArray(path,new TObject[] { cache });
		}

		private void _PutDataArray<TObject>(string path,TObject[] cacheArray) where TObject : Object
		{
			if(!m_cacheDataDict.TryGetValue(path,out var dataList))
			{
				dataList = new List<CacheData>();
				m_cacheDataDict.Add(path,dataList);
			}

			dataList.Add(new CacheData(cacheArray,DateTime.Now.AddSeconds(c_delete_time).Ticks));
		}
	}
}