using System;

public abstract class Singleton<TClass> : IDisposable where TClass : class, new()
{
	private static volatile TClass m_Instance = null;

	protected bool m_Disposed = false;

	public static TClass In
	{
		get
		{
			m_Instance ??= new TClass();

			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

	protected Singleton() => Initialize();
	~Singleton() => Release(false);

	protected virtual void Initialize() { }

	protected virtual void Release(bool _disposing)
	{
		if(m_Disposed)
		{
			return;
		}

		if(_disposing) { }

		m_Disposed = true;
		m_Instance = null;
	}

	public void Dispose()
	{
		Release(true);
		GC.SuppressFinalize(this);
	}
}

public abstract class DataSingleton<TClass> : Singleton<TClass> where TClass : class,new()
{
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

		base.Release(_disposing);
	}

	protected abstract void ClearAll();
}