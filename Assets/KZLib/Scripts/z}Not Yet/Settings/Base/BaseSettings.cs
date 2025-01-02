using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class BaseSettings<TScriptable> : SingletonSO<TScriptable> where TScriptable : ScriptableObject
{
	public static bool IsExist => CommonUtility.IsExistAsset($"t:ScriptableObject {typeof(TScriptable).Name}");

	protected static void CreateScriptableObject(string _path)
	{
		s_instance = CreateInstance<TScriptable>();

		var filePath = $"{(_path.StartsWith(Global.ASSETS_HEADER) ? _path : $"Assets/Resources/{_path}")}.asset";

		CommonUtility.CreateFolder(CommonUtility.GetAbsolutePath(filePath,true));

		AssetDatabase.CreateAsset(s_instance,CommonUtility.GetAssetsPath(filePath));
		AssetDatabase.Refresh();
	}
}

/// <summary>
/// Load [Assets/Resources/Settings/] folder. Not exist -> create in [Assets/Resources/Settings/]
/// </summary>
public abstract class InnerBaseSettings<TScriptable> : BaseSettings<TScriptable> where TScriptable : ScriptableObject
{
	private const string SETTINGS_PATH = "ScriptableObjects/Settings";

	public static new TScriptable In
	{
		get
		{
			if(!s_instance)
			{
				var path = CommonUtility.PathCombine(SETTINGS_PATH,typeof(TScriptable).Name);

				s_instance = Resources.Load<TScriptable>(path);

#if UNITY_EDITOR
				if(!s_instance)
				{
					CreateScriptableObject(path);
				}
#endif
			}

			return s_instance;
		}
	}

	public static void CreateSettings()
	{
		CreateScriptableObject(CommonUtility.PathCombine(SETTINGS_PATH,typeof(TScriptable).Name));
	}
}

/// <summary>
/// Load [Assets/WorkResources/Settings/] folder. Not exist -> create in [Assets/WorkResources/Settings/] (Only Editor)
/// </summary>
public abstract class OuterBaseSettings<TScriptable> : BaseSettings<TScriptable> where TScriptable : ScriptableObject
{
	private const string SETTINGS_PATH = "Assets/WorkResources/ScriptableObjects/Settings";

	public static new TScriptable In
	{
		get
		{
			if(!s_instance)
			{
#if UNITY_EDITOR
				var path = CommonUtility.PathCombine(SETTINGS_PATH,typeof(TScriptable).Name);

				s_instance = AssetDatabase.LoadAssetAtPath<TScriptable>($"{path}.asset");


				if(!s_instance)
				{
					CreateScriptableObject(path);
				}
#endif
			}

			return s_instance;
		}
	}

	public static void CreateSettings()
	{
		CreateScriptableObject(CommonUtility.PathCombine(SETTINGS_PATH,typeof(TScriptable).Name));
	}
}