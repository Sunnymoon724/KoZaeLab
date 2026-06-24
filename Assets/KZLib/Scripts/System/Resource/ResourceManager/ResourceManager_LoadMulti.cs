using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;
using KZLib.Utilities;
using System.IO;

using Object = UnityEngine.Object;

namespace KZLib
{
	/// <summary>Folder-based load API and deferred folder enqueue for <see cref="ResourceManager"/>.</summary>
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		/// <summary>
		/// Loads all prefabs in a folder, instantiates them, and returns the requested component from each instance.
		/// </summary>
		/// <param name="immediately">Passed through to <see cref="GetObjectArray(string,Transform,bool)"/>.</param>
		public TComponent[] GetObjectArray<TComponent>(string folderPath,Transform parent = null,bool immediately = true) where TComponent : Component
		{
			var gameObjectArray = GetObjectArray(folderPath,parent,immediately);

			static TComponent _Convert(GameObject gameObject)
			{
				return gameObject.GetComponentInChildren<TComponent>(true);
			}

			return gameObjectArray != null ? Array.ConvertAll(gameObjectArray,_Convert) : null;
		}

		/// <summary>
		/// Loads all prefabs in a folder and returns new instances parented under <paramref name="parent"/>.
		/// </summary>
		/// <param name="folderPath">Project-relative folder path without a file extension, e.g. <c>Resources/Prefab/FX</c>.</param>
		/// <param name="immediately">
		/// When false, enqueues one load per prefab in the folder and returns null. The background queue loads one item per update period
		/// so many prefabs can be spread across frames; each instance is parented under <paramref name="parent"/>.
		/// Not intended as a completion callback — use when you only need gradual loading under a known parent.
		/// </param>
		public GameObject[] GetObjectArray(string folderPath,Transform parent = null,bool immediately = true)
		{
			if(immediately)
			{
				var gameObjectArray = _GetResourceArray<GameObject>(folderPath);

				if(gameObjectArray == null)
				{
					return null;
				}

				for(var i=0;i<gameObjectArray.Length;i++)
				{
					gameObjectArray[i].transform.SetParent(parent);
				}

				return gameObjectArray;
			}
			else
			{
				_EnqueueFolderGameObject(folderPath,parent);
			}

			return null;
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public AnimatorOverrideController[] GetAnimatorOverrideControllerArray(string folderPath)
		{
			return _GetResourceArray<AnimatorOverrideController>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public AnimationClip[] GetAnimationClipArray(string folderPath)
		{
			return _GetResourceArray<AnimationClip>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public ScriptableObject[] GetScriptableObjectArray(string folderPath)
		{
			return _GetResourceArray<ScriptableObject>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public AudioClip[] GetAudioClipArray(string folderPath)
		{
			return _GetResourceArray<AudioClip>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public VideoClip[] GetVideoClipArray(string folderPath)
		{
			return _GetResourceArray<VideoClip>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public Sprite[] GetSpriteArray(string folderPath)
		{
			return _GetResourceArray<Sprite>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public TextAsset[] GetTextAssetArray(string folderPath)
		{
			return _GetResourceArray<TextAsset>(folderPath);
		}

		/// <inheritdoc cref="GetObjectArray(string,Transform,bool)"/>
		public Material[] GetMaterialArray(string folderPath)
		{
			return _GetResourceArray<Material>(folderPath);
		}

		/// <summary>
		/// Returns loaded assets for every file in a folder, using the internal cache when available.
		/// <see cref="GameObject"/> sources are instantiated; all other types return cached source assets.
		/// </summary>
		private TObject[] _GetResourceArray<TObject>(string folderPath) where TObject : Object
		{
			if(folderPath.IsEmpty())
			{
				LogChannel.Resource.I("Path is null.");

				return null;
			}

			var cacheArray = _GetCacheArray<TObject>(folderPath);

			if(cacheArray.IsNullOrEmpty())
			{
				cacheArray = _LoadResourceArray<TObject>(folderPath);

				if(cacheArray.IsNullOrEmpty())
				{
					LogChannel.Resource.E($"Resources do not exist. [path : {folderPath}]");

					return null;
				}

				_StoreCacheArray(folderPath,cacheArray);
			}

			if(typeof(TObject) == typeof(GameObject))
			{
				var resourceArray = new TObject[cacheArray.Length];

				for(var i=0;i<cacheArray.Length;i++)
				{
					var cache = cacheArray[i].CopyObject() as TObject;

					if(m_useServerResource)
					{
						(cache as GameObject).ReAssignShader();
					}

					resourceArray[i] = cache;
				}

				return resourceArray;
			}

			return cacheArray;
		}

		/// <summary>
		/// Loads all source assets in a folder without caching or instantiating.
		/// <list type="number">
		/// <item><description><c>Resources/...</c> → <see cref="Resources.LoadAll{TObject}(string)"/></description></item>
		/// <item><description>Server resource mode → <see cref="AddressablesManager.ExtractObjectArray{TObject}(string)"/></description></item>
		/// <item><description>Editor local mode → <see cref="KZAssetKit.FindAssetArrayInFolder{TObject}(string)"/></description></item>
		/// </list>
		/// </summary>
		private TObject[] _LoadResourceArray<TObject>(string folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(Path.HasExtension(folderPath))
			{
				LogChannel.Resource.E($"Path is folder path.[path : {folderPath}]");

				return null;
			}
#endif
			if(folderPath.StartsWith(c_Resources))
			{
				return Resources.LoadAll<TObject>(KZFileKit.RemoveHeaderInPath(folderPath,c_Resources));
			}

			var assetPath = KZFileKit.GetAssetPath(folderPath);

			if(m_useServerResource)
			{
				return AddressablesManager.In.ExtractObjectArray<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return KZAssetKit.FindAssetArrayInFolder<TObject>(assetPath);
#else
			return null;
#endif
		}

		/// <summary>
		/// Resolves prefab file paths under <paramref name="folderPath"/> and enqueues one deferred load per prefab.
		/// </summary>
		private void _EnqueueFolderGameObject(string folderPath,Transform parent)
		{
			var filePathArray = _CollectGameObjectFilePaths(folderPath);

			if(filePathArray.IsNullOrEmpty())
			{
				LogChannel.Resource.E($"Resources do not exist. [path : {folderPath}]");

				return;
			}

			for(var i=0;i<filePathArray.Length;i++)
			{
				_AddLoadingQueue(filePathArray[i],parent);
			}
		}

		/// <summary>
		/// Collects project-relative prefab file paths under a folder for deferred loading.
		/// Listing may load source assets once; instantiation is still spread by the deferred queue.
		/// </summary>
		private string[] _CollectGameObjectFilePaths(string folderPath)
		{
#if UNITY_EDITOR
			if(Path.HasExtension(folderPath))
			{
				LogChannel.Resource.E($"Path is folder path.[path : {folderPath}]");

				return Array.Empty<string>();
			}
#endif
			if(folderPath.StartsWith(c_Resources))
			{
				var resourceFolder = KZFileKit.RemoveHeaderInPath(folderPath,c_Resources);
				var assetArray = Resources.LoadAll<GameObject>(resourceFolder);

				if(assetArray.IsNullOrEmpty())
				{
					return Array.Empty<string>();
				}

				var filePathArray = new string[assetArray.Length];

				for(var i=0;i<assetArray.Length;i++)
				{
					filePathArray[i] = $"{folderPath}/{assetArray[i].name}.prefab";
				}

				return filePathArray;
			}

			var assetPath = KZFileKit.GetAssetPath(folderPath);

			if(m_useServerResource)
			{
				var gameObjectArray = AddressablesManager.In.ExtractObjectArray<GameObject>(assetPath);

				if(gameObjectArray.IsNullOrEmpty())
				{
					return Array.Empty<string>();
				}

				var filePathArray = new string[gameObjectArray.Length];

				for(var i=0;i<gameObjectArray.Length;i++)
				{
					filePathArray[i] = $"{folderPath}/{gameObjectArray[i].name}.prefab";
				}

				return filePathArray;
			}

#if UNITY_EDITOR
			var pathList = new List<string>();

			KZAssetKit.ExecuteMatchedAssetPath("t:Prefab",new[] { assetPath },(path,_,_) =>
			{
				pathList.Add(KZFileKit.RemoveHeaderInPath(path,"Assets/"));

				return true;
			});

			return pathList.ToArray();
#else
			return Array.Empty<string>();
#endif
		}
	}
}
