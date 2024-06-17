using System.Threading;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		private const string RESOURCES = "Resources";
		private const float UPDATE_PERIOD = 0.1f;

		private CancellationTokenSource m_Source = null;

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(m_Source != null)
			{
				m_Source.Cancel();
				m_Source.Dispose();
				m_Source = null;
			}

			if(_disposing)
			{
				foreach(var data in m_CachedDataDict.Values)
				{
					data.Release();
				}

				m_CachedDataDict.Clear();
				m_LoadingQueue.Clear();
			}

			base.Release(_disposing);
		}
	}
}