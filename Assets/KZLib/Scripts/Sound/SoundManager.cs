using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class SoundManager : Singleton<SoundManager>
	{
		private bool m_disposed = false;

		//? BGM
		private AudioSource m_bgmSource = null;
		public AudioSource BGMSource => m_bgmSource;

		//? UI
		private AudioSource m_uiSource = null;
		public AudioSource UISource => m_uiSource;

		private SoundVolume m_effectVolume = SoundVolume.zero;

		protected override void Initialize()
		{
			//? Use CameraManager or Camera main
			m_bgmSource = CameraManager.HasInstance ? CameraManager.In.gameObject.GetComponentInChildren<AudioSource>() : Camera.main.gameObject.GetOrAddComponent<AudioSource>();

			if(UIManager.HasInstance)
			{
				m_uiSource = UIManager.In.gameObject.GetComponentInChildren<AudioSource>();
			}

			var optionConfig = ConfigManager.In.Access<ConfigData.OptionConfig>();

			optionConfig.OnSoundVolumeChange += OnChangeSoundOption;

			OnChangeSoundOption(optionConfig.MasterVolume,optionConfig.MusicVolume,optionConfig.EffectVolume);
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_bgmSource = null;
				m_uiSource = null;

				var optionConfig = ConfigManager.In.Access<ConfigData.OptionConfig>();

				optionConfig.OnSoundVolumeChange -= OnChangeSoundOption;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void OnChangeSoundOption(SoundVolume masterVolume,SoundVolume musicVolume,SoundVolume effectVolume)
		{
			m_bgmSource.volume = masterVolume.level*musicVolume.level;
			m_bgmSource.mute = masterVolume.mute || musicVolume.mute;

			m_effectVolume = new SoundVolume(masterVolume.level*effectVolume.level,masterVolume.mute || effectVolume.mute);

			m_uiSource.volume = m_effectVolume.level;
			m_uiSource.mute = m_effectVolume.mute;
		}

		#region UI
		public bool TryPlayUISound(string audioPath,float volume = 1.0f)
		{
			if(audioPath.IsEmpty())
			{
				LogTag.System.E("Audio path is empty");

				return false;
			}

			return TryPlayUISound(ResourceManager.In.GetAudioClip(audioPath),volume);
		}

		public bool TryPlayUISound(AudioClip audioClip,float volume = 1.0f)
		{
			return TryPlaySFXSound(m_uiSource,audioClip,volume);
		}

		public void StopUISound()
		{
			StopSFXSound(m_uiSource);
		}
		#endregion UI

		#region SFX
		public bool TryPlaySFXSound(AudioSource source,string audioPath,float volume = 1.0f)
		{
			if(!source)
			{
				LogTag.System.E("AudioSource is null");

				return false;
			}

			if(audioPath.IsEmpty())
			{
				LogTag.System.E("Audio path is empty");

				return false;
			}

			return TryPlaySFXSound(source,ResourceManager.In.GetAudioClip(audioPath),volume);
		}

		public bool TryPlaySFXSound(AudioSource source,AudioClip audioClip,float volume = 1.0f)
		{
			if(!source)
			{
				LogTag.System.E("Audio Source is null");

				return false;
			}

			if(!audioClip)
			{
				LogTag.System.E("Audio clip is null");

				return false;
			}

			source.volume = m_effectVolume.level;
			source.mute = m_effectVolume.mute;

			source.PlayOneShot(audioClip,volume);

			return true;
		}

		public void StopSFXSound(AudioSource source)
		{
			if(source)
			{
				source.Stop();
			}
		}
		#endregion SFX

		#region BGM
		public bool TryPlayBGMSound(string audioPath,float startTime = 0.0f)
		{
			if(audioPath.IsEmpty())
			{
				LogTag.System.E("Audio path is empty");

				return false;
			}

			return TryPlayBGMSound(ResourceManager.In.GetAudioClip(audioPath),startTime);
		}

		public bool TryPlayBGMSound(AudioClip audioClip,float startTime = 0.0f)
		{
			if(!audioClip)
			{
				LogTag.System.E("Audio clip is null");

				return false;
			}

			if(m_bgmSource.clip != null && m_bgmSource.clip.name.IsEqual(audioClip.name))
			{
				LogTag.System.W($"{audioClip.name} is already played");

				return false;
			}

			m_bgmSource.clip = audioClip;
			m_bgmSource.loop = true;
			m_bgmSource.time = startTime;

			m_bgmSource.Play();

			return true;
		}

		public void ReplayBGMSound(float? startTime = null)
		{
			if(startTime != null)
			{
				m_bgmSource.time = startTime.Value;
			}

			m_bgmSource.Play();
		}

		public void PauseBGMSound()
		{
			m_bgmSource.Pause();
		}

		public void ResumeBGMSound()
		{
			m_bgmSource.Play();
		}

		public bool IsPlayingBGMSound()
		{
			return m_bgmSource.isPlaying;
		}

		public void StopBGMSound(bool clearClip)
		{
			m_bgmSource.Stop();

			if(clearClip)
			{
				m_bgmSource.clip = null;
			}
		}
		#endregion BGM
	}
}