using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using Object = UnityEngine.Object;
using KZLib.KZDevelop;

namespace KZLib
{
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		private record LoadingResourceInfo(string ResourcePath,bool IsFilePath,Transform Parent);

		private bool m_disposed = false;

		private const float c_updatePeriod = 0.1f;
		private const string c_Resources = "Resources";

		private CancellationTokenSource m_tokenSource = null;

		private readonly Queue<LoadingResourceInfo> m_loadingInfoQueue = new();

		private readonly CacheResolver<Object[]> m_cacheResolver = new();

		private bool m_useServerResource = false;

		protected override void Initialize()
		{
			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			var gameCfg = ConfigManager.In.Access<GameConfig>();

			m_useServerResource = !gameCfg.IsLocalResource;

			_LoopProcessAsync(m_tokenSource.Token).Forget();
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
				m_loadingInfoQueue.Clear();

				m_cacheResolver.Dispose();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private async UniTaskVoid _LoopProcessAsync(CancellationToken token)
		{
			while(!token.IsCancellationRequested) 
			{
				await UniTask.Delay(TimeSpan.FromSeconds(c_updatePeriod),true,cancellationToken : token).SuppressCancellationThrow();

				// Set Loading Queue
				if(m_loadingInfoQueue.Count > 0)
				{
					var loadingInfo = m_loadingInfoQueue.Dequeue();

					GetObject(loadingInfo.ResourcePath,loadingInfo.Parent,true);
				}
			}
		}

		private void _AddLoadingQueue(string path,bool isFilePath,Transform parent = null)
		{
			m_loadingInfoQueue.Enqueue(new LoadingResourceInfo(path,isFilePath,parent));
		}

		private TObject _GetCache<TObject>(string path) where TObject : Object
		{
			var cacheArray = _GetCacheArray<TObject>(path);

			return cacheArray?.Length > 0 ? cacheArray[0] : null;
		}

		private TObject[] _GetCacheArray<TObject>(string path) where TObject : Object
		{
			return m_cacheResolver.TryGetCache(path,out var cacheArray) ? cacheArray as TObject[] : null;
		}

		private void _StoreCache<TObject>(string path,TObject cache) where TObject : Object
		{
			_StoreCacheArray(path,new TObject[] { cache });
		}

		private void _StoreCacheArray<TObject>(string path,TObject[] cacheArray) where TObject : Object
		{
			m_cacheResolver.StoreCache(path,cacheArray,false);
		}
	}
}