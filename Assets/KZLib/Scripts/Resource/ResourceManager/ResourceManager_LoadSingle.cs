using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;
using KZLib.KZUtility;
using System.IO;

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
				var gameObject = GetResource<GameObject>(filePath);

				if(gameObject)
				{
					gameObject.transform.SetParent(parent);

					return gameObject;
				}
			}
			else
			{
				AddLoadingQueue(filePath,true,parent);
			}

			return null;
		}

		public AnimatorOverrideController GetAnimatorOverrideController(string filePath)
		{
			return GetResource<AnimatorOverrideController>(filePath);
		}

		public AnimationClip GetAnimationClip(string filePath)
		{
			return GetResource<AnimationClip>(filePath);
		}

		public ScriptableObject GetScriptableObject(string filePath)
		{
			return GetResource<ScriptableObject>(filePath);
		}

		public TObject GetScriptableObject<TObject>(string filePath) where TObject : ScriptableObject
		{
			return GetResource<TObject>(filePath);
		}

		public AudioClip GetAudioClip(string filePath)
		{
			return GetResource<AudioClip>(filePath);
		}

		public VideoClip GetVideoClip(string filePath)
		{
			return GetResource<VideoClip>(filePath);
		}

		public Sprite GetSprite(string filePath)
		{
			return GetResource<Sprite>(filePath);
		}

		public TextAsset GetTextAsset(string filePath)
		{
			return GetResource<TextAsset>(filePath);
		}

		public Material GetMaterial(string filePath)
		{
			return GetResource<Material>(filePath);
		}

		private TObject GetResource<TObject>(string filePath) where TObject : Object
		{
			if(filePath.IsEmpty())
			{
				LogTag.System.I("Path is null.");

				return null;
			}

			// use cache data
			var cacheData = GetCacheData<TObject>(filePath);

			if(!cacheData)
			{
				// load data
				cacheData = LoadData<TObject>(filePath);

				if(!cacheData)
				{
					LogTag.System.E($"Resources is not exist. [path : {filePath}]");

					return null;
				}

				PutData(filePath,cacheData);
			}

			// data is GameObject -> copy data
			if(typeof(TObject) == typeof(GameObject))
			{
				var gameObject = cacheData.CopyObject() as TObject;

				if(m_useServerResource)
				{
					(gameObject as GameObject).ReAssignShader();
				}

				return gameObject;
			}

			return cacheData;
		}

		private TObject LoadData<TObject>(string filePath) where TObject : Object
		{
#if UNITY_EDITOR
			if(!Path.HasExtension(filePath))
			{
				LogTag.System.E($"Path is folder path.[path : {filePath}]");

				return null;
			}
#endif
			if(filePath.StartsWith(c_resource_text))
			{
				var resourcePath = CommonUtility.RemoveTextInPath(filePath,c_resource_text);

				return Resources.Load<TObject>(resourcePath[..resourcePath.LastIndexOf('.')]);
			}

			var assetPath = CommonUtility.GetAssetsPath(filePath);

			if(m_useServerResource)
			{
				return AddressablesMgr.In.GetObject<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return AssetDatabase.LoadAssetAtPath<TObject>(assetPath);
#else
			return null;
#endif
		}
	}
}