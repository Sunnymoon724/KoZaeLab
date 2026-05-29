using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	public abstract class StanzaLerp : Stanza
	{
		public new record Param(float? Duration = null,bool IsReverse = false) : Stanza.Param;

		[SerializeField,HideInInspector]
		private float m_duration = 0.0f;

		[SerializeField,HideInInspector]
		private float m_progress = 0.0f;

		protected virtual bool DurationLock => false;

		[VerticalGroup("General/0",Order = 0),ShowInInspector,DisableIf(nameof(DurationLock))]
		public float Duration { get => m_duration; protected set => m_duration = value; }

		public bool IsPlayable => m_duration > 0.0f;

		[VerticalGroup("Progress",Order = 99),ShowInInspector,PropertyRange(0.0f,1.0f)]
		public virtual float Progress
		{
			get => m_progress;
			set
			{
				m_progress = Mathf.Clamp01(value);

				_SetProgress(value);
			}
		}

		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected bool m_IgnoreTimeScale = false;

		private readonly Subject<float> m_changeSubject = new();
		public Observable<float> OnChanged => m_changeSubject;

		protected abstract void _SetProgress(float progress);

		public override void ResetAll()
		{
			base.ResetAll();

			Progress = 0.0f;
		}

		protected async override UniTask _DoPlayAsync(Stanza.Param param)
		{
			var progressParam = param == null ? new Param() : param as Param;
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