using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using KZLib.KZUtility;
using Object = UnityEngine.Object;

namespace KZLib
{
	public class AddressablesMgr : Singleton<AddressablesMgr>
	{
		private bool m_disposed = false;

		private record AssetData(Object Asset,string Label);

		private readonly Dictionary<string,AssetData> m_assetDataDict = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_assetDataDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public async UniTask<long> GetDownloadAssetSizeAsync(string label)
		{
			var handle = Addressables.GetDownloadSizeAsync(label);

			await handle;

			var size = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : 0L;

			Addressables.Release(handle);

			return size;
		}

		public async UniTask<bool> DownloadAssetAsync(string label,Action<float,long,long> onUpdateProgress = null)
		{
			var handle = Addressables.DownloadDependenciesAsync(label);

			while(!handle.IsDone)
			{
				var status = handle.GetDownloadStatus();

				onUpdateProgress?.Invoke(status.Percent,status.DownloadedBytes,status.TotalBytes);

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
					LogSvc.System.E(errorLog);
				}

				return false;
			}
		}

		public TObject GetObject<TObject>(string path) where TObject : Object
		{
			if(!m_assetDataDict.ContainsKey(path))
			{
				LogSvc.System.E($"Asset is not exist. [{path}]");

				return null;
			}

			var data = m_assetDataDict[path];

			return data.Asset as TObject;
		}

		public TObject[] GetObjectArray<TObject>(string path) where TObject : Object
		{
			var dataGroup = m_assetDataDict.Where(x=>x.Key.Contains(path));

			if(dataGroup.IsNullOrEmpty())
			{
				LogSvc.System.E($"Asset is not exist. [{path}]");

				return null;
			}

			return dataGroup.Select(x => x.Value.Asset as TObject).Where(y => y != null).ToArray();
		}

		public async UniTask LoadResourceAsync(string[] labelArray,Action<float,float> onUpdateProgress)
		{
			if(labelArray.IsNullOrEmpty())
			{
				LogSvc.System.E($"LabelArray is null or empty.");

				return;
			}

			var dataList = new List<(IResourceLocation,string)>();

			foreach(var label in labelArray)
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

				if(m_assetDataDict.ContainsKey(key))
				{
					continue;
				}

				var handle = Addressables.LoadAssetAsync<Object>(data.Item1.PrimaryKey);

				await handle;

				if(handle.Status == AsyncOperationStatus.Failed)
				{
					throw handle.OperationException;
				}

				m_assetDataDict.Add(key,new AssetData(handle.Result,data.Item2));

				onUpdateProgress?.Invoke(i,totalCount);
			}
		}

		public void ReleaseResources(params string[] labelArray)
		{
			foreach(var key in m_assetDataDict.Where(x => labelArray.Contains(x.Value.Label)).Select(y => y.Key))
			{
				Addressables.Release(m_assetDataDict[key].Asset);
				m_assetDataDict.Remove(key);
			}
		}
	}
}