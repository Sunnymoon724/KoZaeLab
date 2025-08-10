#if KZLIB_PLAY_FAB
using KZLib.KZUtility;

namespace KZLib
{
	public partial class PlayFabMgr : Singleton<PlayFabMgr>
	{
		private bool m_disposed = false;

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				
			}

			m_disposed = true;

			base.Release(disposing);
		}
	}
}
#endif