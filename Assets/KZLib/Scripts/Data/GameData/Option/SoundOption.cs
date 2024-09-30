using Newtonsoft.Json;

namespace GameData
{
	public enum SoundType { Master, Music, Effect }
	public record SoundData(float Volume,bool Mute);

	public class SoundOption : Option
	{
		protected override string OPTION_KEY => "Sound Option";
		protected override EventTag Tag => EventTag.ChangeSoundOption;

		private class Sound
		{
			[JsonProperty("Master")]
			public SoundData Master { get; set; }

			[JsonProperty("Music")]
			public SoundData Music { get; set; }

			[JsonProperty("Effect")]
			public SoundData Effect { get; set; }
		}

		private Sound m_Sound = null;

		public override void Initialize()
		{
			m_Sound = GetOption(new Sound()
			{
				Master	= new SoundData(1.0f,false),
				Music	= new SoundData(1.0f,false),
				Effect	= new SoundData(1.0f,false),
			});
		}

		public override void Release()
		{
			
		}

		/// <summary>
		/// Master Sound
		/// </summary>
		public SoundData Master
		{
			get => m_Sound.Master;
			set
			{
				if(m_Sound.Master == value)
				{
					return;
				}

				m_Sound.Master = value;

				SaveOption(m_Sound);
			}
		}

		/// <summary>
		/// Background Music
		/// </summary>
		public SoundData Music
		{
			get => m_Sound.Music;
			set
			{
				if(m_Sound.Music == value)
				{
					return;
				}

				m_Sound.Music = value;

				SaveOption(m_Sound);
			}
		}

		/// <summary>
		/// Sound Effect 
		/// </summary>
		public SoundData Effect
		{
			get => m_Sound.Effect;
			set
			{
				if(m_Sound.Effect == value)
				{
					return;
				}

				m_Sound.Effect = value;

				SaveOption(m_Sound);
			}
		}
	}
}