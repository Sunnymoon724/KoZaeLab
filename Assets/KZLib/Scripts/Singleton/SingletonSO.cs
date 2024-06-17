using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#else

using System;

#endif

/// <summary>
/// InSideSingletonSO or OutSideSingletonSO를 사용 함. (직접 사용 X)
/// </summary>
public abstract class SingletonSO<TObject> : SerializedScriptableObject where TObject : ScriptableObject
{
	protected static TObject m_Instance;

	// Assets 이후로
	protected static void SaveAsset(string _filePath)
	{
#if UNITY_EDITOR
		var filePath = string.Format("{0}.asset",_filePath);

		CommonUtility.CreateFolder(filePath);

		AssetDatabase.CreateAsset(m_Instance,CommonUtility.GetAssetsPath(filePath));

		AssetDatabase.Refresh();
#endif
	}

	private void Awake()
	{
#if UNITY_EDITOR
		if(!m_Initialize)
		{
			m_Initialize = true;

			Initialize();

			SaveData();

			return;
		}
#endif
		Open();
	}

	/// <summary>
	/// 시작할 떄 쓰임
	/// </summary>
	protected virtual void Open() { }

#if UNITY_EDITOR
	[SerializeField,HideInInspector]
	private bool m_Initialize = false;

	/// <summary>
	/// 처음 생성 될때만 쓰임
	/// </summary>
	protected virtual void Initialize() { }

	protected void CreateFolderByFullPath(string _path)
	{
		CommonUtility.CreateFolder(_path);

		AssetDatabase.Refresh();
	}

	protected virtual void SaveData()
	{
		EditorUtility.SetDirty(this);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static bool IsExistFile => !AssetDatabase.FindAssets(string.Format("t:ScriptableObject {0}",typeof(TObject).Name)).IsNullOrEmpty();
#endif

	public void OnDestroy()
	{
		Release();

		m_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => m_Instance;
}

/// <summary>
/// Assets/Resources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/Resources/Settings/ 안에 생성한다.
/// </summary>
public abstract class InSideSingletonSO<TObject> : SingletonSO<TObject> where TObject : ScriptableObject
{
	public static TObject In
	{
		get
		{
			if(!m_Instance)
			{
				var typeName = typeof(TObject).Name;

				var path = CommonUtility.PathCombine(string.Format(Global.IN_SIDE_SO_PATH,"Settings"),typeName);

				m_Instance = Resources.Load<TObject>(path.Replace("Resources/",""));

				if(!m_Instance)
				{
					m_Instance = CreateInstance<TObject>();

					SaveAsset(path);
				}
			}

			return m_Instance;
		}
	}
}

/// <summary>
/// Assets/WorkResources/Settings/ 폴더 안에서 불러오며 없으면 새로 만들고 Assets/WorkResources/Settings/ 안에 생성한다. (오직 에디터용)
/// </summary>
public abstract class OutSideSingletonSO<TObject> : SingletonSO<TObject> where TObject : ScriptableObject
{
	public static TObject In
	{
		get
		{
			if(!m_Instance)
			{
				var typeName = typeof(TObject).Name;
#if UNITY_EDITOR
				var path = CommonUtility.PathCombine(string.Format(Global.OUT_SIDE_SO_PATH,"Settings"),typeName);

				m_Instance = AssetDatabase.LoadAssetAtPath<TObject>(CommonUtility.GetAssetsPath(string.Format("{0}.asset",path)));

				if(!m_Instance)
				{
					m_Instance = CreateInstance<TObject>();

					SaveAsset(path);
				}
#else
				throw new NullReferenceException(string.Format("{0}는 에디터 이외의 곳에서 만들어 질 수 없습니다.",typeName));
#endif
			}

			return m_Instance;
		}
	}
}