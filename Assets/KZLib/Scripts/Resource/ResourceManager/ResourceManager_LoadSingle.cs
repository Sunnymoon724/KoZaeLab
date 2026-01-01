using UnityEngine;
using UnityEngine.Video;
using KZLib.KZUtility;
using System.IO;

using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		public TComponent GetObject<TComponent>(string filePath,Transform parent = null,bool immediately = true) where TComponent : Component
		{
			var gameObject = GetObject(filePath,parent,immediately);

			return gameObject ? gameObject.GetComponent<TComponent>() : null;
		}

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
				_AddLoadingQueue(filePath,true,parent);
			}

			return null;
		}

		public AnimatorOverrideController GetAnimatorOverrideController(string filePath)
		{
			return _GetResource<AnimatorOverrideController>(filePath);
		}

		public AnimationClip GetAnimationClip(string filePath)
		{
			return _GetResource<AnimationClip>(filePath);
		}

		public ScriptableObject GetScriptableObject(string filePath)
		{
			return _GetResource<ScriptableObject>(filePath);
		}

		public TObject GetScriptableObject<TObject>(string filePath) where TObject : ScriptableObject
		{
			return _GetResource<TObject>(filePath);
		}

		public AudioClip GetAudioClip(string filePath)
		{
			return _GetResource<AudioClip>(filePath);
		}

		public VideoClip GetVideoClip(string filePath)
		{
			return _GetResource<VideoClip>(filePath);
		}

		public Sprite GetSprite(string filePath)
		{
			return _GetResource<Sprite>(filePath);
		}

		public TextAsset GetTextAsset(string filePath)
		{
			return _GetResource<TextAsset>(filePath);
		}

		public Material GetMaterial(string filePath)
		{
			return _GetResource<Material>(filePath);
		}

		private TObject _GetResource<TObject>(string filePath) where TObject : Object
		{
			if(filePath.IsEmpty())
			{
				LogSvc.System.I("Path is null.");

				return null;
			}

			// use cache
			var cache = _GetCache<TObject>(filePath);

			if(!cache)
			{
				// load
				cache = _LoadResource<TObject>(filePath);

				if(!cache)
				{
					LogSvc.System.E($"Resources is not exist. [path : {filePath}]");

					return null;
				}

				_StoreCache(filePath,cache);
			}

			// resource is GameObject -> copy
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

		private TObject _LoadResource<TObject>(string filePath) where TObject : Object
		{
#if UNITY_EDITOR
			if(!Path.HasExtension(filePath))
			{
				LogSvc.System.E($"Path is folder path.[path : {filePath}]");

				return null;
			}
#endif
			if(filePath.StartsWith(c_Resources))
			{
				var resourcePath = FileUtility.RemoveHeaderInPath(filePath,c_Resources);

				return Resources.Load<TObject>(resourcePath[..resourcePath.LastIndexOf('.')]);
			}

			var assetPath = FileUtility.GetAssetPath(filePath);

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