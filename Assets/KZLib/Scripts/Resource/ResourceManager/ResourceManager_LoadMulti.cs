using UnityEngine;
using UnityEngine.Video;
using System;
using System.Linq;
using KZLib.KZUtility;
using System.IO;
using Object = UnityEngine.Object;

namespace KZLib
{
	public partial class ResourceManager : Singleton<ResourceManager>
	{
		public TComponent[] GetObjectArray<TComponent>(string folderPath,Transform parent = null,bool immediately = true) where TComponent : Component
		{
			var gameObjectArray = GetObjectArray(folderPath,parent,immediately);

			static TComponent _Convert(GameObject gameObject)
			{
				return gameObject.GetComponent<TComponent>();
			}

			return gameObjectArray != null ? Array.ConvertAll(gameObjectArray,_Convert) : null;
		}

		public GameObject[] GetObjectArray(string folderPath,Transform parent = null,bool immediately = true)
		{
			if(immediately)
			{
				var gameObjectArray = _GetResourceArray<GameObject>(folderPath);

				foreach(var gameObject in gameObjectArray)
				{
					gameObject.transform.SetParent(parent);
				}

				return gameObjectArray;
			}
			else
			{
				_AddLoadingQueue(folderPath,false,parent);
			}

			return null;
		}

		public AnimatorOverrideController[] GetAnimatorOverrideControllerArray(string folderPath)
		{
			return _GetResourceArray<AnimatorOverrideController>(folderPath);
		}

		public AnimationClip[] GetAnimationClipArray(string folderPath)
		{
			return _GetResourceArray<AnimationClip>(folderPath);
		}

		public ScriptableObject[] GetScriptableObjectArray(string folderPath)
		{
			return _GetResourceArray<ScriptableObject>(folderPath);
		}

		public AudioClip[] GetAudioClipArray(string folderPath)
		{
			return _GetResourceArray<AudioClip>(folderPath);
		}

		public VideoClip[] GetVideoClipArray(string folderPath)
		{
			return _GetResourceArray<VideoClip>(folderPath);
		}

		public Sprite[] GetSpriteArray(string folderPath)
		{
			return _GetResourceArray<Sprite>(folderPath);
		}

		public TextAsset[] GetTextAssetArray(string folderPath)
		{
			return _GetResourceArray<TextAsset>(folderPath);
		}

		public Material[] GetMaterialArray(string folderPath)
		{
			return _GetResourceArray<Material>(folderPath);
		}

		private TObject[] _GetResourceArray<TObject>(string folderPath) where TObject : Object
		{
			if(folderPath.IsEmpty())
			{
				LogSvc.System.I("Path is null.");

				return null;
			}

			// use cache
			var cacheArray = _GetCacheArray<TObject>(folderPath);

			if(cacheArray.IsNullOrEmpty())
			{
				// load
				cacheArray = _LoadResourceArray<TObject>(folderPath);

				if(cacheArray.IsNullOrEmpty())
				{
					LogSvc.System.E($"Resources is not exist. [path : {folderPath}]");

					return null;
				}

				_StoreCacheArray(folderPath,cacheArray);
			}

			// resource is GameObject -> copy
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

		private TObject[] _LoadResourceArray<TObject>(string folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(Path.HasExtension(folderPath))
			{
				LogSvc.System.E($"Path is folder path.[path : {folderPath}]");

				return null;
			}
#endif
			if(folderPath.StartsWith(Global.RESOURCES_TEXT))
			{
				return Resources.LoadAll<TObject>(FileUtility.RemoveHeaderInPath(folderPath,Global.RESOURCES_TEXT));
			}

			var assetPath = FileUtility.GetAssetPath(folderPath);

			if(m_useServerResource)
			{
				return AddressablesManager.In.ExtractObjectArray<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return CommonUtility.FindAssetGroupInFolder<TObject>(assetPath).ToArray();
#else
			return null;
#endif
		}
	}
}