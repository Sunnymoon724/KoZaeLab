using UnityEngine;
using UnityEngine.Video;
using KZLib.Utilities;
using System.IO;

using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	/// <summary>Single-asset load API and load routing for <see cref="ResourceManager"/>.</summary>
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		/// <summary>
		/// Loads a prefab, instantiates it, and returns the requested component from the instance root or its children.
		/// </summary>
		/// <param name="immediately">Passed through to <see cref="GetObject(string,Transform,bool)"/>.</param>
		public TComponent GetObject<TComponent>(string filePath,Transform parent = null,bool immediately = true) where TComponent : Component
		{
			var gameObject = GetObject(filePath,parent,immediately);

			return gameObject ? gameObject.GetComponentInChildren<TComponent>(true) : null;
		}

		/// <summary>
		/// Loads a prefab and returns a new instance parented under <paramref name="parent"/>.
		/// </summary>
		/// <param name="filePath">Project-relative file path, e.g. <c>Resources/Prefab/UI/Foo.prefab</c>.</param>
		/// <param name="immediately">
		/// When false, enqueues the load and returns null. The background queue loads one item per update period
		/// so many prefabs can be spread across frames; each instance is parented under <paramref name="parent"/>.
		/// Not intended as a completion callback — use when you only need gradual loading under a known parent.
		/// </param>
		public GameObject GetObject(string filePath,Transform parent = null,bool immediately = true)
		{
			if(immediately)
			{
				var gameObject = _GetResource<GameObject>(filePath);

				if(gameObject)
				{
					gameObject.transform.SetParent(parent);

					return gameObject;
				}
			}
			else
			{
				_AddLoadingQueue(filePath,parent);
			}

			return null;
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public AnimatorOverrideController GetAnimatorOverrideController(string filePath)
		{
			return _GetResource<AnimatorOverrideController>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public AnimationClip GetAnimationClip(string filePath)
		{
			return _GetResource<AnimationClip>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public ScriptableObject GetScriptableObject(string filePath)
		{
			return _GetResource<ScriptableObject>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public TObject GetScriptableObject<TObject>(string filePath) where TObject : ScriptableObject
		{
			return _GetResource<TObject>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public AudioClip GetAudioClip(string filePath)
		{
			return _GetResource<AudioClip>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public VideoClip GetVideoClip(string filePath)
		{
			return _GetResource<VideoClip>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public Sprite GetSprite(string filePath)
		{
			return _GetResource<Sprite>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public TextAsset GetTextAsset(string filePath)
		{
			return _GetResource<TextAsset>(filePath);
		}

		/// <inheritdoc cref="GetObject(string,Transform,bool)"/>
		public Material GetMaterial(string filePath)
		{
			return _GetResource<Material>(filePath);
		}

		/// <summary>
		/// Returns a loaded asset by path, using the internal cache when available.
		/// <see cref="GameObject"/> sources are instantiated; all other types return the cached source asset.
		/// </summary>
		private TObject _GetResource<TObject>(string filePath) where TObject : Object
		{
			if(filePath.IsEmpty())
			{
				LogChannel.Resource.I("Path is null.");

				return null;
			}

			var cache = _GetCache<TObject>(filePath);

			if(!cache)
			{
				cache = _LoadResource<TObject>(filePath);

				if(!cache)
				{
					LogChannel.Resource.E($"Resources do not exist. [path : {filePath}]");

					return null;
				}

				_StoreCache(filePath,cache);
			}

			if(typeof(TObject) == typeof(GameObject))
			{
				var gameObject = cache.CopyObject() as TObject;

				if(m_useServerResource)
				{
					(gameObject as GameObject).ReAssignShader();
				}

				return gameObject;
			}

			return cache;
		}

		/// <summary>
		/// Loads a source asset without caching or instantiating.
		/// <list type="number">
		/// <item><description><c>Resources/...</c> → <see cref="Resources.Load{TObject}(string)"/></description></item>
		/// <item><description>Server resource mode → <see cref="AddressablesManager.GetObject{TObject}(string)"/></description></item>
		/// <item><description>Editor local mode → <see cref="AssetDatabase.LoadAssetAtPath{TObject}(string)"/></description></item>
		/// </list>
		/// </summary>
		private TObject _LoadResource<TObject>(string filePath) where TObject : Object
		{
#if UNITY_EDITOR
			if(!Path.HasExtension(filePath))
			{
				LogChannel.Resource.E($"Path is folder path.[path : {filePath}]");

				return null;
			}
#endif
			if(filePath.StartsWith(c_Resources))
			{
				var resourcePath = KZFileKit.RemoveHeaderInPath(filePath,c_Resources);
				var dotIndex = resourcePath.LastIndexOf('.');

				if(dotIndex <= 0)
				{
					throw new InvalidDataException($"Path has no extension. [path : {filePath}]");
				}

				return Resources.Load<TObject>(resourcePath[..dotIndex]);
			}

			var assetPath = KZFileKit.GetAssetPath(filePath);

			if(m_useServerResource)
			{
				return AddressablesManager.In.GetObject<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return AssetDatabase.LoadAssetAtPath<TObject>(assetPath);
#else
			return null;
#endif
		}
	}
}
