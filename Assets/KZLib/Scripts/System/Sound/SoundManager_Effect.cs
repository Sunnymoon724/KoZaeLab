using KZLib.Data;
using KZLib.Utilities;
using UnityEngine;

namespace KZLib.Sounds
{
	/// <summary>
	/// Short sound effect (SFX) API for <see cref="SoundManager"/>.
	/// <para>
	/// Playback pipeline: acquire → <see cref="_Initialize_Effect2D"/> / <see cref="_Initialize_Effect3D"/> → <see cref="_PlaySound"/> (immediate).
	/// 2D and 3D share one <see cref="SoundLanePool"/>; lanes return on end-of-clip <c>Tick</c>, manual <see cref="ReleaseEffect"/>, or follow-target destroy.
	/// </para>
	/// </summary>
	public partial class SoundManager : SingletonMB<SoundManager>
	{
		private SoundVolume m_effectVolume = SoundVolume.max;

		/// <summary>Loads and plays a 3D effect at a fixed world position.</summary>
		public AudioSource PlayEffect3D(string audioPath,Vector3 worldPosition,bool ignoreListenerPause = false)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.Sound.E("Audio path is empty");

				return null;
			}

			return PlayEffect3D(ResourceManager.In.GetAudioClip(audioPath),worldPosition,ignoreListenerPause);
		}

		/// <summary>Plays a 3D effect at a fixed world position (no follow target; source stays under pool storage).</summary>
		public AudioSource PlayEffect3D(AudioClip audioClip,Vector3 worldPosition,bool ignoreListenerPause = false)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return null;
			}

			if(!_TryAcquireEffectSource(null,out var audioSource))
			{
				return null;
			}

			audioSource.transform.position = worldPosition;

			_Initialize_Effect3D(audioSource,audioClip,ignoreListenerPause);
			_PlaySound(audioSource);

			return audioSource;
		}

		/// <summary>Loads and plays a 3D follow effect. See <see cref="PlayEffect3D(AudioClip, Transform, bool)"/>.</summary>
		public AudioSource PlayEffect3D(string audioPath,Transform followTarget,bool ignoreListenerPause = false)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.Sound.E("Audio path is empty");

				return null;
			}

			return PlayEffect3D(ResourceManager.In.GetAudioClip(audioPath),followTarget,ignoreListenerPause);
		}

		/// <summary>Plays a 3D effect parented under <paramref name="followTarget"/> until the clip ends, the lane is released, or the target is destroyed.</summary>
		public AudioSource PlayEffect3D(AudioClip audioClip,Transform followTarget,bool ignoreListenerPause = false)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return null;
			}

			if(!followTarget)
			{
				LogChannel.Sound.E("Follow target is null");

				return null;
			}

			if(!_TryAcquireEffectSource(followTarget,out var audioSource))
			{
				return null;
			}

			_Initialize_Effect3D(audioSource,audioClip,ignoreListenerPause);
			_PlaySound(audioSource);

			return audioSource;
		}

		/// <summary>Loads and plays a non-spatial 2D effect (UI, etc.).</summary>
		public AudioSource PlayEffect2D(string audioPath,bool ignoreListenerPause = false)
		{
			if(audioPath.IsEmpty())
			{
				LogChannel.Sound.E("Audio path is empty");

				return null;
			}

			return PlayEffect2D(ResourceManager.In.GetAudioClip(audioPath),ignoreListenerPause);
		}

		/// <summary>Plays a non-spatial 2D effect (source under pool storage, no follow target).</summary>
		public AudioSource PlayEffect2D(AudioClip audioClip,bool ignoreListenerPause = false)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return null;
			}

			if(!_TryAcquireEffectSource(null,out var audioSource))
			{
				return null;
			}

			_Initialize_Effect2D(audioSource,audioClip,ignoreListenerPause);
			_PlaySound(audioSource);

			return audioSource;
		}

		/// <summary>Releases every active effect lane regardless of clip and returns all slots to the pool.</summary>
		public void ReleaseAllEffects()
		{
			m_soundLanePool.ReleaseAll();
		}

		/// <summary>
		/// Releases the first active effect whose <see cref="AudioClip.name"/> matches <paramref name="clipName"/>.
		/// By project convention, clip names are unique; duplicate-name assets do not exist.
		/// </summary>
		public void ReleaseOneEffect(string clipName)
		{
			var result = m_soundLanePool.TryReleaseOneEffect(clipName);

			if(!result)
			{
				LogChannel.Sound.E($"Failed to release effect [{clipName}]");
			}
		}

		/// <summary>Releases the first active effect playing <paramref name="audioClip"/>.</summary>
		public void ReleaseOneEffect(AudioClip audioClip)
		{
			var result = m_soundLanePool.TryReleaseOneEffect(audioClip);

			if(!result)
			{
				LogChannel.Sound.E($"Failed to release effect [{audioClip.name}]");
			}
		}

		/// <summary>
		/// Releases every active effect whose <see cref="AudioClip.name"/> matches <paramref name="clipName"/>.
		/// By project convention, clip names are unique; duplicate-name assets do not exist.
		/// </summary>
		public void ReleaseEffects(string clipName)
		{
			var result = m_soundLanePool.TryReleaseEffects(clipName);

			if(!result)
			{
				LogChannel.Sound.E($"Failed to release effects [{clipName}]");
			}
		}

		/// <summary>Releases every active effect playing <paramref name="audioClip"/>.</summary>
		public void ReleaseEffects(AudioClip audioClip)
		{
			var result = m_soundLanePool.TryReleaseEffects(audioClip);

			if(!result)
			{
				LogChannel.Sound.E($"Failed to release effects [{audioClip.name}]");
			}
		}

		/// <summary>Releases the pooled effect source returned by <see cref="PlayEffect2D"/> / <see cref="PlayEffect3D"/> and returns its lane to the pool.</summary>
		public void ReleaseEffect(AudioSource audioSource)
		{
			var result = m_soundLanePool.TryReleaseEffect(audioSource);

			if(!result)
			{
				LogChannel.Sound.E($"Failed to release effect [{audioSource.name}]");
			}
		}
	}
}