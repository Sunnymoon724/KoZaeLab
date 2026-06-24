using KZLib.Data;
using KZLib.Utilities;
using UnityEngine;

namespace KZLib.Sounds
{
	/// <summary>Music (BGM) API for <see cref="SoundManager"/>. Uses the dedicated <see cref="m_musicSource"/>; supports delayed start via <see cref="PlayMusic(AudioClip, float, float)"/>.</summary>
	public partial class SoundManager : SingletonMB<SoundManager>
	{
		/// <summary>Dedicated looping music source. Falls back to the component on this GameObject when unset.</summary>
		[SerializeField]
		private AudioSource m_musicSource = null;

		private SoundVolume m_musicVolume = SoundVolume.max;

		/// <summary>Set by <see cref="PauseMusic"/>; cleared by <see cref="ResumeMusic"/>, <see cref="StopMusic"/>, or <see cref="SetMusic"/>.</summary>
		private bool m_musicPaused = false;
		/// <summary>When true, <see cref="ResumeMusic"/> calls <see cref="AudioSource.Play"/> instead of <see cref="AudioSource.UnPause"/> (PlayDelayed was cancelled).</summary>
		private bool m_musicResumeWithPlay = false;

		/// <summary>True while <see cref="m_musicSource"/> is actively playing.</summary>
		public bool IsMusicPlaying => m_musicSource.isPlaying;

		/// <summary>True between <see cref="PauseMusic"/> and <see cref="ResumeMusic"/> (or stop/clear).</summary>
		public bool IsMusicPaused => m_musicPaused;

		/// <summary>Current music clip name, or empty when no clip is assigned.</summary>
		public string GetPlayingMusicName()
		{
			return m_musicSource.clip == null ? string.Empty : m_musicSource.clip.name;
		}

		/// <summary>Playback position in seconds. Returns 0 when there is no clip or frequency is invalid.</summary>
		public float GetMusicTime()
		{
			if(m_musicSource.clip == null || m_musicSource.clip.frequency == 0)
			{
				return 0.0f;
			}

			return (float) m_musicSource.timeSamples/m_musicSource.clip.frequency;
		}

		/// <summary>Loads a clip from Resources and assigns it without playing.</summary>
		public AudioSource SetMusic(string audioPath)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.Sound.E("Audio path is empty");

				return null;
			}

			return SetMusic(ResourceManager.In.GetAudioClip(audioPath));
		}

		/// <summary>Assigns a clip (looping) without playing.</summary>
		public AudioSource SetMusic(AudioClip audioClip)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return null;
			}

			_ClearMusicPauseState();
			_SetAudioSource(m_musicSource,audioClip,$"[Music] {audioClip.name}",true,m_musicVolume);

			return m_musicSource;
		}

		/// <summary>Loads a clip from Resources, assigns it, and starts playback.</summary>
		public AudioSource PlayMusic(string audioPath,float time = 0.0f,float delay = 0.0f)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.Sound.E("Audio path is empty");

				return null;
			}

			return PlayMusic(ResourceManager.In.GetAudioClip(audioPath),time,delay);
		}

		/// <summary>Assigns a clip and starts playback at <paramref name="time"/> (optionally after <paramref name="delay"/> seconds).</summary>
		public AudioSource PlayMusic(AudioClip audioClip,float time = 0.0f,float delay = 0.0f)
		{
			if(SetMusic(audioClip) == null)
			{
				return null;
			}

			_PlaySound(m_musicSource,time,delay);

			return m_musicSource;
		}

		/// <summary>
		/// Re-starts playback from <paramref name="time"/> or the current position.
		/// Returns false when there is no clip, or when not paused and not currently playing.
		/// </summary>
		public bool RestartMusic(float? time = null)
		{
			if(m_musicSource.clip == null)
			{
				return false;
			}

			if(m_musicPaused)
			{
				_ClearMusicPauseState();
			}
			else if(!_IsPlayingSource(m_musicSource))
			{
				return false;
			}

			_PlaySound(m_musicSource,time ?? m_musicSource.time);

			return true;
		}

		/// <summary>Stops music and clears pause flags. Clears the clip when <paramref name="clearClip"/> is true.</summary>
		public void StopMusic(bool clearClip)
		{
			_ClearMusicPauseState();
			_StopSound(m_musicSource,clearClip);
		}

		/// <summary>
		/// Pauses playing music.
		/// When a clip is assigned but not yet playing (e.g. <see cref="AudioSource.PlayDelayed"/>), cancels the scheduled play instead.
		/// </summary>
		public void PauseMusic()
		{
			if(m_musicPaused)
			{
				return;
			}

			if(m_musicSource.isPlaying)
			{
				m_musicPaused = true;
				m_musicResumeWithPlay = false;
				m_musicSource.Pause();

				return;
			}

			if(m_musicSource.clip == null)
			{
				return;
			}

			m_musicPaused = true;
			m_musicResumeWithPlay = true;
			m_musicSource.Stop();
		}

		/// <summary>Resumes music paused via <see cref="PauseMusic"/>.</summary>
		public void ResumeMusic()
		{
			if(!m_musicPaused)
			{
				return;
			}

			m_musicPaused = false;

			if(m_musicSource.clip == null)
			{
				m_musicResumeWithPlay = false;

				return;
			}

			if(m_musicResumeWithPlay)
			{
				m_musicSource.Play();
			}
			else
			{
				m_musicSource.UnPause();
			}

			m_musicResumeWithPlay = false;
		}

		private void _ClearMusicPauseState()
		{
			m_musicPaused = false;
			m_musicResumeWithPlay = false;
		}
	}
}