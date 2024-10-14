using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace KZLib
{
	public interface IMetaData
	{
		int MetaId { get; }
		string Version { get; }
		bool IsExist { get; }
	}

	public class MetaDataMgr : DataSingleton<MetaDataMgr>
	{
		private const double FRAME_TIME = 1.0/30.0d; // 30 fps (0.0333s)
		private const int DELAY_TIME = 1; // 1ms

		private bool m_Loaded = false;

		// Type / MetaId / Data
		private readonly Dictionary<Type,Dictionary<int,IMetaData>> m_MetaDataDict = new();

		protected override void ClearAll()
		{
			m_MetaDataDict.Clear();

			m_Loaded = false;
		}

		public async UniTask LoadAllAsync(CancellationToken _token = default)
		{
			if(m_Loaded)
			{
				return;
			}

			var start = DateTime.Now;
			var assetArray = ResMgr.In.GetTextAssetArray(GameSettings.In.MetaDataFilePath);

			if(assetArray.IsNullOrEmpty())
			{
				return;
			}
			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();

			LogTag.Data.I("Meta Data Load Start");

			for(var i=0;i<assetArray.Length;i++)
			{
				stopwatch.Restart();

				LoadMetaFile(assetArray[i]);

				accumulatedTime += stopwatch.Elapsed.TotalSeconds;

				if(accumulatedTime >= FRAME_TIME)
				{
					accumulatedTime = 0.0d;

					await UniTask.Delay(DELAY_TIME,cancellationToken : _token);
				}
			}

			stopwatch.Stop();

			LogTag.Data.I($"Meta Data Load Complete [Count : {assetArray.Length} / Duration : {(DateTime.Now-start).TotalSeconds}]");

			m_Loaded = true;
		}

		public bool TryGetMetaData<TData>(int _metaId,out IMetaData _data)
		{
			return TryGetMetaData(_metaId,typeof(TData),out _data);
		}

		public bool TryGetMetaData(int _metaId,Type _type,out IMetaData _data)
		{
			_data = GetMetaData(_metaId,_type);

			return _data == default;
		}

		public TData GetMetaData<TData>(int _metaId)
		{
			return (TData) GetMetaData(_metaId,typeof(TData));
		}

		public IMetaData GetMetaData(int _metaId,Type _type)
		{
			if(_metaId <= 0)
			{
				throw new ArgumentException($"{_metaId} is not valid. {_metaId}");
			}

			if(m_MetaDataDict.TryGetValue(_type,out var dataDict))
			{
				if(dataDict.TryGetValue(_metaId,out var data) && data is IMetaData result)
				{
					return result;
				}
				else
				{
					throw new ArgumentException($"{_type.Name} is not include {_metaId}.");
				}
			}
			else
			{
				throw new ArgumentException($"{_type.Name} is not exist.");
			}
		}

		public IEnumerable<IMetaData> GetMetaDataGroup(Type _type)
		{
			if(m_MetaDataDict.TryGetValue(_type,out var dataDict))
			{
				foreach(var data in dataDict.Values)
				{
					yield return data;
				}
			}
		}

		public bool IsValidate<TData>(int _metaId) where TData : class,IMetaData
		{
			return _metaId > 0 && m_MetaDataDict.TryGetValue(typeof(TData),out var dataDict) && dataDict.ContainsKey(_metaId);
		}

		public int GetCount<TData>() where TData : class,IMetaData
		{
			return m_MetaDataDict.TryGetValue(typeof(TData),out var dataDict) ? dataDict.Count : 0;
		}

		private void LoadMetaFile(TextAsset _textAsset)
		{
			if(_textAsset == null)
			{
				return;
			}

			var type = ReflectionUtility.FindType($"MetaData.{_textAsset.name}");
			var deserialize = MessagePackSerializer.Deserialize(type.MakeArrayType(),_textAsset.bytes);

			if(deserialize is not object[] resultArray)
			{
				throw new NullReferenceException($"{_textAsset.name} is not array.");
			}

			var dataDict = new Dictionary<int,IMetaData>();

			foreach(var result in resultArray)
			{
				if(result is not IMetaData metaData)
				{
					continue;
				}

				if(!GameUtility.CheckVersion(metaData.Version) || !metaData.IsExist)
				{
					continue;
				}

				if(metaData.MetaId != -1 && dataDict.ContainsKey(metaData.MetaId))
				{
					continue;
				}

				dataDict.Add(metaData.MetaId,metaData);
			}

			m_MetaDataDict.Add(type,dataDict);
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
			var assetArray = ResMgr.In.GetTextAssetArray(GameSettings.In.MetaDataFilePath);

			if(assetArray.IsNullOrEmpty())
			{
				return;
			}

			for(var i=0;i<assetArray.Length;i++)
			{
				LoadMetaFile(assetArray[i]);
			}

			LogTag.Data.I($"Meta Data Load Complete [Count : {assetArray.Length} / Duration : {(DateTime.Now-start).TotalSeconds}]");

			m_Loaded = true;
		}
#endif
	}
}