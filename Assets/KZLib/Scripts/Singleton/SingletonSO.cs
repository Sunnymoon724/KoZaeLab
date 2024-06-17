using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// InSideSingletonSO or OutSideSingletonSO를 사용 함. (직접 사용 X)
/// </summary>
public abstract class SingletonSO<TObject> : SerializedScriptableObject where TObject : SerializedScriptableObject
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

			EditorUtility.SetDirty(this);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

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

	protected static void CreateScriptableObject(string _path)
	{
		m_Instance = CreateInstance<TObject>();

		SaveAsset(_path);
	}
#endif

	public void OnDestroy()
	{
		Release();

		m_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => m_Instance;
}