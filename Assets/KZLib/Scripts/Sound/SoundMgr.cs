using UnityEngine;

namespace KZLib
{
	public class SoundMgr : Singleton<SoundMgr>
	{
		private bool m_Disposed = false;

		//? BGM 관련
		private AudioSource m_BGMSource = null;
		public AudioSource BGMSource => m_BGMSource;

		//? UI 관련
		private AudioSource m_UISource = null;

		protected override void Initialize()
		{
			if(CameraMgr.HasInstance)
			{
				m_BGMSource = CameraMgr.In.gameObject.GetComponentInChildren<AudioSource>();
			}
			else
			{
				//? 카메라 매니저가 없으면 메인 카메라에 오디오 리스너를 생성함
				m_BGMSource = Camera.main.gameObject.GetOrAddComponent<AudioSource>();
			}

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

			var master = option.Master;
			var music = option.Music;
			var effect = option.Effect;

			m_BGMSource.volume = master.Volume*music.Volume;
			m_BGMSource.mute = master.Mute || music.Mute;

			m_UISource.volume = master.Volume*effect.Volume;
			m_UISource.mute= master.Mute || effect.Mute;
		}

		public void PlayUIShot(string _path,float _volume = 1.0f)
		{
			PlayUIShot(ResMgr.In.GetAudioClip(_path),_volume);
		}

		public void PlayUIShot(AudioClip _clip,float _volume = 1.0f)
		{
			m_UISource.PlayOneShot(_clip,_volume);
		}

		public void ReplayBGM(float _time = 0.0f)
		{
			m_BGMSource.time = _time;

			m_BGMSource.Play();
		}

		public void PlayBGM(string _path,float _time = 0.0f)
		{
			PlayBGM(ResMgr.In.GetAudioClip(_path),_time);
		}

		public void PlayBGM(AudioClip _clip,float _time = 0.0f)
		{
			//? 중복 금지
			if(m_BGMSource.clip != null && m_BGMSource.clip.name.IsEqual(_clip.name))
			{
				return;
			}

			m_BGMSource.clip = _clip;
			m_BGMSource.loop = true;
			m_BGMSource.time = _time;

			m_BGMSource.Play();
		}

		public void StopBGM(bool _clear)
		{
			m_BGMSource.Stop();

			if(_clear)
			{
				m_BGMSource.clip = null;
			}
		}
	}
}