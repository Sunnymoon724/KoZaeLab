using System;
using KZLib.Utilities;

namespace KZLib
{
	public class ServerClockManager : Singleton<ServerClockManager>
	{
		private TimeSpan m_timeDifference = TimeSpan.Zero;

		private ServerClockManager() { }

		public DateTime LastSyncedAt { get; private set; }

		public DateTime UtcNow => DateTime.UtcNow-m_timeDifference;

		public DateTime Now => UtcNow.ToLocalTime();

		protected override void _Initialize()
		{
			base._Initialize();

			LastSyncedAt = DateTime.UtcNow;
			m_timeDifference = TimeSpan.Zero;
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				LastSyncedAt = default;
				m_timeDifference = TimeSpan.Zero;
			}

			base._Release(disposing);
		}

		public DateTime GetNow(bool isLocal = true)
		{
			return isLocal ? Now : UtcNow;
		}

		public void Sync(long newServerTimestamp)
		{
			Sync(newServerTimestamp.ToDateTime(false));
		}

		public void Sync(DateTime newServerTime)
		{
			m_timeDifference = DateTime.UtcNow-newServerTime;

			LastSyncedAt = DateTime.UtcNow;
		}
	}
}