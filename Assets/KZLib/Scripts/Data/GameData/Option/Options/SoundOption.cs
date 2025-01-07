using System.Collections.Generic;
using KZLib.KZData;
using KZLib.KZUtility;
using Newtonsoft.Json;

namespace GameData
{
	public class SoundOption : Option
	{
		private enum SoundType { Master, Music, Effect }

		protected override string OptionKey => "Sound_Option";
		protected override EventTag OptionTag => EventTag.ChangeSoundOption;

		private class SoundData : IOptionData
		{
			private readonly Dictionary<SoundType,SoundVolume> m_volumeDict = new()
			{
				{ SoundType.Master,	new(1.0f,false) },
				{ SoundType.Music,	new(1.0f,false) },
				{ SoundType.Effect,	new(1.0f,false) },
			};

			public bool TrySetVolume(SoundType soundType,SoundVolume newVolume)
			{
				if(m_volumeDict.TryAdd(soundType,newVolume))
				{
					return true;
				}

				var oldVolume = m_volumeDict[soundType];

				if(oldVolume == newVolume)
				{
					return false;
				}

				m_volumeDict[soundType] = newVolume;

				return true;
			}

			private SoundVolume GetOrCreateVolume(SoundType soundType)
			{
				if(!m_volumeDict.TryGetValue(soundType,out var volume))
				{
					volume = new SoundVolume(1.0f,false);

					m_volumeDict.Add(soundType,volume);
				}

				return volume;
			}

			[JsonIgnore] public SoundVolume MasterVolume => GetOrCreateVolume(SoundType.Master);
			[JsonIgnore] public SoundVolume MusicVolume => GetOrCreateVolume(SoundType.Music);
			[JsonIgnore] public SoundVolume EffectVolume => GetOrCreateVolume(SoundType.Effect);
		}

		private SoundData m_soundData = null;

		public override void Initialize()
		{
			base.Initialize();

			m_soundData = LoadOptionData<SoundData>();
		}

		public override void Release()
		{
			SaveOptionData(m_soundData,false);
		}

		/// <summary>
		/// Master Sound
		/// </summary>
		public SoundVolume MasterVolume
		{
			get => m_soundData.MasterVolume;
			set => SetVolume(SoundType.Master,value);
		}

		/// <summary>
		/// Background Music
		/// </summary>
		public SoundVolume MusicVolume
		{
			get => m_soundData.MusicVolume;
			set => SetVolume(SoundType.Music,value);
		}

		/// <summary>
		/// Sound Effect 
		/// </summary>
		public SoundVolume EffectVolume
		{
			get => m_soundData.EffectVolume;
			set => SetVolume(SoundType.Effect,value);
		}

		private void SetVolume(SoundType soundType,SoundVolume volume)
		{
			if(m_soundData.TrySetVolume(soundType,volume))
			{
				return;
			}

			LogTag.System.I($"{soundType} Volume is changed. [{volume}]");

			SaveOptionData(m_soundData,true);
		}
	}
}