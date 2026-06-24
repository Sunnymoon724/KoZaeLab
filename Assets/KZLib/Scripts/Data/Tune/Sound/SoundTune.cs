using R3;

namespace KZLib.Data
{
	/// <summary>
	/// User sound volumes (master / music / effect). Effect = short sound effect (SFX).
	/// <see cref="KZLib.Sounds.SoundManager"/> subscribes to <see cref="OnChangedSoundVolume"/>.
	/// </summary>
	public class SoundTune : Tune
	{
		protected SoundTune() { }

		private SoundVolume m_masterVolume;
		private SoundVolume m_musicVolume;
		private SoundVolume m_effectVolume;

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
			m_effectVolume	= _LoadValue(nameof(m_effectVolume),SoundVolume.TryParse,SoundVolume.max);
		}

		public void SetMasterVolume(SoundVolume volume)
		{
			_SetVolume(ref m_masterVolume,volume,nameof(m_masterVolume));
		}

		public void SetMasterLevel(float level)
		{
			var volume = m_masterVolume;

			volume.Set(level,volume.mute);

			SetMasterVolume(volume);
		}

		public void SetMasterMute(bool mute)
		{
			var volume = m_masterVolume;

			volume.Set(volume.level,mute);

			SetMasterVolume(volume);
		}

		public void SetMusicVolume(SoundVolume volume)
		{
			_SetVolume(ref m_musicVolume,volume,nameof(m_musicVolume));
		}

		public void SetMusicLevel(float level)
		{
			var volume = m_musicVolume;

			volume.Set(level,volume.mute);

			SetMusicVolume(volume);
		}

		public void SetMusicMute(bool mute)
		{
			var volume = m_musicVolume;

			volume.Set(volume.level,mute);

			SetMusicVolume(volume);
		}

		public void SetEffectVolume(SoundVolume volume)
		{
			_SetVolume(ref m_effectVolume,volume,nameof(m_effectVolume));
		}

		public void SetEffectLevel(float level)
		{
			var volume = m_effectVolume;

			volume.Set(level,volume.mute);

			SetEffectVolume(volume);
		}

		public void SetEffectMute(bool mute)
		{
			var volume = m_effectVolume;

			volume.Set(volume.level,mute);

			SetEffectVolume(volume);
		}

		private void _SetVolume(ref SoundVolume oldValue,SoundVolume newValue,string nameKey)
		{
			_SetValue(ref oldValue,newValue,nameKey,null);
		}
	}
}