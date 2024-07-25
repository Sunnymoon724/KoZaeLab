using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class BaseSettings<TObject> : SingletonSO<TObject> where TObject : SerializedScriptableObject
{
	public static bool IsExist => CommonUtility.IsExistAsset(string.Format("t:ScriptableObject {0}",typeof(TObject).Name));
}

/// <summary>
/// Assets/Resources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/Resources/Settings/ 안에 생성한다.
/// </summary>
public abstract class InnerBaseSettings<TObject> : BaseSettings<TObject> where TObject : SerializedScriptableObject
{
	private const string SETTINGS_PATH = "ScriptableObjects/Settings";

	public static new TObject In
	{
		get
		{
			if(!s_Instance)
			{
				var path = FileUtility.PathCombine(SETTINGS_PATH,typeof(TObject).Name);

				s_Instance = Resources.Load<TObject>(path);

#if UNITY_EDITOR
				if(!s_Instance)
				{
					CreateScriptableObject(path);
				}
#endif
			}

			return s_Instance;
		}
	}

	public static void CreateSettings()
	{
		CreateScriptableObject(FileUtility.PathCombine(SETTINGS_PATH,typeof(TObject).Name));
	}
}

/// <summary>
/// Assets/WorkResources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/WorkResources/Settings/ 안에 생성한다. (오직 에디터용)
/// </summary>
public abstract class OuterBaseSettings<TObject> : BaseSettings<TObject> where TObject : SerializedScriptableObject
{
	private const string SETTINGS_PATH = "Assets/WorkResources/ScriptableObjects/Settings";

	public static new TObject In
	{
		get
		{
			if(!s_Instance)
			{
#if UNITY_EDITOR
				var path = FileUtility.PathCombine(SETTINGS_PATH,typeof(TObject).Name);

				s_Instance = AssetDatabase.LoadAssetAtPath<TObject>(string.Format("{0}.asset",path));


				if(!s_Instance)
				{
					CreateScriptableObject(path);
				}
#endif
			}

			return s_Instance;
		}
	}

	public static void CreateSettings()
	{
		CreateScriptableObject(FileUtility.PathCombine(SETTINGS_PATH,typeof(TObject).Name));
	}
}