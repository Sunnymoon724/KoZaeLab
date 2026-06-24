using System;
using KZLib.Utilities;
using UnityEngine;

namespace KZLib.Sounds
{
	/// <summary>
	/// Lane activation parameter passed to <see cref="SoundLane.Initialize"/>.
	/// <para>
	/// <see cref="Target"/> is an optional hierarchy parent for 3D follow playback.
	/// <c>null</c> for 2D effects or 3D effects at a fixed world position set by <see cref="SoundManager"/> after acquire.
	/// </para>
	/// </summary>
	/// <param name="Target">Parent transform for follow playback on acquire; <c>null</c> keeps the source under pool storage for 2D or caller-placed fixed 3D effects.</param>
	internal record SoundLaneParam(Transform Target) : ILaneParam;

	/// <summary>
	/// One pooled effect <see cref="AudioSource"/> slot managed by <see cref="LanePool{TLane,TSource}"/>.
	/// <para>
	/// At pool creation each lane is parented under <see cref="SoundLanePool"/> storage.
	/// On acquire, <see cref="Initialize"/> reparents under <see cref="SoundLaneParam.Target"/> for follow playback, or keeps the source under storage when <see cref="SoundLaneParam.Target"/> is <c>null</c>.
	/// <see cref="Release"/> and <see cref="LanePool{TLane,TSource}.Tick"/> return lanes to storage.
	/// <see cref="FollowTargetWatcher"/> on the follow target calls <see cref="Release"/> when the target is destroyed externally.
	/// </para>
	/// <para>
	/// Lifecycle: <see cref="Create"/> → <see cref="Initialize"/> (on acquire) → <see cref="Release"/> (on return).
	/// Clip assignment, spatial settings, and <see cref="AudioSource.Play"/> are handled by <see cref="SoundManager"/> after acquire.
	/// </para>
	/// </summary>
	internal class SoundLane : ILane<AudioSource>
	{
		private AudioSource m_source = null;
		private bool m_isActive = false;

		private string m_defaultName = null;

		public AudioSource Payload => m_source;
		public bool IsActive => m_isActive;

		private Transform m_target = null;
		private Transform m_storage = null;

		/// <summary>Used by <see cref="LanePool{TLane,TSource}.Tick"/> to auto-release lanes after playback ends.</summary>
		public bool IsPlaying => m_source != null && m_source.isPlaying;

		/// <summary>Called once when the pool creates this lane. Binds the <see cref="AudioSource"/> and runs <see cref="_ResetSource"/>.</summary>
		public void Create(AudioSource source,Transform storage)
		{
			if(source == null)
			{
				throw new InvalidOperationException($"{nameof(source)} is null.");
			}

			m_source = source;
			m_storage = storage;

			m_defaultName = source.name;

			_ResetSource();
		}

		/// <summary>
		/// Called by <see cref="LanePool{TLane,TSource}.TryAcquire"/> when the lane is borrowed.
		/// Marks the lane active, reparents the source under <see cref="SoundLaneParam.Target"/> or storage when <c>null</c>, and binds <see cref="FollowTargetWatcher"/> for follow targets. Does not play audio.
		/// </summary>
		public void Initialize(ILaneParam param)
		{
			if(param is not SoundLaneParam soundLaneParam)
			{
				throw new InvalidOperationException($"{nameof(param)} is not {nameof(SoundLaneParam)}.");
			}

			m_isActive = true;

			m_target = soundLaneParam.Target;

			_SetParent(m_target ?? m_storage);

			if(m_target)
			{
				var watcher = m_target.gameObject.GetOrAddComponent<FollowTargetWatcher>();

				watcher.Bind(_ReturnStorage);
			}
		}

		/// <summary>Reparents the pooled source and resets local position and rotation to identity.</summary>
		private void _SetParent(Transform parent)
		{
			var transform = m_source.transform;

			transform.SetParent(parent);
			transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
		}

		/// <summary>
		/// Returns the lane to the pool: stops playback, marks inactive, resets <see cref="AudioSource"/> to pooled defaults, and reparents the source under storage.
		/// Unbinds <see cref="FollowTargetWatcher"/> when the lane had a follow target.
		/// </summary>
		public void Release()
		{
			m_source.Stop();

			m_isActive = false;

			_ResetSource();

			_SetParent(m_storage);

			if(m_target)
			{
				var watcher = m_target.gameObject.GetComponent<FollowTargetWatcher>();

				if(watcher)
				{
					watcher.Unbind(_ReturnStorage);
				}
			}

			m_target = null;
		}

		/// <summary>Destroys the underlying GameObject when <see cref="LanePool{TLane,TSource}.Clear"/> runs.</summary>
		public void Destroy()
		{
			m_source.DestroyObject();
		}

		/// <summary>Clears per-borrow playback state when the lane returns to the pool. Spatial rolloff is applied by <see cref="SoundManager"/> on the next 3D play.</summary>
		private void _ResetSource()
		{
			m_source.name = m_defaultName;

			m_source.playOnAwake = false;
			m_source.loop = false;
			m_source.spatialBlend = 0.0f;
			m_source.pitch = 1.0f;
			m_source.ignoreListenerPause = false;

			m_source.volume = 1.0f;
			m_source.mute = false;

			m_source.clip = null;
		}

		/// <summary>Callback from <see cref="FollowTargetWatcher"/> on the follow target's <c>OnDestroy</c>. Returns the lane via <see cref="Release"/>.</summary>
		private void _ReturnStorage(GameObject _)
		{
			if(!m_isActive)
			{
				return;
			}

			Release();
		}
	}
}