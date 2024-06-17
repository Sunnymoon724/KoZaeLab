using KZLib;
using KZLib.KZDevelop;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public partial class Option : IGameData
	{
		private const string SOUND_OPTION = "Sound Option";

		public partial class Sound
		{
			[SerializeField,JsonProperty("MasterLevel")]
			private float m_MasterLevel = 1.0f;
			[SerializeField,JsonProperty("MasterMute")]
			private bool m_MasterMute = false;

			[JsonIgnore]
			/// <summary>
			/// 마스터 볼륨
			/// </summary>
			public SoundVolumeData MasterVolume
			{
				get => new(m_MasterLevel,m_MasterMute);
				set
				{
					if((m_MasterLevel == value.Level) && (m_MasterMute == value.Mute))
					{
						return;
					}

					m_MasterLevel = value.Level;
					m_MasterMute = value.Mute;

					SaveSound();
				}
			}

			[SerializeField,JsonProperty("MusicLevel")]
			private float m_MusicLevel = 1.0f;
			[SerializeField,JsonProperty("MusicMute")]
			private bool m_MusicMute = false;

			[JsonIgnore]
			/// <summary>
			/// 배경음 볼륨
			/// </summary>
			public SoundVolumeData MusicVolume
			{
				get => new(m_MusicLevel,m_MusicMute);
				set
				{
					if((m_MusicLevel == value.Level) && (m_MusicMute == value.Mute))
					{
						return;
					}

					m_MusicLevel = value.Level;
					m_MusicMute = value.Mute;

					SaveSound();
				}
			}

			[SerializeField,JsonProperty("EffectLevel")]
			private float m_EffectLevel = 1.0f;
			[SerializeField,JsonProperty("EffectMute")]
			private bool m_EffectMute = false;

			[JsonIgnore]
			/// <summary>
			/// 효과음 볼륨
			/// </summary>
			public SoundVolumeData EffectVolume
			{
				get => new(m_EffectLevel,m_EffectMute);
				set
				{
					if((m_EffectLevel == value.Level) && (m_EffectMute == value.Mute))
					{
						return;
					}

					m_EffectLevel = value.Level;
					m_EffectMute = value.Mute;

					SaveSound();
				}
			}

			public Sound()
			{
				m_MasterLevel = 1.0f;
				m_MasterMute = false;

				m_MusicLevel = 1.0f;
				m_MusicMute = false;

				m_EffectLevel = 1.0f;
				m_EffectMute = false;
			}

			private void SaveSound()
			{
				s_SaveHandler.SetObject(SOUND_OPTION,this);

				Broadcaster.SendEvent(EventTag.ChangeSoundOption);
			}
		}

		public Sound SoundOption { get; private set; }

		private void InitializeSound()
		{
			SoundOption = s_SaveHandler.GetObject(SOUND_OPTION,new Sound());
		}

		private void ReleaseSound() { }
	}
}