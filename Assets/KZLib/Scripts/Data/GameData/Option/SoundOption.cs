using Newtonsoft.Json;

namespace GameData
{
	public enum SoundType { Master, Music, Effect }

	public class SoundOption : Option
	{
		protected override string OPTION_KEY => "Sound Option";
		protected override EventTag Tag => EventTag.ChangeSoundOption;

		private class SoundData
		{
			[JsonProperty("MasterVolume")]
			private SoundVolume m_MasterVolume = new(1.0f,false);

			[JsonIgnore]
			public SoundVolume MasterVolume => m_MasterVolume;

			public bool SetMasterVolume(SoundVolume _sound)
			{
				if(m_MasterVolume == _sound)
				{
					return false;
				}

				m_MasterVolume = _sound;

				return true;
			}

			[JsonProperty("MusicVolume")]
			private SoundVolume m_MusicVolume = new(1.0f,false);

			[JsonIgnore]
			public SoundVolume MusicVolume => m_MasterVolume;

			public bool SetMusicVolume(SoundVolume _sound)
			{
				if(m_MusicVolume == _sound)
				{
					return false;
				}

				m_MusicVolume = _sound;

				return true;
			}

			[JsonProperty("EffectVolume")]
			private SoundVolume m_EffectVolume = new(1.0f,false);

			[JsonIgnore]
			public SoundVolume EffectVolume => m_MasterVolume;

			public bool SetEffectVolume(SoundVolume _sound)
			{
				if(m_EffectVolume == _sound)
				{
					return false;
				}

				m_EffectVolume = _sound;

				return true;
			}
		}

		private SoundData m_SoundData = null;

		public override void Initialize()
		{
			base.Initialize();

			LoadOption(ref m_SoundData);
		}

		public override void Release()
		{
			SaveOption(m_SoundData,false);
		}

		/// <summary>
		/// Master Sound
		/// </summary>
		public SoundVolume MasterVolume
		{
			get => m_SoundData.MasterVolume;
			set
			{
				if(m_SoundData.SetMasterVolume(value))
				{
					return;
				}

				LogTag.Data.I($"Master Volume is changed. [{value}]");

				SaveOption(m_SoundData,true);
			}
		}

		/// <summary>
		/// Background Music
		/// </summary>
		public SoundVolume MusicVolume
		{
			get => m_SoundData.MusicVolume;
			set
			{
				if(m_SoundData.SetMusicVolume(value))
				{
					return;
				}

				LogTag.Data.I($"Music Volume is changed. [{value}]");

				SaveOption(m_SoundData,true);
			}
		}

		/// <summary>
		/// Sound Effect 
		/// </summary>
		public SoundVolume EffectVolume
		{
			get => m_SoundData.EffectVolume;
			set
			{
				if(m_SoundData.SetEffectVolume(value))
				{
					return;
				}

				LogTag.Data.I($"Effect Volume is changed. [{value}]");

				SaveOption(m_SoundData,true);
			}
		}
	}
}