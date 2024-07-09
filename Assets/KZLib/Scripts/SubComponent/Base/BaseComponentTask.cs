using System.Threading;

public abstract class BaseComponentTask : BaseComponent
{
	protected CancellationTokenSource m_Source = null;

	protected override void OnDisable()
	{
		base.OnDisable();

		m_Source?.Cancel();
	}

	protected override void Release()
	{
		base.Release();

		ReleaseTokenSource();
	}

	protected void ReleaseTokenSource()
	{
		if(m_Source != null)
		{
			m_Source.Cancel();
			m_Source.Dispose();

			m_Source = null;
		}
	}

	protected void InitializeTokenSource()
	{
		ReleaseTokenSource();

		m_Source = new();
	}
}