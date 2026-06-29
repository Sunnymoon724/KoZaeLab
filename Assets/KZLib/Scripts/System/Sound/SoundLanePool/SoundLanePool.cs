using KZLib.Utilities;
using UnityEngine;

namespace KZLib.Sounds
{
	/// <summary>
	/// Pooled effect lanes backed by <see cref="LanePool{TLane,TSource}"/>.
	/// Shared by <see cref="SoundManager"/> for 2D and 3D effect playback.
	/// </summary>
	internal class SoundLanePool : LanePool<SoundLane,AudioSource>
	{
		/// <summary>Creates effect lanes under <paramref name="storage"/>.</summary>
		/// <param name="storage">Parent for idle <see cref="AudioSource"/> objects.</param>
		/// <param name="prepareCount">Lanes created by <see cref="LanePool{TLane,TSource}.Prepare"/>.</param>
		/// <param name="maxCount">Maximum concurrent lanes.</param>
		public SoundLanePool(Transform storage,int prepareCount,int maxCount) : base(storage,prepareCount,maxCount) { }

		/// <summary>
		/// Borrows an idle lane and activates it via <see cref="SoundLane.Initialize"/>.
		/// Returns the lane <see cref="AudioSource"/>; <c>null</c> when the pool is at capacity.
		/// </summary>
		/// <param name="followTarget">
		/// Optional parent for follow playback; <see cref="SoundLane.Initialize"/> sets the pooled source parent under it with local identity.
		/// <c>null</c> keeps the source under pool storage; the caller sets world position for fixed 3D playback.
		/// </param>
		public AudioSource TryAcquireSource(Transform followTarget)
		{
			if(!TryAcquire(new SoundLaneParam(followTarget),out var lane))
			{
				return null;
			}

			return lane.Payload;
		}

		/// <summary>Applies global effect volume from <see cref="SoundManager"/> to every active lane.</summary>
		public void ApplyVolume(float level,bool mute)
		{
			void _ApplyVolume(SoundLane lane)
			{
				lane.Payload.volume = level;
				lane.Payload.mute = mute;
			}

			ForEachActive(_ApplyVolume);
		}

		/// <summary>Releases the lane that owns <paramref name="audioSource"/>.</summary>
		public bool TryReleaseEffect(AudioSource audioSource)
		{
			if(!audioSource)
			{
				LogChannel.Sound.E("Audio source is null");

				return false;
			}

			bool _FindLane(SoundLane lane)
			{
				return lane.Payload == audioSource;
			}

			return TryReleaseActive(_FindLane);
		}

		/// <summary>
		/// Releases the first active lane whose <see cref="AudioClip.name"/> matches <paramref name="clipName"/>.
		/// By project convention, clip names are unique; duplicate-name assets do not exist.
		/// </summary>
		public bool TryReleaseOneEffect(string clipName)
		{
			if(clipName.IsEmpty())
			{
				LogChannel.Sound.E("Clip name is empty");

				return false;
			}

			bool _FindLane(SoundLane lane)
			{
				return _MatchesClipName(lane,clipName);
			}

			return TryReleaseActive(_FindLane);
		}

		/// <summary>Releases the first active lane playing <paramref name="audioClip"/>.</summary>
		public bool TryReleaseOneEffect(AudioClip audioClip)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return false;
			}

			bool _FindLane(SoundLane lane)
			{
				return _MatchesClip(lane,audioClip);
			}

			return TryReleaseActive(_FindLane);
		}

		/// <summary>
		/// Releases every active lane whose <see cref="AudioClip.name"/> matches <paramref name="clipName"/>.
		/// By project convention, clip names are unique; duplicate-name assets do not exist.
		/// </summary>
		/// <returns>True when at least one matching lane was released.</returns>
		public bool TryReleaseEffects(string clipName)
		{
			if(clipName.IsEmpty())
			{
				LogChannel.Sound.E("Clip name is empty");

				return false;
			}

			bool _FindLane(SoundLane lane)
			{
				return _MatchesClipName(lane,clipName);
			}

			return TryReleaseAllActive(_FindLane);
		}

		/// <summary>Releases every active lane playing <paramref name="audioClip"/>.</summary>
		/// <returns>True when at least one matching lane was released.</returns>
		public bool TryReleaseEffects(AudioClip audioClip)
		{
			if(!audioClip)
			{
				LogChannel.Sound.E("Audio clip is null");

				return false;
			}

			bool _FindLane(SoundLane lane)
			{
				return _MatchesClip(lane,audioClip);
			}

			return TryReleaseAllActive(_FindLane);
		}

		/// <summary>Creates one pooled <see cref="AudioSource"/> under <see cref="LanePool{TLane,TSource}"/> storage and registers a <see cref="SoundLane"/>.</summary>
		protected override SoundLane _Create(int index)
		{
			var child = m_storage.AddChild($"Effect_{index}");
			var source = child.gameObject.AddComponent<AudioSource>();

			var lane = new SoundLane();

			lane.Create(source,m_storage);

			return lane;
		}

		private bool _MatchesClipName(SoundLane lane,string clipName)
		{
			var clip = lane.Payload.clip;

			return clip != null && string.Equals(clip.name,clipName);
		}

		private bool _MatchesClip(SoundLane lane,AudioClip audioClip)
		{
			var clip = lane.Payload.clip;

			return clip != null && clip.Equals(audioClip);
		}
	}
}