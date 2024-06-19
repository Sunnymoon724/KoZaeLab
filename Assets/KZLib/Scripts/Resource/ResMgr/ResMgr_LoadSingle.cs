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
				throw new NullReferenceException("경로가 없습니다.");
			}

			var resource = GetData<TObject>(_filePath);

			if(!resource)
			{
				//? 리소스가 없을 경우 로드 한다.
				resource = LoadData<TObject>(_filePath);

				if(!resource)
				{
					throw new NullReferenceException(string.Format("리소스가 없습니다.[파일 경로 : {0}]",_filePath));
				}

				PutData(_filePath,resource);
			}

			if(typeof(TObject) == typeof(GameObject))
			{
				var data = CommonUtility.CopyObject(resource);

				if(GameSettings.In.IsServerResource)
				{
					CommonUtility.ReAssignShader(data as GameObject);
				}

				return data;
			}

			return resource;
		}

		private TObject LoadData<TObject>(string _filePath) where TObject : Object
		{
#if UNITY_EDITOR
			if(!CommonUtility.IsFilePath(_filePath))
			{
				throw new NullReferenceException(string.Format("경로가 폴더 경로 입니다.[경로 : {0}]",_filePath));
			}
#endif
			//? Resources 안의 파일 이므로 바로 로드 한다.
			if(_filePath.StartsWith(RESOURCES))
			{
				var filePath = CommonUtility.RemoveHeaderDirectory(_filePath,RESOURCES);

				return Resources.Load<TObject>(filePath[..filePath.LastIndexOf('.')]);
			}

			//? 경로에 Assets을 포함시킨다.
			var assetPath = CommonUtility.GetAssetsPath(_filePath);

			if(GameSettings.In.IsServerResource)
			{
				return AddressablesMgr.In.GetObject<TObject>(assetPath);
			}

			//? 에디터에서만 사용
#if UNITY_EDITOR
			return AssetDatabase.LoadAssetAtPath<TObject>(assetPath);
#else
			return null;
#endif
		}
	}
}