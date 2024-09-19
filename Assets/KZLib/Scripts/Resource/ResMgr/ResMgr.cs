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

		private record CachedData
		{
			public Object[] DataArray { get; }

			private readonly long m_Duration = 0L;

			public CachedData(Object[] _dataArray,long _duration)
			{
				DataArray = _dataArray;
				m_Duration = _duration;
			}

			public bool IsOverdue => m_Duration < DateTime.Now.Ticks;
		}

		private bool m_Disposed = false;

		private const string RESOURCES = "Resources";
		private const float UPDATE_PERIOD = 0.1f;

		private const float POOL_LOOP_TIME = 30.0f;   // 30초
		private const double DEFAULT_DELETE_TIME = 60.0d;	// 60초

		private CancellationTokenSource m_TokenSource = null;

		private float m_PoolTimer = 0.0f;

		private readonly Queue<LoadingData> m_LoadingQueue = new();

		private readonly Dictionary<string,List<CachedData>> m_CachedDataDict = new();
		private readonly List<CachedData> m_RemoveList = new();

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

			UniTaskUtility.KillTokenSource(ref m_TokenSource);

			if(_disposing)
			{
				m_CachedDataDict.Clear();
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

				// 오브젝트 풀 정리
				{
					m_PoolTimer += UPDATE_PERIOD;

					if(m_PoolTimer >= POOL_LOOP_TIME)
					{
						m_PoolTimer = 0.0f;

						foreach(var pair in m_CachedDataDict)
						{
							pair.Value.RemoveAll(x => x.IsOverdue);

							if(pair.Value.Count == 0)
							{
								m_CachedDataDict.RemoveSafe(pair.Key);
							}
						}
					}
				}

				// 로딩 큐 체크하고 있으면 로딩 큐 실행
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

		private TObject GetData<TObject>(string _path) where TObject : Object
		{
			if(m_CachedDataDict.TryGetValue(_path,out var dataList) && dataList.Count > 0)
			{
				return dataList[0].DataArray[0] as TObject;
			}

			return null;
		}

		private TObject[] GetDataArray<TObject>(string _path) where TObject : Object
		{
			if(m_CachedDataDict.TryGetValue(_path,out var dataList) && dataList.Count > 0)
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
			if(!m_CachedDataDict.TryGetValue(_path,out var list))
			{
				list = new List<CachedData>();
				m_CachedDataDict.Add(_path,list);
			}

			list.Add(new CachedData(_objectArray,DateTime.Now.AddSeconds(DEFAULT_DELETE_TIME).Ticks));
		}
	}
}