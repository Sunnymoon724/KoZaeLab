﻿using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public partial class SoundMgr : LoadSingletonMB<SoundMgr>
	{
		protected override void Initialize()
		{
			if(m_bgmSource)
			{
				m_bgmSource = GetComponent<AudioSource>();
			}

			for(var i=0;i<c_sfxCount;i++)
			{
				m_sfxList.Add(CreateSFX(i));
			}

			var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			optionCfg.OnSoundVolumeChanged += _OnChangeSoundOption;

			_OnChangeSoundOption(optionCfg.MasterVolume,optionCfg.MusicVolume,optionCfg.EffectVolume);
		}

		protected override void Release()
		{
			if(ConfigMgr.HasInstance)
			{
				var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

				optionCfg.OnSoundVolumeChanged -= _OnChangeSoundOption;
			}

			m_sfxList.Clear();
		}

		private void _OnChangeSoundOption(SoundVolume masterVolume,SoundVolume musicVolume,SoundVolume effectVolume)
		{
			m_bgmVolume = new SoundVolume(masterVolume.level*musicVolume.level,masterVolume.mute || musicVolume.mute);
			m_sfxVolume = new SoundVolume(masterVolume.level*effectVolume.level,masterVolume.mute || effectVolume.mute);

			m_bgmSource.volume = m_bgmVolume.level;
			m_bgmSource.mute = m_bgmVolume.mute;

			foreach(var source in m_sfxList)
			{
				source.volume = m_sfxVolume.level;
				source.mute = m_sfxVolume.mute;
			}
		}

		private void _SetAudioSource(AudioSource audioSource,AudioClip audioClip,string sourceName,bool loop,SoundVolume soundVolume)
		{
			audioSource.name = sourceName;
			audioSource.clip = audioClip;
			audioSource.loop = loop;
			audioSource.pitch = 1.0f;
			audioSource.ignoreListenerPause = false;

			audioSource.volume = soundVolume.level;
			audioSource.mute = soundVolume.mute;
		}

		private void _PlaySound(AudioSource audioSource,float time = 0.0f,float delay = 0.0f)
		{
			audioSource.time = time;

			if(delay == 0.0f)
			{
				audioSource.Play();
			}
			else
			{
				if(delay < 0.0f)
				{
					LogTag.System.W($"Delay time is negative: {delay}");
				}

				audioSource.PlayDelayed(delay);
			}
		}

		private bool _RestartSound(AudioSource audioSource,float time = 0.0f,float delay = 0.0f)
		{
			if(!_IsPlayingSource(audioSource))
			{
				return false;
			}

			_PlaySound(audioSource,time,delay);

			return true;
		}

		private bool _IsPlayingSource(AudioSource audioSource)
		{
			return audioSource.clip && audioSource.isPlaying;
		}

		private void _StopSound(AudioSource audioSource,bool clearClip)
		{
			audioSource.Stop();

			if(clearClip)
			{
				audioSource.clip = null;
			}
		}
	}
}