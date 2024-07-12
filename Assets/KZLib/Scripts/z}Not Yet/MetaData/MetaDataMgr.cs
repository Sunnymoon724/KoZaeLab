using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using R3;
using UnityEngine;

namespace KZLib
{
	public interface IMetaData
	{
		int MetaId { get; }
		string Version { get; }
		string Name { get; }
		bool IsExist { get; }
	}

	/// <summary>
	/// 테이블 핸들러로 데이터 테이블을 받아서 MetaID를 Key로 저장한다.
	/// </summary>
	public class MetaDataMgr : DataSingleton<MetaDataMgr>
	{
		private const double FRAME_TIME = 1.0/30.0d; // 30 fps (약 0.0333초)
		private const int DELAY_TIME = 1; // 1ms

		private bool m_Loaded = false;

		//key는 타입 / Value [Key는 Metadata Id / Value는 데이터]
		private readonly Dictionary<Type,Dictionary<int,IMetaData>> m_MetaDataContainer = new();

		private readonly ReactiveProperty<int> m_LoadMaxCount = new();
		private readonly ReactiveProperty<int> m_LoadCount = new();

		protected override void ClearAll()
		{
			m_MetaDataContainer.Clear();

			m_Loaded = false;
		}

		public async UniTask LoadAllAsync(CancellationToken _token = default)
		{
			if(m_Loaded)
			{
				return;
			}

			var start = DateTime.Now;
			var assetArray = ResMgr.In.GetTextAssetArray(GameSettings.In.MetaAssetPath);

			if(assetArray.IsNullOrEmpty())
			{
				return;
			}

			m_LoadCount.Value = 0;
			m_LoadMaxCount.Value = assetArray.Length;

			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();

			LogTag.Data.I("메타 데이터 로드 시작");

			for(var i=0;i<assetArray.Length;i++)
			{
				var path = assetArray[i];
				stopwatch.Restart();

				LoadMetaFile(path);

				m_LoadCount.Value++;

				var elapsedTime = stopwatch.Elapsed.TotalSeconds;

				LogTag.Data.I("{0} 로드 시간 {1}",path,elapsedTime);

				accumulatedTime += elapsedTime;

				if(accumulatedTime >= FRAME_TIME)
				{
					accumulatedTime = 0.0d;

					await UniTask.Delay(DELAY_TIME,cancellationToken : _token);
				}
			}

			stopwatch.Stop();

			LogTag.Data.I("메타 데이터 로드 완료 [갯수 : {0} / 시간 : {1}]",assetArray.Length,(DateTime.Now-start).TotalSeconds);

			m_Loaded = true;

			//? 2차 가공
		}

		public bool TryGetMetaData<TData>(int _metaId,out IMetaData _data)
		{
			return TryGetMetaData(_metaId,typeof(TData),out _data);
		}

		public bool TryGetMetaData(int _metaId,Type _type,out IMetaData _data)
		{
			_data = Get(_metaId,_type);

			return _data == default;
		}

		public TData Get<TData>(int _metaId)
		{
			return (TData) Get(_metaId,typeof(TData));
		}

		public IMetaData Get(int _metaId,Type _type)
		{
			if(_metaId <= 0)
			{
				throw new ArgumentException(string.Format("{0}은 유효한 메타아이디가 아닙니다.",_metaId));
			}

			if(m_MetaDataContainer.TryGetValue(_type,out var dataDict))
			{
				if(dataDict.TryGetValue(_metaId,out var data) && data is IMetaData result)
				{
					return result;
				}
				else
				{
					throw new ArgumentException(string.Format("{0} 테이블 안에는 {1}가 없습니다.",_type.Name,_metaId));
				}
			}
			else
			{
				throw new ArgumentException(string.Format("{0}은 존재하지 않습니다.",_type.Name));
			}
		}

		public Dictionary<int,IMetaData>.Enumerator GetContainerIterator<TData>()
		{
			return GetContainerIterator(typeof(TData));
		}

		public Dictionary<int,IMetaData>.Enumerator GetContainerIterator(Type _type)
		{
			if(m_MetaDataContainer.TryGetValue(_type,out var dataDict))
			{
				return dataDict.GetEnumerator();
			}

			return new Dictionary<int,IMetaData>().GetEnumerator();
		}

		public bool IsValidate<TData>(int _metaId) where TData : class,IMetaData
		{
			return _metaId > 0 && m_MetaDataContainer.TryGetValue(typeof(TData),out var dataDict) && dataDict.ContainsKey(_metaId);
		}

		// public IEnumerable<TData> GetContainer<TData>() where TData : class,IMetaData
		// {
		// 	if(m_MetaDataContainer.TryGetValue(typeof(TData),out var dictionary))
		// 	{
		// 		var resultList = new List<TData>();

		// 		foreach(var value in dictionary.Values)
		// 		{
		// 			if(value is TData data)
		// 			{
		// 				resultList.Add(data);
		// 			}
		// 		}

		// 		return resultList;
		// 	}

		// 	return null;
		// }

		public int GetCount<TData>() where TData : class,IMetaData
		{
			return m_MetaDataContainer.TryGetValue(typeof(TData),out var dataDict) ? dataDict.Count : 0;
		}

		private void LoadMetaFile(TextAsset _asset)
		{
			var type = ReflectionUtility.FindType(string.Format("MetaData.{0}",_asset.name));
			var deserialize = MessagePackSerializer.Typeless.Deserialize(_asset.bytes);

			if(deserialize is not List<object> resultList)
			{
				throw new NullReferenceException(string.Format("{0}의 형태는 리스트가 아닙니다.",_asset.name));
			}

			var dataDict = new Dictionary<int,IMetaData>();

			foreach(var result in resultList)
			{
				if(result is not IMetaData data)
				{
					continue;
				}

				if(!CommonUtility.CheckVersion(data.Version) || !data.IsExist)
				{
					continue;
				}

				if(data.MetaId != -1 && dataDict.ContainsKey(data.MetaId))
				{
					continue;
				}

				dataDict.Add(data.MetaId,data);
			}

			m_MetaDataContainer.Add(type,dataDict);
		}

#if UNITY_EDITOR
		public void Reload()
		{
			ClearAll();

			Load_Editor();
		}

		public void Load_Editor()
		{
			if(m_Loaded)
			{
				return;
			}

			var start = DateTime.Now;
			var assetArray = ResMgr.In.GetTextAssetArray(GameSettings.In.MetaAssetPath);

			if(assetArray.IsNullOrEmpty())
			{
				return;
			}

			for(var i=0;i<assetArray.Length;i++)
			{
				LoadMetaFile(assetArray[i]);
			}

			LogTag.Data.I("메타 데이터 로드 완료 [갯수 : {0} / 시간 : {1}]",assetArray.Length,(DateTime.Now-start).TotalSeconds);

			m_Loaded = true;
		}
#endif
	}
}