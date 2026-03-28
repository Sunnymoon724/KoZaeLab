using R3;

namespace KZLib.Data
{
	public class NativeTune : Tune
	{
        private bool m_useVibration = true;
		public bool UseVibration => m_useVibration;

		public Observable<bool> OnChangedVibration => OnChangedWithStart(nameof(m_useVibration)).Select(_GetVibration);

		private bool _GetVibration(Unit _)
		{
			return UseVibration;
		}


		private bool m_useNotification = true;
		public bool UseNotification => m_useNotification;

		public Observable<bool> OnChangedNotification => OnChangedWithStart(nameof(m_useNotification)).Select(_GetNotification);

		private bool _GetNotification(Unit _)
		{
			return UseNotification;
		}

		private bool m_useNightNotification = true;
		public bool UseNightNotification => m_useNightNotification;

		public Observable<bool> OnChangedNightNotification => OnChangedWithStart(nameof(m_useNightNotification)).Select(_GetNightNotification);

		private bool _GetNightNotification(Unit _)
		{
			return UseNightNotification;
		}

		protected override void _LoadAll()
		{
			m_useVibration	= _LoadValue(nameof(m_useVibration),bool.TryParse,true);
			m_useNotification	= _LoadValue(nameof(m_useNotification),bool.TryParse,true);
			m_useNightNotification = _LoadValue(nameof(m_useNightNotification),bool.TryParse,true);
		}

		public void SetVibration(bool newVibration)
		{
			_SetValue(ref m_useVibration,newVibration,nameof(m_useVibration),null);
		}

		public void SetNotification(bool newNotification)
		{
			_SetValue(ref m_useNotification,newNotification,nameof(m_useNotification),null);
		}

		public void SetNightNotification(bool newNightNotification)
		{
			_SetValue(ref m_useNightNotification,newNightNotification,nameof(m_useNightNotification),null);
		}
	}
}