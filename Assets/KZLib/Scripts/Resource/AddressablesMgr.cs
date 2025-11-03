using System;
using System.Collections.Generic;
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
		private record LocationData(IResourceLocation Location,string Label);

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
			return await CommonUtility.LoadHandleSafeAsync(Addressables.GetDownloadSizeAsync(label));
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

			handle.Release();

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
			if(!m_assetDataDict.TryGetValue(path,out var result))
			{
				LogSvc.System.E($"Asset is not exist. [{path}]");

				return null;
			}

			return result.Asset as TObject;
		}

		public TObject[] ExtractObjectArray<TObject>(string path) where TObject : Object
		{
			var objectList = new List<TObject>();
			
			foreach(var pair in m_assetDataDict)
			{
				if(pair.Key.IsEqual(path))
				{
					if(pair.Value.Asset is TObject result)
					{
						objectList.Add(result);
					}
				}
			}

			if(objectList.IsNullOrEmpty())
			{
				LogSvc.System.E($"Asset is not exist. [{path}]");

				return Array.Empty<TObject>();
			}

			return objectList.ToArray();
		}

		public async UniTask LoadResourceAsync(string[] labelArray,Action<float,float> onUpdateProgress)
		{
			if(labelArray.IsNullOrEmpty())
			{
				LogSvc.System.E($"LabelArray is null or empty.");

				return;
			}

			var locationDataList = new List<LocationData>();

			foreach(var label in labelArray)
			{
				var locationList = await CommonUtility.LoadHandleSafeAsync(Addressables.LoadResourceLocationsAsync(label));
				
				foreach(var location in locationList)
                {
                    locationDataList.Add(new LocationData(location,label));
                }
			}

			var totalCount = locationDataList.Count;

			for(var i=0;i<locationDataList.Count;i++)
			{
				var locationData = locationDataList[i];
				var key = locationData.Location.InternalId;

				if(m_assetDataDict.ContainsKey(key))
				{
					continue;
				}

				var asset = await CommonUtility.LoadHandleSafeAsync(Addressables.LoadAssetAsync<Object>(locationData.Location.PrimaryKey));

				m_assetDataDict.Add(key,new AssetData(asset,locationData.Label));

				onUpdateProgress?.Invoke(i,totalCount);
			}
		}

		public void ReleaseResources(params string[] labelArray)
		{
			var keyList = new List<string>();

			foreach(var pair in m_assetDataDict)
			{
				var assetData = pair.Value;

				foreach(var label in labelArray)
				{
					if(assetData.Label.IsEqual(label))
					{
						keyList.Add(label);
					}
				}
			}

			foreach(var key in keyList)
			{
				Addressables.Release(m_assetDataDict[key].Asset);

				m_assetDataDict.Remove(key);
			}
		}
	}
}