using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// BGM & UI [effect -> UI or use EffectClip]
	/// </summary>
	public class SoundMgr : Singleton<SoundMgr>
	{
		private bool m_Disposed = false;

		//? BGM
		private AudioSource m_BGMSource = null;
		public AudioSource BGMSource => m_BGMSource;

		//? UI
		private AudioSource m_UISource = null;

		protected override void Initialize()
		{
			//? Use CameraMgr or Camera main
			m_BGMSource = CameraMgr.HasInstance ? CameraMgr.In.gameObject.GetComponentInChildren<AudioSource>() : Camera.main.gameObject.GetOrAddComponent<AudioSource>();

			if(UIMgr.HasInstance)
			{
				m_UISource = UIMgr.In.gameObject.GetComponentInChildren<AudioSource>();
			}

			Broadcaster.EnableListener(EventTag.ChangeSoundOption,OnChangeSoundOption);

			OnChangeSoundOption();
		}

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				m_BGMSource = null;
				m_UISource = null;

				Broadcaster.DisableListener(EventTag.ChangeSoundOption,OnChangeSoundOption);
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		private void OnChangeSoundOption()
		{
			var option = GameDataMgr.In.Access<GameData.SoundOption>();

			var master = option.MasterVolume;
			var music = option.MusicVolume;
			var effect = option.EffectVolume;

			m_BGMSource.volume = master.volume*music.volume;
			m_BGMSource.mute = master.mute || music.mute;

			m_UISource.volume = master.volume*effect.volume;
			m_UISource.mute= master.mute || effect.mute;
		}

		#region UI
		public void PlayUIShot(string _path,float _volume = 1.0f)
		{
			PlayUIShot(ResMgr.In.GetAudioClip(_path),_volume);
		}

		public void PlayUIShot(AudioClip _clip,float _volume = 1.0f)
		{
			m_UISource.PlayOneShot(_clip,_volume);
		}
		#endregion UI

		#region BGM
		public void ReplayBGM(float? _time = null)
		{
			if(_time != null)
			{
				m_BGMSource.time = _time.Value;
			}

			m_BGMSource.Play();
		}

		public void PlayBGM(string _path,float _time = 0.0f)
		{
			PlayBGM(ResMgr.In.GetAudioClip(_path),_time);
		}

		public void PlayBGM(AudioClip _clip,float _time = 0.0f)
		{
			if(m_BGMSource.clip != null && m_BGMSource.clip.name.IsEqual(_clip.name))
			{
				return;
			}

			m_BGMSource.clip = _clip;
			m_BGMSource.loop = true;
			m_BGMSource.time = _time;

			m_BGMSource.Play();
		}

		public void PauseBGM()
		{
			m_BGMSource.Pause();
		}

		public bool IsPlayingBGM()
		{
			return m_BGMSource.isPlaying;
		}

		public void StopBGM(bool _clear)
		{
			m_BGMSource.Stop();

			if(_clear)
			{
				m_BGMSource.clip = null;
			}
		}
		#endregion BGM
	}
}