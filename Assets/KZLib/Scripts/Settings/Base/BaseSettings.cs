using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#else

using System;

#endif

public abstract class BaseSettings<TObject> : SingletonSO<TObject> where TObject : SerializedScriptableObject
{
	public static void CreateSettings() { }

	public static bool IsExist => CommonUtility.IsExistAsset(string.Format("t:ScriptableObject {0}",typeof(TObject).Name));
}

/// <summary>
/// Assets/Resources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/Resources/Settings/ 안에 생성한다.
/// </summary>
public abstract class InnerBaseSettings<TObject> : BaseSettings<TObject> where TObject : SerializedScriptableObject
{
	public static TObject In
	{
		get
		{
			if(!m_Instance)
			{
				var path = CommonUtility.PathCombine("ScriptableObjects/Settings",typeof(TObject).Name);

				m_Instance = Resources.Load<TObject>(path);

#if UNITY_EDITOR
				if(!m_Instance)
				{
					CreateSettings();
				}
#endif
			}

			return m_Instance;
		}
	}

	public static new void CreateSettings()
	{
		CreateScriptableObject(CommonUtility.PathCombine("Resources/ScriptableObjects/Settings",typeof(TObject).Name));
	}
}

/// <summary>
/// Assets/WorkResources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/WorkResources/Settings/ 안에 생성한다. (오직 에디터용)
/// </summary>
public abstract class OuterBaseSettings<TObject> : BaseSettings<TObject> where TObject : SerializedScriptableObject
{
	public static TObject In
	{
		get
		{
			if(!m_Instance)
			{
				var typeName = typeof(TObject).Name;
#if UNITY_EDITOR
				var path = CommonUtility.PathCombine("WorkResources/ScriptableObjects/Settings",typeName);

				m_Instance = AssetDatabase.LoadAssetAtPath<TObject>(CommonUtility.GetAssetsPath(string.Format("{0}.asset",path)));

				if(!m_Instance)
				{
					CreateSettings();
				}
#else
				throw new NullReferenceException(string.Format("{0}는 에디터 이외의 곳에서 만들어 질 수 없습니다.",typeName));
#endif
			}

			return m_Instance;
		}
	}

	public static new void CreateSettings()
	{
		CreateScriptableObject(CommonUtility.PathCombine("WorkResources/ScriptableObjects/Settings",typeof(TObject).Name));
	}
}