using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		private record LoadingData(string DataPath,bool IsFilePath,Transform Parent);

		private record CacheData
		{
			public Object[] DataArray { get; }

			private readonly long m_Duration = 0L;

			public CacheData(Object[] _dataArray,long _duration)
			{
				DataArray = _dataArray;
				m_Duration = _duration;
			}

			public bool IsOverdue => m_Duration < DateTime.Now.Ticks;
		}

		private bool m_Disposed = false;

		private const string RESOURCES = "Resources";
		private const float UPDATE_PERIOD = 0.1f;

		private const float POOL_LOOP_TIME = 30.0f;   // 30s
		private const double DEFAULT_DELETE_TIME = 60.0d;	// 60s

		private CancellationTokenSource m_TokenSource = null;

		private float m_PoolTimer = 0.0f;

		private readonly Queue<LoadingData> m_LoadingQueue = new();

		private readonly Dictionary<string,List<CacheData>> m_CacheDataDict = new();

		protected override void Initialize()
		{
			m_TokenSource = new();

			LoopProcessAsync().Forget();
		}

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			CommonUtility.KillTokenSource(ref m_TokenSource);

			if(_disposing)
			{
				m_CacheDataDict.Clear();
				m_LoadingQueue.Clear();
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		private async UniTaskVoid LoopProcessAsync()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(UPDATE_PERIOD),true,cancellationToken : m_TokenSource.Token);

				// Check ObjectPool 
				{
					m_PoolTimer += UPDATE_PERIOD;

					if(m_PoolTimer >= POOL_LOOP_TIME)
					{
						m_PoolTimer = 0.0f;

						var removeList = new List<string>();

						foreach(var pair in m_CacheDataDict)
						{
							pair.Value.RemoveAll(x => x.IsOverdue);

							if(pair.Value.Count == 0)
							{
								removeList.Add(pair.Key);
							}
						}

						foreach(var remove in removeList)
						{
							m_CacheDataDict.RemoveSafe(remove);
						}
					}
				}

				// Set Loading Queue
				if(m_LoadingQueue.Count > 0)
				{
					var loadingData = m_LoadingQueue.Dequeue();

					GetObject(loadingData.DataPath,loadingData.Parent,true);
				}
			}
		}

		private void AddLoadingQueue(string _path,bool _isFilePath,Transform _parent = null)
		{
			m_LoadingQueue.Enqueue(new LoadingData(_path,_isFilePath,_parent));
		}

		private TObject GetCacheData<TObject>(string _path) where TObject : Object
		{
			var dataArray = GetCacheDataArray<TObject>(_path);

			return dataArray?.Length > 0 ? dataArray[0] : null;
		}

		private TObject[] GetCacheDataArray<TObject>(string _path) where TObject : Object
		{
			if(m_CacheDataDict.TryGetValue(_path,out var dataList) && dataList.Count > 0)
			{
				var data = dataList[0];

				if(data.DataArray is TObject[] resultArray)
				{
					return resultArray;
				}
			}

			return null;
		}

		private void PutData<TObject>(string _path,TObject _object) where TObject : Object
		{
			PutDataArray(_path,new TObject[] { _object });
		}

		private void PutDataArray<TObject>(string _path,TObject[] _objectArray) where TObject : Object
		{
			if(!m_CacheDataDict.TryGetValue(_path,out var dataList))
			{
				dataList = new List<CacheData>();
				m_CacheDataDict.Add(_path,dataList);
			}

			dataList.Add(new CacheData(_objectArray,DateTime.Now.AddSeconds(DEFAULT_DELETE_TIME).Ticks));
		}
	}
}