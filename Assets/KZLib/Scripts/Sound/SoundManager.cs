using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using System;
using R3;

namespace KZLib
{
    [RequireComponent(typeof(AudioSource))]
	[SingletonConfig(AutoCreate = true,PrefabPath = "Prefab/SoundManager",DontDestroy = true)]
    public partial class SoundManager : SingletonMB<SoundManager>
	{
		private IDisposable m_soundSubscription;

		protected override void _Initialize()
		{
			base._Initialize();

			if(m_bgmSource)
			{
				m_bgmSource = GetComponent<AudioSource>();
			}

			for(var i=0;i<c_sfxCount;i++)
			{
				m_sfxList.Add(CreateSFX(i));
			}

			var optionCfg = ConfigManager.In.Access<OptionConfig>();

			optionCfg.OnChangedSoundVolume.Subscribe(_OnChangeSoundOption).RegisterTo(destroyCancellationToken);

			_OnChangeSoundOption(optionCfg.CurrentSound);
		}

		protected override void _Release()
		{
			base._Release();

			m_sfxList.Clear();
		}

		private void _OnChangeSoundOption(SoundProfile soundProfile)
		{
			m_bgmVolume = soundProfile.OutputMusic;
			m_sfxVolume = soundProfile.OutputEffect;

			m_bgmSource.volume = m_bgmVolume.level;
			m_bgmSource.mute = m_bgmVolume.mute;

			for(var i=0;i<m_sfxList.Count;i++)
			{
				var source = m_sfxList[i];

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
					LogSvc.System.W($"Delay time is negative: {delay}");
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