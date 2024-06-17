using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 씬에 있으면 사용하고 없으면 에러 (씬이 사라지면 같이 사라짐)
/// </summary>
public abstract class SingletonMB<TComponent> : SerializedMonoBehaviour where TComponent : Component
{
	protected static TComponent m_Instance;

	public static TComponent In => m_Instance;

	private void Awake()
	{
		//! 중복 처리
		if(m_Instance)
		{
			CommonUtility.DestroyObject(gameObject);

			return;
		}

		m_Instance = gameObject.GetComponent<TComponent>();

		if(!m_Instance)
		{
			throw new NullReferenceException(string.Format("{0}이 존재하지 않습니다.",typeof(TComponent)));
		}

		Initialize();
	}

	protected virtual void Initialize() { }

	private void OnDestroy()
	{
		Release();

		m_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => m_Instance;
}

/// <summary>
/// 없으면 자동으로 게임 오브젝트를 만들어서 생성한다.
/// </summary>
public class AutoSingletonMB<TComponent> : SerializedMonoBehaviour where TComponent : Component
{
	protected static TComponent m_Instance = null;

	public static TComponent In
	{
		get
		{
			if(!m_Instance)
			{
				m_Instance = FindObjectOfType<TComponent>();

				if(!m_Instance)
				{
					m_Instance = new GameObject(typeof(TComponent).Name).AddComponent<TComponent>();
				}
			}

			return m_Instance;
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

		m_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => m_Instance;
}

/// <summary>
/// Resources폴더에서 가져오며 없으면 에러
/// </summary>
public class LoadSingletonMB<TComponent> : SerializedMonoBehaviour where TComponent : Component
{
	protected static TComponent m_Instance = null;

	public static TComponent In
	{
		get
		{
			if(!m_Instance)
			{
				m_Instance = FindObjectOfType<TComponent>();

				if(!m_Instance)
				{
					var data = Resources.Load<TComponent>(typeof(TComponent).Name);

					m_Instance = CommonUtility.CopyObject(data);
					m_Instance.name = data.name;
				}
			}

			return m_Instance;
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

		m_Instance = null;
	}

	protected virtual void Release() { }

	public static bool HasInstance => m_Instance;
}