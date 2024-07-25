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
				throw new NullReferenceException("폴더가 없습니다.");
			}

			var resourceArray = GetDataArray<TObject>(_folderPath);

			if(resourceArray.IsNullOrEmpty())
			{
				//? 리소스가 없을 경우 로드 한다.
				resourceArray = LoadDataArray<TObject>(_folderPath);

				if(resourceArray.IsNullOrEmpty())
				{
					LogTag.System.W(string.Format("리소스가 없습니다.[폴더 경로 : {0}]",_folderPath));

					return null;
				}

				PutDataArray(_folderPath,resourceArray);
			}

			if(typeof(TObject) == typeof(GameObject))
			{
				var dataArray = new TObject[resourceArray.Length];

				for(var i=0;i<resourceArray.Length;i++)
				{
					var data = CommonUtility.CopyObject(resourceArray[i]);

					if(GameSettings.In.IsServerResource)
					{
						CommonUtility.ReAssignShader(data as GameObject);
					}

					dataArray[i] = data;
				}

				return dataArray;
			}

			return resourceArray;
		}

		private TObject[] LoadDataArray<TObject>(string _folderPath) where TObject : Object
		{
#if UNITY_EDITOR
			if(FileUtility.IsFilePath(_folderPath))
			{
				throw new NullReferenceException(string.Format("경로가 파일 경로 입니다.[경로 : {0}]",_folderPath));
			}
#endif

			//? Resources로 시작하는건 리소스 폴더이므로
			if(_folderPath.StartsWith(RESOURCES))
			{
				return Resources.LoadAll<TObject>(FileUtility.RemoveHeaderDirectory(_folderPath,RESOURCES));
			}

			if(GameSettings.In.IsServerResource)
			{
				return AddressablesMgr.In.GetObjectArray<TObject>(_folderPath);
			}

#if UNITY_EDITOR
			return CommonUtility.LoadAssetGroupInFolder<TObject>(_folderPath).ToArray();
#else
			return null;
#endif
		}
	}
}