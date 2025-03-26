using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using System;
using DG.Tweening;

namespace KZLib
{
	public class SoundMgr : Singleton<SoundMgr>
	{
		private bool m_disposed = false;

		//? BGM
		private AudioSource m_bgmSource = null;
		public AudioSource BGMSource => m_bgmSource;

		//? UI
		private AudioSource m_uiSource = null;
		public AudioSource UISource => m_uiSource;

		private SoundVolume m_effectVolume = SoundVolume.zero;

		private WeakReference<ConfigData.OptionConfig> m_optionRef = null;

		protected override void Initialize()
		{
			//? Use CameraMgr or Camera main
			m_bgmSource = CameraMgr.HasInstance ? CameraMgr.In.gameObject.GetComponentInChildren<AudioSource>() : Camera.main.gameObject.GetOrAddComponent<AudioSource>();

			if(UIMgr.HasInstance)
			{
				m_uiSource = UIMgr.In.gameObject.GetComponentInChildren<AudioSource>();
			}

			var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			optionCfg.OnSoundVolumeChange += OnChangeSoundOption;

			m_optionRef = new WeakReference<ConfigData.OptionConfig>(optionCfg);

			OnChangeSoundOption(optionCfg.MasterVolume,optionCfg.MusicVolume,optionCfg.EffectVolume);
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				if(m_optionRef.TryGetTarget(out var optionCfg))
				{
					optionCfg.OnSoundVolumeChange -= OnChangeSoundOption;
				}

				m_optionRef = null;

				m_bgmSource = null;
				m_uiSource = null;
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

			return TryPlayUISound(ResMgr.In.GetAudioClip(audioPath),volume);
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

			return TryPlaySFXSound(source,ResMgr.In.GetAudioClip(audioPath),volume);
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

			return TryPlayBGMSound(ResMgr.In.GetAudioClip(audioPath),startTime);
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

		public Tween PlayBGMInFade(float volume,float duration)
		{
			var bgmVolume = Mathf.Clamp01(volume);

			return DOTween.To(() => m_bgmSource.volume,x => m_bgmSource.volume = x,bgmVolume,duration);
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