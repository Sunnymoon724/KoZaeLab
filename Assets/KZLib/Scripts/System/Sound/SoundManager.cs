using System;
using UnityEngine;
using KZLib.Utilities;
using KZLib.Data;
using R3;
using Sirenix.OdinInspector;

namespace KZLib.Sounds
{
	/// <summary>
	/// Central audio service for music and short sound effects (SFX).
	/// <list type="bullet">
	/// <item><description><b>Music</b> — single looping <see cref="AudioSource"/> on this GameObject. See <c>SoundManager_Music.cs</c>.</description></item>
	/// <item><description><b>Effect2D / Effect3D</b> — pooled <see cref="AudioSource"/> children under <see cref="m_effectStorage"/>. See <c>SoundManager_Effect.cs</c>.</description></item>
	/// </list>
	/// "Effect" means audio SFX, not visual <see cref="KZLib.Effects.EffectManager"/>.
	/// Volumes follow <see cref="SoundTune"/> (master × music / effect).
	/// Effect lanes auto-return via <see cref="SoundLanePool"/> <c>Tick</c> when playback ends.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	[SingletonConfig(AutoCreate = true,PrefabPath = "Prefab/SoundManager",DontDestroy = true)]
	public partial class SoundManager : SingletonMB<SoundManager>
	{
		/// <summary>Idle lanes prepared at init (<see cref="LanePool{TLane,TSource}.Prepare"/>). Serialized; edited via <see cref="EffectPrepareCount"/>.</summary>
		[SerializeField,HideInInspector]
		private int m_effectPoolSize = 8;
		[ShowInInspector,PropertyRange(1,nameof(EffectMaxCount))]
		private int EffectPrepareCount { get => m_effectPoolSize; set => m_effectPoolSize = value; }

		/// <summary>Upper bound on concurrent effect lanes. Serialized; edited via <see cref="EffectMaxCount"/>.</summary>
		[SerializeField,HideInInspector]
		private int m_effectMaxCount = 32;
		[ShowInInspector]
		private int EffectMaxCount { get => m_effectMaxCount; set => m_effectMaxCount = value; }

		[PropertySpace(10)]

		/// <summary>3D rolloff distances: <c>x</c> = <see cref="AudioSource.minDistance"/>, <c>y</c> = <see cref="AudioSource.maxDistance"/>. Serialized; edited via <see cref="Effect3dDistance"/>.</summary>
		[SerializeField,HideInInspector]
		private Vector2 m_effect3dDistance = new(1.0f,500.0f);
		[ShowInInspector,MinMaxSlider(0.0f,1000000.0f,true)]
		private Vector2 Effect3dDistance { get => m_effect3dDistance; set => m_effect3dDistance = value; }

		private SoundTune m_soundTune = null;

		private SoundLanePool m_soundLanePool = null;

		/// <summary>Parent transform for idle pooled effect <see cref="AudioSource"/> objects.</summary>
		[SerializeField]
		private Transform m_effectStorage = null;

		protected override void _Initialize()
		{
			base._Initialize();

			if(!m_musicSource)
			{
				m_musicSource = GetComponent<AudioSource>();
			}

			if(!m_effectStorage)
			{
				throw new InvalidOperationException($"{nameof(m_effectStorage)} is not assigned.");
			}

			m_soundLanePool = new SoundLanePool(m_effectStorage,EffectPrepareCount,EffectMaxCount);
			m_soundLanePool.Prepare();

			m_soundTune = TuneManager.In.Fetch<SoundTune>();
			// OnChangedWithStart on the tune side: first subscription applies saved volumes.
			m_soundTune.OnChangedSoundVolume.Subscribe(_OnChangeSoundOption).RegisterTo(destroyCancellationToken);
		}

		protected override void _Release()
		{
			StopMusic(true);
			ReleaseAllEffects();

			m_soundLanePool.Clear();
			m_soundLanePool = null;

			base._Release();

			m_soundTune = null;
		}

		/// <summary>Releases effect lanes whose <see cref="AudioSource"/> has finished playing.</summary>
		private void Update()
		{
			m_soundLanePool.Tick();
		}

