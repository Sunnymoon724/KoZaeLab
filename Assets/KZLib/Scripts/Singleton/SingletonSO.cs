using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// 해당 경로에서 체크해서 없으면 에러
/// </summary>
public abstract class SingletonSO<TObject> : SerializedScriptableObject where TObject : SerializedScriptableObject
{
	protected static TObject s_Instance = null;

	public static TObject In
	{
		get
		{
			if(!s_Instance)
			{
				s_Instance = Resources.Load<TObject>(CommonUtility.PathCombine("ScriptableObjects",typeof(TObject).Name));
			}

			return s_Instance;
		}
	}

	protected static void CreateScriptableObject(string _path)
	{
		s_Instance = CreateInstance<TObject>();

		var filePath = string.Format("{0}.asset",_path.StartsWith(Global.ASSETS_HEADER) ? _path : CommonUtility.PathCombine("Assets/Resources",_path));

		CommonUtility.CreateFolder(filePath);

		AssetDatabase.CreateAsset(s_Instance,CommonUtility.GetAssetsPath(filePath));
		AssetDatabase.Refresh();
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
#endif

	public void OnDestroy()
	{
		Release();

		s_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => s_Instance;
}