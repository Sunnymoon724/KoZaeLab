using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// <see cref="Stanza"/> that animates a normalized <see cref="Progress"/> from 0 to 1 (or reverse) over <see cref="Duration"/>.
	/// Subclasses implement <see cref="_SetProgress"/> to apply the value to targets.
	/// </summary>
	public abstract class StanzaLerp : Stanza
	{
		/// <summary>Optional duration override and reverse direction for a single play.</summary>
		public new record Param(float? Duration = null,bool IsReverse = false) : Stanza.Param;

		[SerializeField,HideInInspector]
		private float m_duration = 0.0f;

		[SerializeField,HideInInspector]
		private float m_progress = 0.0f;

		/// <summary>When true, <see cref="Duration"/> is driven externally (e.g. fade mode) and not editable.</summary>
		protected virtual bool DurationLock => false;

		[VerticalGroup("General/0",Order = 0),ShowInInspector,DisableIf(nameof(DurationLock))]
		public float Duration { get => m_duration; protected set => m_duration = value; }

		/// <summary>True when <see cref="Duration"/> is greater than zero.</summary>
		public bool IsPlayable => m_duration > 0.0f;

		/// <summary>Normalized animation value in [0, 1]; updates targets via <see cref="_SetProgress"/>.</summary>
		[VerticalGroup("Progress",Order = 99),ShowInInspector,PropertyRange(0.0f,1.0f)]
		public virtual float Progress
		{
			get => m_progress;
			set
			{
			m_progress = Mathf.Clamp01(value);

			_SetProgress(m_progress);
			}
		}

		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected bool m_IgnoreTimeScale = false;

		private readonly Subject<float> m_changeSubject = new();
		/// <summary>Emits each progress step during playback (before <see cref="Progress"/> setter side effects).</summary>
		public Observable<float> OnChanged => m_changeSubject;

		/// <summary>Applies <paramref name="progress"/> to the lerped target(s).</summary>
		protected abstract void _SetProgress(float progress);

		public override void ResetAll()
		{
			base.ResetAll();

			Progress = 0.0f;
		}

		protected async override UniTask _DoPlayAsync(Stanza.Param param)
		{
			var progressParam = (param as Param) ?? new Param();
			var duration = progressParam.Duration ?? Duration;

			if(duration <= 0.0f)
			{
				LogChannel.Develop.E("Duration must be greater than zero");

				return;
			}

			var start = progressParam.IsReverse ? 1.0f : 0.0f;
			var finish = progressParam.IsReverse ? 0.0f : 1.0f;

			void _Lerp(float progress)
			{
				m_changeSubject.OnNext(progress);
				Progress = progress;
			}

			await KZExternalKit.ExecuteProgressAsync(start,finish,duration,_Lerp,m_IgnoreTimeScale,null,m_tokenSource.Token).SuppressCancellationThrow();
		}
	}
}
