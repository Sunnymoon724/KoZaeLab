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
	/// <summary>
	/// Addressables-backed asset cache used when <see cref="GameConfig.IsLocalResource"/> is false.
	/// <list type="bullet">
	/// <item><description><see cref="LoadResourceAsync"/> preloads assets by Addressables label into an internal dictionary.</description></item>
	/// <item><description><see cref="ResourceManager"/> resolves non-<c>Resources/</c> paths through <see cref="GetObject{TObject}"/> and <see cref="ExtractObjectArray{TObject}"/>.</description></item>
	/// <item><description>Dictionary keys are normalized <c>Assets/...</c> paths so they match <see cref="ResourceManager"/> lookups.</description></item>
	/// <item><description><see cref="ReleaseResources"/> unloads assets by the label stored at load time (e.g. scene name on scene unload).</description></item>
	/// </list>
	/// </summary>
	public class AddressablesManager : Singleton<AddressablesManager>
	{
		/// <summary>Loaded source asset and the Addressables label that loaded it.</summary>
		private record AssetInfo(Object Asset,string Label);

		/// <summary>Resolved Addressables location and the label used to find it.</summary>
		private record LocationInfo(IResourceLocation Location,string Label);

		/// <summary>Preloaded source assets keyed by <c>Assets/...</c> path.</summary>
		private readonly Dictionary<string,AssetInfo> m_assetInfoDict = new();

		private AddressablesManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				foreach(var pair in m_assetInfoDict)
				{
					Addressables.Release(pair.Value.Asset);
				}

				m_assetInfoDict.Clear();
			}

			base._Release(disposing);
		}

		/// <summary>Returns the download size in bytes for all dependencies of <paramref name="label"/>.</summary>
		public async UniTask<long> GetDownloadAssetSizeAsync(string label)
		{
			return await KZExternalKit.LoadHandleSafeAsync(Addressables.GetDownloadSizeAsync(label));
		}

		/// <summary>
		/// Downloads remote dependencies for <paramref name="label"/> without loading assets into the cache.
		/// Call <see cref="LoadResourceAsync"/> afterward when assets should be available to <see cref="ResourceManager"/>.
		/// </summary>
		/// <param name="onUpdateProgress">Reports percent, downloaded bytes, and total bytes while downloading.</param>
		public async UniTask<bool> DownloadAssetAsync(string label,Action<float,long,long> onUpdateProgress = null)
		{
			var handle = Addressables.DownloadDependenciesAsync(label);

			try
			{
				if(onUpdateProgress != null)
				{
					await UniTask.WhenAll(
						handle.ToUniTask(),
						_MonitorDownloadProgressAsync(handle,onUpdateProgress)
					);
				}
				else
				{
					await handle.ToUniTask();
				}

				if(handle.Status != AsyncOperationStatus.Succeeded)
				{
					if(handle.OperationException != null)
					{
						LogChannel.Resource.E(handle.OperationException.Message);
					}

					return false;
				}

				return true;
			}
			finally
			{
				handle.Release();
			}
		}

		/// <summary>
		/// Returns a preloaded source asset for <paramref name="path"/>.
		/// <paramref name="path"/> must be a normalized <c>Assets/...</c> file path.
		/// </summary>
		public TObject GetObject<TObject>(string path) where TObject : Object
		{
			path = _NormalizeAssetPath(path);

			if(!m_assetInfoDict.TryGetValue(path,out var result))
			{
				LogChannel.Resource.E($"Asset does not exist. [{path}]");

				return null;
			}

			return result.Asset as TObject;
		}

		/// <summary>
		/// Returns preloaded source assets whose paths are under <paramref name="path"/>.
		/// <paramref name="path"/> must be a normalized <c>Assets/...</c> folder path.
		/// </summary>
		public TObject[] ExtractObjectArray<TObject>(string path) where TObject : Object
		{
			path = _NormalizeAssetPath(path);

			var objectList = new List<TObject>();

			foreach(var pair in m_assetInfoDict)
			{
				if(!_IsAssetUnderFolder(pair.Key,path))
				{
					continue;
				}

				if(pair.Value.Asset is TObject result)
				{
					objectList.Add(result);
				}
			}

			if(objectList.IsNullOrEmpty())
			{
				LogChannel.Resource.E($"Asset does not exist. [{path}]");

				return Array.Empty<TObject>();
			}

			return objectList.ToArray();
		}

		/// <summary>
		/// Loads all Addressables locations for <paramref name="labelArray"/> and stores source assets in the cache.
		/// Assets already present in the cache are skipped.
		/// </summary>
		/// <param name="onUpdateProgress">Reports completed count and total location count after each newly loaded asset.</param>
		public async UniTask LoadResourceAsync(string[] labelArray,Action<float,float> onUpdateProgress)
		{
			if(labelArray.IsNullOrEmpty())
			{
				LogChannel.Resource.E($"LabelArray is null or empty.");

				return;
			}

			var locationInfoList = new List<LocationInfo>();

			for(var i=0;i<labelArray.Length;i++)
			{
				var label = labelArray[i];
				var locationList = await KZExternalKit.LoadHandleSafeAsync(Addressables.LoadResourceLocationsAsync(label));

				for(var j=0;j<locationList.Count;j++)
				{
					locationInfoList.Add(new LocationInfo(locationList[j],label));
				}
			}

			var totalCount = locationInfoList.Count;

			for(var i=0;i<locationInfoList.Count;i++)
			{
				var locationInfo = locationInfoList[i];
				var key = _GetAssetPathKey(locationInfo.Location);

				if(m_assetInfoDict.ContainsKey(key))
				{
					continue;
				}

				var asset = await KZExternalKit.LoadHandleSafeAsync(Addressables.LoadAssetAsync<Object>(locationInfo.Location));

				m_assetInfoDict.Add(key,new AssetInfo(asset,locationInfo.Label));

				onUpdateProgress?.Invoke(i + 1,totalCount);
			}
		}

		/// <summary>
		/// Releases cached assets whose stored label matches any value in <paramref name="labelArray"/>.
		/// </summary>
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
						keyList.Add(pair.Key);
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

		/// <summary>
		/// Resolves the dictionary key used by <see cref="ResourceManager"/> (<c>Assets/...</c> path).
		/// </summary>
		private static string _GetAssetPathKey(IResourceLocation location)
		{
			if(KZFileKit.IsStartWithAssetHeader(location.PrimaryKey))
			{
				return _NormalizeAssetPath(location.PrimaryKey);
			}

			if(KZFileKit.IsStartWithAssetHeader(location.InternalId))
			{
				return _NormalizeAssetPath(location.InternalId);
			}

			return _NormalizeAssetPath(location.PrimaryKey);
		}

		/// <summary>Normalizes a project path for dictionary lookup.</summary>
		private static string _NormalizeAssetPath(string path)
		{
			return KZFileKit.NormalizePath(path);
		}

		/// <summary>Returns whether <paramref name="assetPath"/> is the folder itself or a file directly under it.</summary>
		private static bool _IsAssetUnderFolder(string assetPath,string folderPath)
		{
			if(assetPath.IsEqual(folderPath))
			{
				return true;
			}

			return assetPath.StartsWith($"{folderPath}/",StringComparison.OrdinalIgnoreCase)
				|| assetPath.StartsWith($"{folderPath}\\",StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Reports download progress until <paramref name="handle"/> completes.</summary>
		private static async UniTask _MonitorDownloadProgressAsync(AsyncOperationHandle handle,Action<float,long,long> onUpdateProgress)
		{
			while(!handle.IsDone)
			{
				var status = handle.GetDownloadStatus();

				onUpdateProgress(status.Percent,status.DownloadedBytes,status.TotalBytes);

				await UniTask.Yield();
			}
		}
	}
}
