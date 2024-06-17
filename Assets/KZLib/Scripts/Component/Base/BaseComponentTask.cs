using System.Threading;

public abstract class BaseComponentTask : BaseComponent
{
	protected CancellationTokenSource m_Source = null;

	protected virtual void OnDisable()
	{
		m_Source?.Cancel();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if(m_Source != null)
		{
			m_Source.Cancel();
			m_Source.Dispose();

			m_Source = null;
		}
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