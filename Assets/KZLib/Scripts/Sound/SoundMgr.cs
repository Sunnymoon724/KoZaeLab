using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	/// <summary>
	/// BGM & UI [effect -> UI or use EffectClip]
	/// </summary>
	public class SoundMgr : Singleton<SoundMgr>
	{
		private bool m_disposed = false;

		//? BGM
		private AudioSource m_bgmSource = null;
		public AudioSource BGMSource => m_bgmSource;

		//? UI
		private AudioSource m_uiSource = null;

		protected override void Initialize()
		{
			//? Use CameraMgr or Camera main
			m_bgmSource = CameraMgr.HasInstance ? CameraMgr.In.gameObject.GetComponentInChildren<AudioSource>() : Camera.main.gameObject.GetOrAddComponent<AudioSource>();

			if(UIMgr.HasInstance)
			{
				m_uiSource = UIMgr.In.gameObject.GetComponentInChildren<AudioSource>();
			}

			var optionConfig = ConfigMgr.In.Access<ConfigData.OptionConfig>();

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

				var optionConfig = ConfigMgr.In.Access<ConfigData.OptionConfig>();

				optionConfig.OnSoundVolumeChange -= OnChangeSoundOption;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void OnChangeSoundOption(SoundVolume masterVolume,SoundVolume musicVolume,SoundVolume effectVolume)
		{
			m_bgmSource.volume = masterVolume.level*musicVolume.level;
			m_bgmSource.mute = masterVolume.mute || musicVolume.mute;

			m_uiSource.volume = masterVolume.level*effectVolume.level;
			m_uiSource.mute= masterVolume.mute || effectVolume.mute;
		}

		#region UI
		public void PlayUIShot(string audioPath,float volume = 1.0f)
		{
			if(audioPath.IsEmpty())
			{
				LogTag.System.I("Audio path is empty");

				return;
			}

			PlayUIShot(ResMgr.In.GetAudioClip(audioPath),volume);
		}

		public void PlayUIShot(AudioClip audioClip,float volume = 1.0f)
		{
			if(!audioClip)
			{
				LogTag.System.I("Audio clip is null");

				return;
			}

			m_uiSource.PlayOneShot(audioClip,volume);
		}
		#endregion UI

		#region BGM
		public void ReplayBGM(float? startTime = null)
		{
			if(startTime != null)
			{
				m_bgmSource.time = startTime.Value;
			}

			m_bgmSource.Play();
		}

		public void PlayBGM(string _path,float startTime = 0.0f)
		{
			if(_path.IsEmpty())
			{
				LogTag.System.I("Audio path is empty");

				return;
			}

			PlayBGM(ResMgr.In.GetAudioClip(_path),startTime);
		}

		public void PlayBGM(AudioClip audioClip,float startTime = 0.0f)
		{
			if(!audioClip)
			{
				LogTag.System.I("Audio clip is null");

				return;
			}

			if(m_bgmSource.clip != null && m_bgmSource.clip.name.IsEqual(audioClip.name))
			{
				return;
			}

			m_bgmSource.clip = audioClip;
			m_bgmSource.loop = true;
			m_bgmSource.time = startTime;

			m_bgmSource.Play();
		}

		public void PauseBGM()
		{
			m_bgmSource.Pause();
		}

		public void ResumeBGM()
		{
			m_bgmSource.Play();
		}

		public bool IsPlayingBGM()
		{
			return m_bgmSource.isPlaying;
		}

		public void StopBGM(bool clearClip)
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