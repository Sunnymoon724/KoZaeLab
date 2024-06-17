using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace KZLib
{
	public partial class SoundMgr : LoadSingletonMB<SoundMgr>
	{
		/// <summary>
		/// 배경음
		/// </summary>
		[SerializeField]
		private AudioSource m_MusicSource = null;

		private float m_MusicVolume = 1.0f;
		private bool m_MusicMute = false;

		public bool IsPlaying => m_MusicSource.isPlaying;

		private double m_MusicStartTime = 0.0;

		public string GetNowPlayingName()
		{
			return m_MusicSource.clip == null ? string.Empty : m_MusicSource.clip.name;
		}

		public double GetMusicTime()
		{
			return AudioSettings.dspTime-m_MusicStartTime;
		}

		public void PlayMusic(AudioClip _clip,float _time = 0.0f,float _delay = 0.0f,float _fadeDuration = 0.0f,Action<float> _onPlay = null,Action _onComplete = null)
		{
			var loop = _onComplete == null;

			SetAudioSource(m_MusicSource,_clip,string.Format("[Music] {0}",_clip.name),GetAudioMixerGroup(MIXER_MUSIC),loop,m_MusicMute,m_MusicVolume);

			PlaySound(m_MusicSource,_time,_delay);

			KillTask(m_MusicSource);

			var taskList = new List<Func<UniTask>>();
			var source = new CancellationTokenSource();
			var length = m_MusicSource.clip.length;

			if(_fadeDuration > 0.0f)
			{
				var duration = Mathf.Clamp(_fadeDuration,0.0f,length);

				taskList.Add(async () => { await FadeInAsync(m_MusicSource,m_MusicSource.volume,source.Token,duration); });
			}

			if(_onPlay != null && _onComplete != null)
			{
				taskList.Add(async () => { await PlaySoundAsync(m_MusicSource,source.Token,length,_delay,_onPlay,_onComplete); });
			}

			if(taskList.IsNullOrEmpty())
			{
				return;
			}

			CommonUtility.MergeUniTaskAsync(taskList.ToArray(),source.Token).Forget();

			AddTask(m_MusicSource,source);
		}

		public bool RestartMusic(float _time = 0.0f)
		{
			return RestartSound(m_MusicSource,_time);
		}

		public void StopMusic(bool _clear,float _fadeDuration = 0.0f)
		{
			StopSound(m_MusicSource,_clear,_fadeDuration);
		}

		public void PlayMusicFadeInOut(AudioClip _clip,Vector3 _fadeDuration,float _time = 0.0f,float _delay = 0.0f,int _count = 1)
		{
			SetAudioSource(m_MusicSource,_clip,string.Format("[Music] {0}",_clip.name),GetAudioMixerGroup(MIXER_MUSIC),false,m_MusicMute,m_MusicVolume);

			var source = new CancellationTokenSource();

			PlayFadeInOutLoopAsync(m_MusicSource,_time,_delay,source.Token,_fadeDuration,_count).Forget();

			AddTask(m_MusicSource,source);
		}
	}
}