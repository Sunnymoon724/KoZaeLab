using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Not exist in scene -> error
/// </summary>
public abstract class SingletonMB<TBehaviour> : SerializedMonoBehaviour where TBehaviour : SerializedMonoBehaviour
{
	protected static TBehaviour s_Instance;

	public static TBehaviour In => s_Instance;

	private void Awake()
	{
		//! Check only one
		if(s_Instance)
		{
			CommonUtility.DestroyObject(gameObject);

			return;
		}

		s_Instance = gameObject.GetComponent<TBehaviour>();

		if(!s_Instance)
		{
			throw new NullReferenceException($"{typeof(TBehaviour)} is not exist.");
		}

		Initialize();
	}

	protected virtual void Initialize() { }

	private void OnDestroy()
	{
		Release();

		if(s_Instance)
		{
			CommonUtility.DestroyObject(gameObject);

			s_Instance = null;
		}
	}

	protected virtual void Release() { }

	public static bool HasInstance => s_Instance;
}

/// <summary>
/// Not exist in scene -> auto create
/// </summary>
public class AutoSingletonMB<TBehaviour> : SerializedMonoBehaviour where TBehaviour : SerializedMonoBehaviour
{
	protected static TBehaviour s_Instance;

	public static TBehaviour In
	{
		get
		{
			if(!s_Instance)
			{
				s_Instance = FindObjectOfType<TBehaviour>();

				if(!s_Instance)
				{
					s_Instance = new GameObject(typeof(TBehaviour).Name).AddComponent<TBehaviour>();
				}
			}

			return s_Instance;
		}
	}

	private void Awake()
	{
		DontDestroyOnLoad(this);

		Initialize();
	}

	protected virtual void Initialize() { }

	private void OnDestroy()
	{
		Release();

		s_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => s_Instance;
}

/// <summary>
/// Load in resources folder.
/// </summary>
public class LoadSingletonMB<TBehaviour> : AutoSingletonMB<TBehaviour> where TBehaviour : SerializedMonoBehaviour
{
	public static new TBehaviour In
	{
		get
		{
			if(!s_Instance)
			{
				s_Instance = FindObjectOfType<TBehaviour>();

				if(!s_Instance)
				{
					var data = Resources.Load<TBehaviour>(typeof(TBehaviour).Name);

					s_Instance = CommonUtility.CopyObject(data);
					s_Instance.name = data.name;
				}
			}

			return s_Instance;
		}
	}
}