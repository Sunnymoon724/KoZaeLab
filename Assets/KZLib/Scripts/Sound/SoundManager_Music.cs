using KZLib.KZData;
using KZLib.KZUtility;
using UnityEngine;

namespace KZLib
{
	public partial class SoundManager : SingletonMB<SoundManager>
	{
		/// <summary>
		/// 배경음
		/// </summary>
		[SerializeField]
		private AudioSource m_bgmSource = null;

		private SoundVolume m_bgmVolume = SoundVolume.max;

		public bool IsPlaying => m_bgmSource.isPlaying;

		public string GetPlayingBGMName()
		{
			return m_bgmSource.clip == null ? string.Empty : m_bgmSource.clip.name;
		}

		public float GetBGMTime()
		{
			return (float)m_bgmSource.timeSamples/m_bgmSource.clip.frequency;
		}

		public AudioSource SetBGM(string audioPath)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.System.E("Audio path is empty");

				return null;
			}

			return SetBGM(ResourceManager.In.GetAudioClip(audioPath));
		}

		public AudioSource SetBGM(AudioClip audioClip)
		{
			if(!audioClip)
			{
				LogChannel.System.E("Audio clip is null");

				return null;
			}

			_SetAudioSource(m_bgmSource,audioClip,$"[Music] {audioClip.name}",true,m_bgmVolume);

			return m_bgmSource;
		}

		public AudioSource PlayBGM(string audioPath,float time = 0.0f,float delay = 0.0f)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.System.E("Audio path is empty");

				return null;
			}

			return PlayBGM(ResourceManager.In.GetAudioClip(audioPath),time,delay);
		}

		public AudioSource PlayBGM(AudioClip audioClip,float time = 0.0f,float delay = 0.0f)
		{
			if(!SetBGM(audioClip))
			{
				return null;
			}

			_PlaySound(m_bgmSource,time,delay);

			return m_bgmSource;
		}

		public bool RestartBGM(float? time = null)
		{
			return _RestartSound(m_bgmSource,time == null ? m_bgmSource.time : time.Value);
		}

		public void StopBGM(bool clearClip)
		{
			_StopSound(m_bgmSource,clearClip);
		}

		public void PauseBGMSound()
		{
			m_bgmSource.Pause();
		}

		public void ResumeBGMSound()
		{
			m_bgmSource.Play();
		}
	}
}