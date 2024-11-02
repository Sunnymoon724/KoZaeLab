using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

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

		var filePath = $"{(_path.StartsWith(Global.ASSETS_HEADER) ? _path : $"Assets/Resources/{_path}")}.asset";

		CommonUtility.CreateFolder(CommonUtility.GetAbsolutePath(filePath,true));

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
	/// Start
	/// </summary>
	protected virtual void Open() { }

#if UNITY_EDITOR
	[SerializeField,HideInInspector]
	private bool m_Initialize = false;

	/// <summary>
	/// Only Create
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