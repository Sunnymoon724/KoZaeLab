using System;
using KZLib;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public static partial class CommonUtility
{
	public static void ClearUnLoadedAssetMemory()
	{
		Resources.UnloadUnusedAssets();

		GC.Collect();
	}

	public static bool CheckVersion(string _version)
	{
		var version = GameSettings.In.GameVersion;

		return version.CompareTo(_version) >= 0;
	}

	public static string CheckMetaData(IMetaData _metaData,int _metaId)
	{
		if(_metaData == null || !_metaData.IsExist)
		{
			//? 존재 X
			return string.Format("<color={0}>{1}</color>",Global.WRONG_HEX_COLOR,_metaId);
		}

		if(!CheckVersion(_metaData.Version))
		{
			//? 버전이 안맞음
			return string.Format("<color={0}>{1}</color>",Global.DISABLE_HEX_COLOR,_metaId);
		}

		return string.Format("{0}",_metaId);
	}

	public static void LockInput()
	{
		SetInput(true);
	}

	public static void UnLockInput()
	{
		SetInput(false);
	}
	
	private static void SetInput(bool _lock)
	{
		// TODO 인풋 체크하기

		if(UIMgr.HasInstance)
		{
			UIMgr.In.BlockInput(_lock);
		}
	}

	public static void PauseGame()
	{
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
	}

	public static void UnPauseGame()
	{
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
	}

	public static bool IsPaused()
	{
		return Time.timeScale == 0.0f;
	}

	public static void ReleaseManager()
	{
		//? SingletonMB 릴리즈
		if(SoundMgr.HasInstance)
		{
			DestroyObject(SoundMgr.In.gameObject);
		}

		if(SceneMgr.HasInstance)
		{
			DestroyObject(SceneMgr.In.gameObject);
		}

		if(UIMgr.HasInstance)
		{
			DestroyObject(UIMgr.In.gameObject);
		}

		if(CameraMgr.HasInstance)
		{
			DestroyObject(CameraMgr.In.gameObject);
		}

		//? Singleton 릴리즈
		if(MetaDataMgr.HasInstance)
		{
			MetaDataMgr.In.Dispose();
		}

		if(GameDataMgr.HasInstance)
		{
			GameDataMgr.In.Dispose();
		}

		if(SaveDataMgr.HasInstance)
		{
			SaveDataMgr.In.Dispose();
		}

		if(ResMgr.HasInstance)
		{
			ResMgr.In.Dispose();
		}

		if(LogMgr.HasInstance)
		{
			LogMgr.In.Dispose();
		}

		if(AddressablesMgr.HasInstance)
		{
			AddressablesMgr.In.Dispose();
		}

		ReflectionUtility.ClearCacheData();
	}

#if UNITY_EDITOR
	public static byte[] GetTestImageData()
	{
		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(GetTemplateFilePath("Ostrich.png"));

		return sprite != null ? sprite.texture.EncodeToPNG() : null;
	}

	public static string GetTemplateFilePath(string _fileName)
	{
		var filePath = string.Format("Packages/com.bsheepstudio.kzlib/WorkResources/Templates/{0}",_fileName);

		if(IsExistFile(PathCombine(GetProjectPath(),filePath)))
		{
			return filePath;
		}

		filePath = string.Format("Assets/KZLib/WorkResources/Templates/{0}",_fileName);

		if(IsExistFile(filePath))
		{
			return filePath;
		}

		throw new NullReferenceException(string.Format("템플릿 폴더에 해당 파일이 없습니다. [{0}]",_fileName));
	}

	public static string GetTemplateFullFilePath(string _fileName)
	{
		var filePath = GetTemplateFilePath(_fileName);

		return filePath.StartsWith("Packages/") ? PathCombine(GetProjectPath(),filePath) : GetFullPath(filePath);
	}
#endif
}