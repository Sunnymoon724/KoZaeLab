using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using KZLib.Utilities;

using Object = UnityEngine.Object;

namespace KZLib
{
	public class AddressablesManager : Singleton<AddressablesManager>
	{
		private record AssetInfo(Object Asset,string Label);
		private record LocationInfo(IResourceLocation Location,string Label);

		private readonly Dictionary<string,AssetInfo> m_assetInfoDict = new();

		private AddressablesManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_assetInfoDict.Clear();
			}

			base._Release(disposing);
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
					LogChannel.System.E(errorLog);
				}

				return false;
			}
		}

		public TObject GetObject<TObject>(string path) where TObject : Object
		{
			if(!m_assetInfoDict.TryGetValue(path,out var result))
			{
				LogChannel.System.E($"Asset is not exist. [{path}]");

				return null;
			}

			return result.Asset as TObject;
		}

		public TObject[] ExtractObjectArray<TObject>(string path) where TObject : Object
		{
			var objectList = new List<TObject>();
			
			foreach(var pair in m_assetInfoDict)
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
				LogChannel.System.E($"Asset is not exist. [{path}]");

				return Array.Empty<TObject>();
			}

			return objectList.ToArray();
		}

		public async UniTask LoadResourceAsync(string[] labelArray,Action<float,float> onUpdateProgress)
		{
			if(labelArray.IsNullOrEmpty())
			{
				LogChannel.System.E($"LabelArray is null or empty.");

				return;
			}

			var locationInfoList = new List<LocationInfo>();

			for(var i=0;i<labelArray.Length;i++)
			{
				var label = labelArray[i];
				var locationList = await CommonUtility.LoadHandleSafeAsync(Addressables.LoadResourceLocationsAsync(label));

				for(var j=0;j<locationList.Count;j++)
                {
					locationInfoList.Add(new LocationInfo(locationList[j],label));
                }
			}

			var totalCount = locationInfoList.Count;

			for(var i=0;i<locationInfoList.Count;i++)
			{
				var locationInfo = locationInfoList[i];
				var key = locationInfo.Location.InternalId;

				if(m_assetInfoDict.ContainsKey(key))
				{
					continue;
				}

				var asset = await CommonUtility.LoadHandleSafeAsync(Addressables.LoadAssetAsync<Object>(locationInfo.Location.PrimaryKey));

				m_assetInfoDict.Add(key,new AssetInfo(asset,locationInfo.Label));

				onUpdateProgress?.Invoke(i,totalCount);
			}
		}

		public void ReleaseResources(params string[] labelArray)
		{
			var keyList = new List<string>();

			foreach(var pair in m_assetInfoDict)
			{
				var assetInfo = pair.Value;

				for(var i=0;i<labelArray.Length;i++)
				{
					var label = labelArray[i];

					if(assetInfo.Label.IsEqual(label))
					{
						keyList.Add(label);
					}
				}
			}

			for(var i=0;i<keyList.Count;i++)
			{
				var key = keyList[i];

				Addressables.Release(m_assetInfoDict[key].Asset);

				m_assetInfoDict.Remove(key);
			}
		}
	}
}