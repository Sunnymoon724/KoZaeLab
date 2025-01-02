using UnityEngine;
using UnityEngine.Video;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		public TComponent[] GetObjectArray<TComponent>(string folderPath,Transform parent = null,bool immediately = true) where TComponent : Component
		{
			var gameObjectArray = GetObjectArray(folderPath,parent,immediately);

			return gameObjectArray != null ? Array.ConvertAll(gameObjectArray,x=>x.GetComponent<TComponent>()) : null;
		}

		public GameObject[] GetObjectArray(string folderPath,Transform parent = null,bool immediately = true)
		{
			if(immediately)
			{
				var gameObjectArray = GetResourceArray<GameObject>(folderPath);

				foreach(var gameObject in gameObjectArray)
				{
					gameObject.transform.SetParent(parent);
				}

				return gameObjectArray;
			}
			else
			{
				AddLoadingQueue(folderPath,false,parent);
			}

			return null;
		}

		public AnimatorOverrideController[] GetAnimatorOverrideControllerArray(string folderPath)
		{
			return GetResourceArray<AnimatorOverrideController>(folderPath);
		}

		public AnimationClip[] GetAnimationClipArray(string folderPath)
		{
			return GetResourceArray<AnimationClip>(folderPath);
		}

		public ScriptableObject[] GetScriptableObjectArray(string folderPath)
		{
			return GetResourceArray<ScriptableObject>(folderPath);
		}

		public AudioClip[] GetAudioClipArray(string folderPath)
		{
			return GetResourceArray<AudioClip>(folderPath);
		}

		public VideoClip[] GetVideoClipArray(string folderPath)
		{
			return GetResourceArray<VideoClip>(folderPath);
		}

		public Sprite[] GetSpriteArray(string folderPath)
		{
			return GetResourceArray<Sprite>(folderPath);
		}

		public TextAsset[] GetTextAssetArray(string folderPath)
		{
			return GetResourceArray<TextAsset>(folderPath);
		}

		public Material[] GetMaterialArray(string folderPath)
		{
			return GetResourceArray<Material>(folderPath);
		}

		private TObject[] GetResourceArray<TObject>(string folderPath) where TObject : Object
		{
			if(folderPath.IsEmpty())
			{
				LogTag.System.I("Path is null.");

				return null;
			}

			// use cache data
			var cacheDataArray = GetCacheDataArray<TObject>(folderPath);

			if(cacheDataArray.IsNullOrEmpty())
			{
				// load data
				cacheDataArray = LoadDataArray<TObject>(folderPath);

				if(cacheDataArray.IsNullOrEmpty())
				{
					LogTag.System.E($"Resources is not exist. [path : {folderPath}]");

					return null;
				}

				PutDataArray(folderPath,cacheDataArray);
			}

			// data is GameObject -> copy data
			if(typeof(TObject) == typeof(GameObject))
			{
				var dataArray = new TObject[cacheDataArray.Length];

				for(var i=0;i<cacheDataArray.Length;i++)
				{
					var cacheData = cacheDataArray[i].CopyObject() as TObject;

					if(GameSettings.In.IsServerResource)
					{
						(cacheData as GameObject).ReAssignShader();
					}

					dataArray[i] = cacheData;
				}

				return dataArray;
			}

			return cacheDataArray;
		}

		private TObject[] LoadDataArray<TObject>(string folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(CommonUtility.IsFilePath(folderPath))
			{
				LogTag.System.E($"Path is folder path.[path : {folderPath}]");

				return null;
			}
#endif
			if(folderPath.StartsWith(RESOURCES))
			{
				return Resources.LoadAll<TObject>(CommonUtility.RemoveTextInPath(folderPath,RESOURCES));
			}

			var assetPath = CommonUtility.GetAssetsPath(folderPath);

			if(GameSettings.In.IsServerResource)
			{
				return AddressablesMgr.In.GetObjectArray<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return CommonUtility.FindAssetGroupInFolder<TObject>(assetPath).ToArray();
#else
			return null;
#endif
		}
	}
}