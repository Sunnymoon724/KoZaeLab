
namespace GameData
{
	public record SoundVolumeData(float Level,bool Mute);

	public class SoundOption : Option
	{
		protected override string OPTION_KEY => "Sound Option";
		protected override EventTag Tag => EventTag.ChangeSoundOption;

		private class SoundData
		{
			public float MasterLevel { get; set; }
			public bool MasterMute { get; set; }

			public float MusicLevel { get; set; }
			public bool MusicMute { get; set; }

			public float EffectLevel { get; set; }
			public bool EffectMute { get; set; }
		}

		private SoundData m_SoundData = null;

		public override void Initialize()
		{
			m_SoundData = GetOption(new SoundData()
			{
				MasterLevel		= 1.0f,
				MasterMute		= false,
				MusicLevel		= 1.0f,
				MusicMute		= false,
				EffectLevel		= 1.0f,
				EffectMute		= false,
			});
		}

		public override void Release()
		{
			
		}

		/// <summary>
		/// 마스터 볼륨
		/// </summary>
		public SoundVolumeData MasterVolume
		{
			get => new(m_SoundData.MasterLevel,m_SoundData.MasterMute);
			set
			{
				if((m_SoundData.MasterLevel == value.Level) && (m_SoundData.MasterMute == value.Mute))
				{
					return;
				}

				m_SoundData.MasterLevel = value.Level;
				m_SoundData.MasterMute = value.Mute;

				SaveOption(m_SoundData);
			}
		}

		/// <summary>
		/// 배경음 볼륨
		/// </summary>
		public SoundVolumeData MusicVolume
		{
			get => new(m_SoundData.MusicLevel,m_SoundData.MusicMute);
			set
			{
				if((m_SoundData.MusicLevel == value.Level) && (m_SoundData.MusicMute == value.Mute))
				{
					return;
				}

				m_SoundData.MusicLevel = value.Level;
				m_SoundData.MusicMute = value.Mute;

				SaveOption(m_SoundData);
			}
		}

		/// <summary>
		/// 효과음 볼륨
		/// </summary>
		public SoundVolumeData EffectVolume
		{
			get => new(m_SoundData.EffectLevel,m_SoundData.EffectMute);
			set
			{
				if((m_SoundData.EffectLevel == value.Level) && (m_SoundData.EffectMute == value.Mute))
				{
					return;
				}

				m_SoundData.EffectLevel = value.Level;
				m_SoundData.EffectMute = value.Mute;

				SaveOption(m_SoundData);
			}
		}
	}
}