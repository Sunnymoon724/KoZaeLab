using UnityEngine;
using UnityEngine.Video;
using System;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		public TComponent GetObject<TComponent>(string _filePath,Transform _parent = null,bool _immediately = true) where TComponent : Component
		{
			var data = GetObject(_filePath,_parent,_immediately);

			return data ? data.GetComponent<TComponent>() : null;
		}

		public GameObject GetObject(string _filePath,Transform _parent = null,bool _immediately = true)
		{
			if(_immediately)
			{
				var data = GetResource<GameObject>(_filePath);

				if(data)
				{
					data.transform.SetParent(_parent);

					return data;
				}
			}
			else
			{
				AddLoadingQueue(_filePath,true,_parent);
			}

			return null;
		}

		public AnimatorOverrideController GetAnimatorOverrideController(string _filePath)
		{
			return GetResource<AnimatorOverrideController>(_filePath);
		}

		public AnimationClip GetAnimationClip(string _filePath)
		{
			return GetResource<AnimationClip>(_filePath);
		}

		public ScriptableObject GetScriptableObject(string _filePath)
		{
			return GetResource<ScriptableObject>(_filePath);
		}

		public TObject GetScriptableObject<TObject>(string _filePath) where TObject : ScriptableObject
		{
			return GetResource<TObject>(_filePath);
		}

		public AudioClip GetAudioClip(string _filePath)
		{
			return GetResource<AudioClip>(_filePath);
		}

		public VideoClip GetVideoClip(string _filePath)
		{
			return GetResource<VideoClip>(_filePath);
		}

		public Sprite GetSprite(string _filePath)
		{
			return GetResource<Sprite>(_filePath);
		}

		public TextAsset GetTextAsset(string _filePath)
		{
			return GetResource<TextAsset>(_filePath);
		}

		public Material GetMaterial(string _filePath)
		{
			return GetResource<Material>(_filePath);
		}

		private TObject GetResource<TObject>(string _filePath) where TObject : Object
		{
			if(_filePath.IsEmpty())
			{
				LogTag.System.I("Path is null.");

				return null;
			}

			// use cache data
			var cacheData = GetCacheData<TObject>(_filePath);

			if(!cacheData)
			{
				// load data
				cacheData = LoadData<TObject>(_filePath);

				if(!cacheData)
				{
					LogTag.System.E($"Resources is not exist. [path : {_filePath}]");

					return null;
				}

				PutData(_filePath,cacheData);
			}

			// data is GameObject -> copy data
			if(typeof(TObject) == typeof(GameObject))
			{
				var data = CommonUtility.CopyObject(cacheData);

				if(GameSettings.In.IsServerResource)
				{
					(data as GameObject).ReAssignShader();
				}

				return data;
			}

			return cacheData;
		}

		private TObject LoadData<TObject>(string _filePath) where TObject : Object
		{
#if UNITY_EDITOR
			if(!CommonUtility.IsFilePath(_filePath))
			{
				LogTag.System.E($"Path is folder path.[path : {_filePath}]");

				return null;
			}
#endif
			if(_filePath.StartsWith(RESOURCES))
			{
				var filePath = CommonUtility.RemoveHeaderDirectory(_filePath,RESOURCES);

				return Resources.Load<TObject>(filePath[..filePath.LastIndexOf('.')]);
			}

			var assetPath = CommonUtility.GetAssetsPath(_filePath);

			if(GameSettings.In.IsServerResource)
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