using UnityEngine;
using UnityEngine.Video;
using System;
using Object = UnityEngine.Object;
using System.Linq;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		public TComponent[] GetObjectArray<TComponent>(string _folderPath,Transform _parent = null,bool _Immediately = true) where TComponent : Component
		{
			var dataArray = GetObjectArray(_folderPath,_parent,_Immediately);

			return dataArray != null ? Array.ConvertAll(dataArray,x=>x.GetComponent<TComponent>()) : null;
		}

		public GameObject[] GetObjectArray(string _folderPath,Transform _parent = null,bool _Immediately = true)
		{
			if(_Immediately)
			{
				var dataArray = GetResourceArray<GameObject>(_folderPath);

				foreach(var data in dataArray)
				{
					data.transform.SetParent(_parent);
				}

				return dataArray;
			}
			else
			{
				AddLoadingQueue(_folderPath,false,_parent);
			}

			return null;
		}

		public AnimatorOverrideController[] GetAnimatorOverrideControllerArray(string _folderPath)
		{
			return GetResourceArray<AnimatorOverrideController>(_folderPath);
		}

		public AnimationClip[] GetAnimationClipArray(string _folderPath)
		{
			return GetResourceArray<AnimationClip>(_folderPath);
		}

		public ScriptableObject[] GetScriptableObjectArray(string _folderPath)
		{
			return GetResourceArray<ScriptableObject>(_folderPath);
		}

		public AudioClip[] GetAudioClipArray(string _folderPath)
		{
			return GetResourceArray<AudioClip>(_folderPath);
		}

		public VideoClip[] GetVideoClipArray(string _folderPath)
		{
			return GetResourceArray<VideoClip>(_folderPath);
		}

		public Sprite[] GetSpriteArray(string _folderPath)
		{
			return GetResourceArray<Sprite>(_folderPath);
		}

		public TextAsset[] GetTextAssetArray(string _folderPath)
		{
			return GetResourceArray<TextAsset>(_folderPath);
		}

		public Material[] GetMaterialArray(string _folderPath)
		{
			return GetResourceArray<Material>(_folderPath);
		}

		private TObject[] GetResourceArray<TObject>(string _folderPath) where TObject : Object
		{
			if(_folderPath.IsEmpty())
			{
				throw new NullReferenceException("No path.");
			}

			// use cache data
			var cacheDataArray = GetCacheDataArray<TObject>(_folderPath);

			if(cacheDataArray.IsNullOrEmpty())
			{
				// load data
				cacheDataArray = LoadDataArray<TObject>(_folderPath);

				if(cacheDataArray.IsNullOrEmpty())
				{
					LogTag.System.W(string.Format($"Resources is not exist. [path : {_folderPath}]"));

					return null;
				}

				PutDataArray(_folderPath,cacheDataArray);
			}

			// data is GameObject -> copy data
			if(typeof(TObject) == typeof(GameObject))
			{
				var dataArray = new TObject[cacheDataArray.Length];

				for(var i=0;i<cacheDataArray.Length;i++)
				{
					var data = UnityUtility.CopyObject(cacheDataArray[i]);

					if(GameSettings.In.IsServerResource)
					{
						(data as GameObject).ReAssignShader();
					}

					dataArray[i] = data;
				}

				return dataArray;
			}

			return cacheDataArray;
		}

		private TObject[] LoadDataArray<TObject>(string _folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(FileUtility.IsFilePath(_folderPath))
			{
				throw new ArgumentException($"Path is file path.[path : {_folderPath}]");
			}
#endif
			if(_folderPath.StartsWith(RESOURCES))
			{
				return Resources.LoadAll<TObject>(FileUtility.RemoveHeaderDirectory(_folderPath,RESOURCES));
			}

			var assetPath = FileUtility.GetAssetsPath(_folderPath);

			if(GameSettings.In.IsServerResource)
			{
				return AddressablesMgr.In.GetObjectArray<TObject>(assetPath);
			}

#if UNITY_EDITOR
			return UnityUtility.LoadAssetGroupInFolder<TObject>(assetPath).ToArray();
#else
			return null;
#endif
		}
	}
}