		/// <summary>Applies <see cref="SoundProfile.OutputMusic"/> and <see cref="SoundProfile.OutputEffect"/> to music and active effect lanes.</summary>
		private void _OnChangeSoundOption(SoundProfile soundProfile)
		{
			m_musicVolume = soundProfile.OutputMusic;
			m_effectVolume = soundProfile.OutputEffect;

			m_musicSource.volume = m_musicVolume.level;
			m_musicSource.mute = m_musicVolume.mute;

			m_soundLanePool.ApplyVolume(m_effectVolume.level,m_effectVolume.mute);
		}

		/// <summary>Configures a pooled source for 2D (non-spatial) playback. Does not call <see cref="AudioSource.Play"/>.</summary>
		private AudioSource _Initialize_Effect2D(AudioSource audioSource,AudioClip audioClip,bool ignoreListenerPause)
		{
			_SetAudioSource(audioSource,audioClip,$"[Effect2D] {audioClip.name}",false,m_effectVolume,ignoreListenerPause);

			audioSource.spatialBlend = 0.0f;

			return audioSource;
		}

		/// <summary>Configures a pooled source for 3D playback using <see cref="Effect3dDistance"/> and logarithmic rolloff. Does not call <see cref="AudioSource.Play"/>.</summary>
		private AudioSource _Initialize_Effect3D(AudioSource audioSource,AudioClip audioClip,bool ignoreListenerPause)
		{
			_SetAudioSource(audioSource,audioClip,$"[Effect3D] {audioClip.name}",false,m_effectVolume,ignoreListenerPause);

			audioSource.spatialBlend = 1.0f;
			audioSource.minDistance = Effect3dDistance.x;
			audioSource.maxDistance = Effect3dDistance.y;
			audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

			return audioSource;
		}

		/// <summary>Shared clip / volume setup for music and pooled effects.</summary>
		private void _SetAudioSource(AudioSource audioSource,AudioClip audioClip,string sourceName,bool loop,SoundVolume soundVolume,bool ignoreListenerPause = false)
		{
			audioSource.name = sourceName;
			audioSource.clip = audioClip;
			audioSource.loop = loop;
			audioSource.pitch = 1.0f;
			audioSource.ignoreListenerPause = ignoreListenerPause;

			audioSource.volume = soundVolume.level;
			audioSource.mute = soundVolume.mute;
		}

		/// <summary>Starts playback on a source that already has a clip assigned. <paramref name="delay"/> is for music; effect APIs call with no delay.</summary>
		private void _PlaySound(AudioSource audioSource,float time = 0.0f,float delay = 0.0f)
		{
			audioSource.time = time;

			if(delay <= 0.0f)
			{
				audioSource.Play();
			}
			else
			{
				audioSource.PlayDelayed(delay);
			}
		}

		/// <summary>True when the source has a clip and <see cref="AudioSource.isPlaying"/> is true.</summary>
		private bool _IsPlayingSource(AudioSource audioSource)
		{
			return audioSource.clip && audioSource.isPlaying;
		}

		/// <summary>Stops the source and optionally clears <see cref="AudioSource.clip"/>.</summary>
		private void _StopSound(AudioSource audioSource,bool clearClip)
		{
			audioSource.Stop();

			if(clearClip)
			{
				audioSource.clip = null;
			}
		}

		/// <summary>
		/// Borrows an idle lane from <see cref="m_soundLanePool"/>.
		/// <see cref="LanePool{TLane,TSource}.TryAcquire"/> calls <see cref="SoundLane.Initialize"/>, which reparents the source under <paramref name="followTarget"/> or keeps it under storage when <c>null</c>.
		/// Clip assignment and <see cref="_PlaySound"/> are the caller's responsibility afterward.
		/// </summary>
		/// <param name="followTarget">
		/// Optional parent for 3D follow playback (source moves with the target via hierarchy).
		/// Pass <c>null</c> for 2D or fixed world position; set <see cref="Transform.position"/> on the returned source after acquire when needed.
		/// </param>
		/// <returns><c>false</c> when no idle lane is available (pool at capacity).</returns>
		private bool _TryAcquireEffectSource(Transform followTarget,out AudioSource audioSource)
		{
			audioSource = m_soundLanePool.TryAcquireSource(followTarget);

			if(!audioSource)
			{
				LogChannel.Sound.E("Sound lane is empty");

				return false;
			}

			return true;
		}
	}
}