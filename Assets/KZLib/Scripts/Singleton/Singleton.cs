using System;

public abstract class Singleton<TClass> : IDisposable where TClass : class,new()
{
	private static TClass s_Instance = null;

	private bool m_Disposed = false;

	public static TClass In
	{
		get
		{
			s_Instance ??= new TClass();

			return s_Instance;
		}
	}

	public static bool HasInstance => s_Instance != null;

	protected Singleton() => Initialize();
	~Singleton() => Release(false);

	protected virtual void Initialize() { }

	protected virtual void Release(bool _disposing)
	{
		if(m_Disposed)
		{
			return;
		}

		if(_disposing)
		{
			// 관리 데이터 제거
			// 이벤트 핸들러 제거
		}

		// 비관리 데이터 제거

		m_Disposed = true;
		s_Instance = null;
	}

	public void Dispose()
	{
		Release(true);
		GC.SuppressFinalize(this);
	}
}

public abstract class DataSingleton<TClass> : Singleton<TClass> where TClass : class,new()
{
	private bool m_Disposed = false;

	protected override void Release(bool _disposing)
	{
		if(m_Disposed)
		{
			return;
		}

		if(_disposing)
		{
			ClearAll();
		}

		m_Disposed = true;

		base.Release(_disposing);
	}

	protected abstract void ClearAll();
}