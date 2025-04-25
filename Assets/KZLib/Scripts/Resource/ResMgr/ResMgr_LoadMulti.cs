using UnityEngine;
using UnityEngine.Video;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using KZLib.KZUtility;
using System.IO;

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
				LogTag.System.I("Path is null.");

				return null;
			}

			// use cache data
			var cacheDataArray = _GetCacheDataArray<TObject>(folderPath);

			if(cacheDataArray.IsNullOrEmpty())
			{
				// load data
				cacheDataArray = _LoadDataArray<TObject>(folderPath);

				if(cacheDataArray.IsNullOrEmpty())
				{
					LogTag.System.E($"Resources is not exist. [path : {folderPath}]");

					return null;
				}

				_PutDataArray(folderPath,cacheDataArray);
			}

			// data is GameObject -> copy data
			if(typeof(TObject) == typeof(GameObject))
			{
				var dataArray = new TObject[cacheDataArray.Length];

				for(var i=0;i<cacheDataArray.Length;i++)
				{
					var cacheData = cacheDataArray[i].CopyObject() as TObject;

					if(m_useServerResource)
					{
						(cacheData as GameObject).ReAssignShader();
					}

					dataArray[i] = cacheData;
				}

				return dataArray;
			}

			return cacheDataArray;
		}

		private TObject[] _LoadDataArray<TObject>(string folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(Path.HasExtension(folderPath))
			{
				LogTag.System.E($"Path is folder path.[path : {folderPath}]");

				return null;
			}
#endif
			if(folderPath.StartsWith(c_resource_text))
			{
				return Resources.LoadAll<TObject>(FileUtility.RemoveHeaderInPath(folderPath,c_resource_text));
			}

			var assetPath = FileUtility.GetAssetPath(folderPath);

			if(m_useServerResource)
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