using System.Collections.Generic;
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
			public Dictionary<SoundType,SoundData> SoundDict { get; set; }

			[JsonIgnore]
			public SoundData Master => SoundDict[SoundType.Master];
			[JsonIgnore]
			public SoundData Music => SoundDict[SoundType.Music];
			[JsonIgnore]
			public SoundData Effect => SoundDict[SoundType.Effect];

			public void SetData(SoundType _type,SoundData _data)
			{
				SoundDict[_type] = _data;
			}
		}

		private Sound m_Sound = null;

		public override void Initialize()
		{
			m_Sound = GetOption(new Sound()
			{
				SoundDict = new Dictionary<SoundType,SoundData>()
				{
					{ SoundType.Master,	new SoundData(1.0f,false) },
					{ SoundType.Music,	new SoundData(1.0f,false) },
					{ SoundType.Effect,	new SoundData(1.0f,false) },
				},
			});
		}

		public override void Release()
		{
			
		}

		/// <summary>
		/// 마스터 볼륨
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

				m_Sound.SetData(SoundType.Master,value);

				SaveOption(m_Sound);
			}
		}

		/// <summary>
		/// 배경음 볼륨
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

				m_Sound.SetData(SoundType.Music,value);

				SaveOption(m_Sound);
			}
		}

		/// <summary>
		/// 효과음 볼륨
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

				m_Sound.SetData(SoundType.Effect,value);

				SaveOption(m_Sound);
			}
		}
	}
}