using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib.KZDevelop;
using UnityEngine;
using UnityEngine.Audio;

namespace KZLib
{
	public partial class SoundMgr : LoadSingletonMB<SoundMgr>
	{
		private bool m_AudioPause = false;

		public bool IsAudioPausing => m_AudioPause;

		private readonly Dictionary<AudioSource,CancellationTokenSource> m_TokenDict = new();

		protected override void Initialize()
		{
			if(m_Mixer)
			{
				m_MixerDict.AddRange(m_Mixer.FindMatchingGroups(MIXER_MASTER));
			}

			for(var i=0;i<EFFECT_MAX_COUNT;i++)
			{
				m_EffectList.Add(CreateEffectSource(i));
			}

			Broadcaster.EnableListener(EventTag.ChangeSoundOption,OnChangeSoundOption);

			OnChangeSoundOption();
		}

		protected override void Release()
		{
			foreach(var token in m_TokenDict.Values)
			{
				token?.Cancel();
				token?.Dispose();
			}

			m_MixerDict.Clear();

			m_EffectList.Clear();
			m_TokenDict.Clear();

			Broadcaster.DisableListener(EventTag.ChangeSoundOption,OnChangeSoundOption);
		}

		private void OnChangeSoundOption()
		{
			var option = GameDataMgr.In.Access<GameData.Option>().SoundOption;

			var master = option.MasterVolume;
			var music = option.MusicVolume;
			var effect = option.EffectVolume;

			m_MusicVolume = master.Level*music.Level;
			m_MusicMute = master.Mute || music.Mute;

			m_EffectVolume = master.Level*effect.Level;
			m_EffectMute = master.Mute || effect.Mute;

			m_MusicSource.volume = m_MusicVolume;
			m_MusicSource.mute = m_MusicMute;

			foreach(var source in m_EffectList)
			{
				source.volume = m_EffectVolume;
				source.mute = m_EffectMute;
			}
		}

		private void SetAudioSource(AudioSource _source,AudioClip _clip,string _name,AudioMixerGroup _mixerGroup,bool _loop,bool _mute,float _volume)
		{
			_source.name = _name;
			_source.clip = _clip;
			_source.outputAudioMixerGroup = _mixerGroup;
			_source.loop = _loop;
			_source.pitch = 1.0f;
			_source.ignoreListenerPause = false;

			_source.volume = _volume;
			_source.mute = _mute;
		}

		private void AddTask(AudioSource _source,CancellationTokenSource _token)
		{
			if(m_TokenDict.ContainsKey(_source))
			{
				KillTask(_source);
			}

			m_TokenDict.Add(_source,_token);
		}

		private void KillTask(AudioSource _source)
		{
			if(m_TokenDict.TryGetValue(_source,out var token))
			{
				token?.Cancel();
				token?.Dispose();

				m_TokenDict.RemoveSafe(_source);
			}
		}

		private bool RestartSound(AudioSource _source,float _time = 0.0f,float _delay = 0.0f)
		{
			if(!IsPlayingSource(_source))
			{
				return false;
			}

			PlaySound(_source,_time,_delay);

			return true;
		}

		private void PlaySound(AudioSource _source,float _time = 0.0f,float _delay = 0.0f)
		{
			_source.time = _time;
			_source.PlayScheduled(AudioSettings.dspTime+_delay);
		}

		private void StopSound(AudioSource _source,bool _clear,float _fadeDuration = 0.0f)
		{
			var volume = _source.volume;

			KillTask(_source);

			var source = new CancellationTokenSource();

			FadeOutAsync(_source,_source.volume,source.Token,_fadeDuration,()=>
			{
				_source.Stop();

				KillTask(_source);

				if(_clear)
				{
					_source.clip = null;
				}

				_source.volume = volume;
			}).Forget();

			if(_fadeDuration > 0.0f)
			{
				AddTask(_source,source);
			}
		}

		private async UniTask PlaySoundAsync(AudioSource _source,CancellationToken _token,float _duration,float _delay = 0.0f,Action<float> _onPlay = null,Action _onComplete = null)
		{
			if(_delay != 0.0f)
			{
				await UniTask.WaitForSeconds(_delay);
			}

			var startTime = AudioSettings.dspTime;
			var elapsedTime = 0.0f;

			while(elapsedTime < _duration)
			{
				if(_token.IsCancellationRequested)
				{
					return;
				}

				_onPlay?.Invoke(elapsedTime);

				await UniTask.Yield(_token);

				var current = (float) (AudioSettings.dspTime-startTime);

				if(elapsedTime != current)
				{
					elapsedTime = current;
				}
			}

			_onPlay?.Invoke(_duration);

			_onComplete?.Invoke();
		}

		private async UniTask FadeOutAsync(AudioSource _source,float _volume,CancellationToken _token,float _duration,Action _onComplete = null)
		{
			if(_duration > 0.0f)
			{
				await PlaySoundAsync(_source,_token,_duration,0.0f,(time)=>
				{
					m_MusicSource.volume = Mathf.Lerp(_volume,0.0f,time/_duration);
				});
			}

			_onComplete?.Invoke();
		}

		private async UniTask FadeInAsync(AudioSource _source,float _volume,CancellationToken _token,float _duration,Action _onComplete = null)
		{
			if(_duration > 0.0f)
			{
				await PlaySoundAsync(_source,_token,_duration,0.0f,(time)=>
				{
					m_MusicSource.volume = Mathf.Lerp(0.0f,_volume,time/_duration);
				});
			}

			_onComplete?.Invoke();
		}

		private async UniTask PlayFadeInOutLoopAsync(AudioSource _source,float _time,float _delay,CancellationToken _token,Vector3 _duration,int _count)
		{
			if(_count == 0)
			{
				return;
			}

			var count = _count;
			var volume = m_MusicSource.volume;

			while(count == -1 || count-- > 0)
			{
				if(_token.IsCancellationRequested)
				{
					return;
				}

				await UniTask.WaitForSeconds(_delay);

				PlaySound(m_MusicSource,_time);

				await FadeInAsync(_source,volume,_token,_duration.x);

				if(_duration.y > 0.0f)
				{
					await UniTask.WaitForSeconds(_duration.y);
				}

				await FadeOutAsync(_source,volume,_token,_duration.z);
			}
		}

		private bool IsPlayingSource(AudioSource _source)
		{
			return _source.clip && _source.isPlaying;
		}
	}
}