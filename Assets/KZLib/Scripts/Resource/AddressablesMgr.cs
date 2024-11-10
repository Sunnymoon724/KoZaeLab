using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace KZLib
{
	public class AddressablesMgr : Singleton<AddressablesMgr>
	{
		private bool m_Disposed = false;

		private record AssetData(Object Asset,string Label);

		private readonly Dictionary<string,AssetData> m_AssetDataDict = new();

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				m_AssetDataDict.Clear();
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		public async UniTask<long> GetDownloadAssetSizeAsync(string _label)
		{
			var handle = Addressables.GetDownloadSizeAsync(_label);

			await handle;

			var size = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : 0L;

			Addressables.Release(handle);

			return size;
		}

		public async UniTask<bool> DownloadAssetAsync(string _label,Action<float,long,long> _onProgress = null)
		{
			var handle = Addressables.DownloadDependenciesAsync(_label);

			while(!handle.IsDone)
			{
				var status = handle.GetDownloadStatus();

				_onProgress?.Invoke(status.Percent,status.DownloadedBytes,status.TotalBytes);

				await UniTask.Yield();
			}

			var result = handle.Status == AsyncOperationStatus.Succeeded;
			var errorLog = string.Empty;

			if(handle.OperationException != null)
			{
				errorLog = handle.OperationException.Message;
			}

			Addressables.Release(handle);

			if(result)
			{
				return true;
			}
			else
			{
				if(!errorLog.IsEmpty())
				{
					LogTag.System.E(errorLog);
				}

				return false;
			}
		}

		public TObject GetObject<TObject>(string _path) where TObject : Object
		{
			if(!m_AssetDataDict.ContainsKey(_path))
			{
				LogTag.System.E($"Asset is not exist. [{_path}]");

				return null;
			}

			var data = m_AssetDataDict[_path];

			return data.Asset as TObject;
		}

		public TObject[] GetObjectArray<TObject>(string _path) where TObject : Object
		{
			var dataGroup = m_AssetDataDict.Where(x=>x.Key.Contains(_path));

			if(dataGroup.IsNullOrEmpty())
			{
				LogTag.System.E($"Asset is not exist. [{_path}]");

				return null;
			}

			return dataGroup.Select(x => x.Value.Asset as TObject).Where(y => y != null).ToArray();
		}

		public async UniTask LoadResourceAsync(string[] _labelArray,Action<float,float> _onProgress)
		{
			if(_labelArray.IsNullOrEmpty())
			{
				LogTag.System.E($"LabelArray is null or empty.");

				return;
			}

			var dataList = new List<(IResourceLocation,string)>();

			foreach(var label in _labelArray)
			{
				var handle = Addressables.LoadResourceLocationsAsync(label);

				await handle;

				if(handle.Status == AsyncOperationStatus.Failed)
				{
					throw handle.OperationException;
				}

				dataList.AddRange(handle.Result.Select(x => (x,label)));

				Addressables.Release(handle);
			}

			var totalCount = dataList.Count;

			for(var i=0;i<dataList.Count;i++)
			{
				var data = dataList[i];
				var key = data.Item1.InternalId;

				if(m_AssetDataDict.ContainsKey(key))
				{
					continue;
				}

				var handle = Addressables.LoadAssetAsync<Object>(data.Item1.PrimaryKey);

				await handle;

				if(handle.Status == AsyncOperationStatus.Failed)
				{
					throw handle.OperationException;
				}

				m_AssetDataDict.Add(key,new AssetData(handle.Result,data.Item2));

				_onProgress?.Invoke(i,totalCount);
			}
		}

		public void ReleaseResources(params string[] _labelArray)
		{
			foreach(var key in m_AssetDataDict.Where(x => _labelArray.Contains(x.Value.Label)).Select(y => y.Key))
			{
				Addressables.Release(m_AssetDataDict[key].Asset);
				m_AssetDataDict.Remove(key);
			}
		}
	}
}