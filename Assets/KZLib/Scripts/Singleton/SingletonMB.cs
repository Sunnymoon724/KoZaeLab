using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 씬에 있으면 사용하고 없으면 에러 (씬이 사라지면 같이 사라짐)
/// </summary>
public abstract class SingletonMB<TBehaviour> : SerializedMonoBehaviour where TBehaviour : SerializedMonoBehaviour
{
	protected static TBehaviour s_Instance;

	public static TBehaviour In => s_Instance;

	private void Awake()
	{
		//! 중복 처리
		if(s_Instance)
		{
			UnityUtility.DestroyObject(gameObject);

			return;
		}

		s_Instance = gameObject.GetComponent<TBehaviour>();

		if(!s_Instance)
		{
			throw new NullReferenceException(string.Format("{0}이 존재하지 않습니다.",typeof(TBehaviour)));
		}

		Initialize();
	}

	protected virtual void Initialize() { }

	private void OnDestroy()
	{
		Release();

		if(s_Instance)
		{
			UnityUtility.DestroyObject(gameObject);

			s_Instance = null;
		}
	}

	protected virtual void Release() { }

	public static bool HasInstance => s_Instance;
}

/// <summary>
/// 없으면 자동으로 게임 오브젝트를 만들어서 생성한다.
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
/// Resources폴더에서 가져오며 없으면 에러
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

					s_Instance = UnityUtility.CopyObject(data);
					s_Instance.name = data.name;
				}
			}

			return s_Instance;
		}
	}
}