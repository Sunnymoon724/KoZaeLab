using R3;

namespace KZLib.Data
{
	public class SoundTune : Tune
	{
		private SoundVolume m_masterVolume = SoundVolume.max;
		private SoundVolume m_musicVolume = SoundVolume.max;
		private SoundVolume m_effectVolume = SoundVolume.max;

		public SoundProfile CurrentSound => new(m_masterVolume,m_musicVolume,m_effectVolume);

		public Observable<SoundProfile> OnChangedSoundVolume => Observable.CombineLatest(OnChangedWithStart(nameof(m_masterVolume)),OnChangedWithStart(nameof(m_musicVolume)),OnChangedWithStart(nameof(m_effectVolume)),_GetSoundProfile);

		private SoundProfile _GetSoundProfile(Unit _,Unit __,Unit ___)
		{
			return CurrentSound;
		}

		protected override void _LoadAll()
		{
			m_masterVolume	= _LoadValue(nameof(m_masterVolume),SoundVolume.TryParse,SoundVolume.max);
			m_musicVolume	= _LoadValue(nameof(m_musicVolume),SoundVolume.TryParse,SoundVolume.max);
			m_musicVolume	= _LoadValue(nameof(m_effectVolume),SoundVolume.TryParse,SoundVolume.max);
		}

		public void SetMasterVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolume(ref m_masterVolume,new SoundVolume(level,isMute),nameof(m_masterVolume));
		}

		public void SetMusicVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolume(ref m_musicVolume,new SoundVolume(level,isMute),nameof(m_musicVolume));
		}

		public void SetEffectVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolume(ref m_effectVolume,new SoundVolume(level,isMute),nameof(m_effectVolume));
		}

		private void _SetVolume(ref SoundVolume oldValue,SoundVolume newValue,string nameKey)
		{
			_SetValue(ref oldValue,newValue,nameKey,null);
		}
	}
}