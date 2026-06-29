using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using KZLib.Utilities;
using KZLib.Data;

using Object = UnityEngine.Object;

namespace KZLib
{
	/// <summary>
	/// Central asset loader for the project.
	/// <list type="bullet">
	/// <item><description><c>Resources/...</c> paths always use <see cref="Resources.Load"/> (built-in assets).</description></item>
	/// <item><description>Other paths use <see cref="UnityEditor.AssetDatabase"/> in the editor or <see cref="AddressablesManager"/> when <see cref="GameConfig.IsLocalResource"/> is false.</description></item>
	/// <item><description>Loaded assets are cached by path. <see cref="GameObject"/> requests return new instances; other types return shared asset references.</description></item>
	/// <item><description>Deferred prefab loads (<c>immediately: false</c>) are spread across frames via an internal lazy queue.</description></item>
	/// </list>
	/// Core partial: initialization, cache, and deferred-load queue.
	/// </summary>
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		/// <summary>Single deferred prefab load entry.</summary>
		private record LoadingResourceInfo(string ResourcePath,Transform Parent);

		/// <summary>Delay between processing two queued prefab loads.</summary>
		private const float c_updatePeriod = 0.1f;

		/// <summary>Path prefix that routes loads through Unity's Resources API.</summary>
		private const string c_Resources = "Resources";

		private CancellationTokenSource m_tokenSource = null;

		private readonly Queue<LoadingResourceInfo> m_loadingInfoQueue = new();

		/// <summary>Caches loaded source assets keyed by path. Values are stored as single-element arrays for unified single/folder storage.</summary>
		private CacheResolver<Object[]> m_cacheResolver = new();

		/// <summary>When true, non-Resources paths are resolved through <see cref="AddressablesManager"/>.</summary>
		private bool m_useServerResource = false;

		/// <summary>Guards against starting multiple lazy loop tasks.</summary>
		private bool m_isLoopRunning = false;

		private ResourceManager() { }

		protected override void _Initialize()
		{
			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			m_isLoopRunning = false;

			var gameCfg = ConfigManager.In.Fetch<GameConfig>();

			m_useServerResource = !gameCfg.IsLocalResource;
		}

		protected override void _Release(bool disposing)
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);

			m_isLoopRunning = false;

			if(disposing)
			{
				m_loadingInfoQueue.Clear();

				m_cacheResolver.Dispose();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Lazy background loop. Started only when the deferred queue receives an item; exits when the queue is empty.
		/// Processes one prefab load per <see cref="c_updatePeriod"/>.
		/// </summary>
		private async UniTaskVoid _LoopProcessAsync(CancellationToken token)
		{
			try
			{
				while(!token.IsCancellationRequested)
				{
					if(m_loadingInfoQueue.Count == 0)
					{
						return;
					}

					await UniTask.Delay(TimeSpan.FromSeconds(c_updatePeriod),true,cancellationToken : token).SuppressCancellationThrow();

					if(token.IsCancellationRequested || m_loadingInfoQueue.Count == 0)
					{
						return;
					}

					var loadingInfo = m_loadingInfoQueue.Dequeue();

					GetObject(loadingInfo.ResourcePath,loadingInfo.Parent,true);
				}
			}
			finally
			{
				m_isLoopRunning = false;
			}
		}

		/// <summary>Starts the lazy loop if it is not already running.</summary>
		private void _EnsureLoopRunning()
		{
			if(m_isLoopRunning)
			{
				return;
			}

			m_isLoopRunning = true;

			_LoopProcessAsync(m_tokenSource.Token).Forget();
		}

		/// <summary>
		/// Enqueues a deferred single-prefab load. The lazy loop processes one item per <see cref="c_updatePeriod"/>
		/// so many prefabs can be spread across frames instead of spiking a single frame.
		/// </summary>
		private void _AddLoadingQueue(string path,Transform parent = null)
		{
			m_loadingInfoQueue.Enqueue(new LoadingResourceInfo(path,parent));

			_EnsureLoopRunning();
		}

		/// <summary>Returns the first cached asset for a single-file path.</summary>
		private TObject _GetCache<TObject>(string path) where TObject : Object
		{
			var cacheArray = _GetCacheArray<TObject>(path);

			return cacheArray?.Length > 0 ? cacheArray[0] : null;
		}

		/// <summary>Returns the cached asset array for a folder path.</summary>
		private TObject[] _GetCacheArray<TObject>(string path) where TObject : Object
		{
			return m_cacheResolver.TryGetCache(path,out var cacheArray) ? cacheArray as TObject[] : null;
		}

		/// <summary>Stores a single loaded source asset under <paramref name="path"/>.</summary>
		private void _StoreCache<TObject>(string path,TObject cache) where TObject : Object
		{
			_StoreCacheArray(path,new TObject[] { cache });
		}

		/// <summary>Stores loaded source assets for a folder path.</summary>
		private void _StoreCacheArray<TObject>(string path,TObject[] cacheArray) where TObject : Object
		{
			m_cacheResolver.StoreCache(path,cacheArray,false);
		}

		/// <summary>
		/// Drops loaded asset references held by this manager and clears pending deferred loads.
		/// Unity's own asset cache is unaffected.
		/// </summary>
		public void ClearCache()
		{
			m_loadingInfoQueue.Clear();

			m_cacheResolver.Dispose();

			m_cacheResolver = new CacheResolver<Object[]>();
		}
	}
}
