using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Effects
{
	/// <summary>
	/// Infinite <see cref="TrailRenderer"/> effect. Does not complete naturally; stop via <see cref="EffectManager.ReleaseEffect"/> or <see cref="EffectClip.ForceEnd"/>.
	/// <see cref="Param.OnComplete"/> is true only after <see cref="EffectClip.ForceEnd"/>(true). Pool release reports false (see base <see cref="EffectClip.Param.OnComplete"/>).
	/// Disable clears the trail immediately; fade-out before pool return is not implemented (see <see cref="OnDisable"/>).
	/// </summary>
	[RequireComponent(typeof(TrailRenderer))]
	public class TrailRendererEffectClip : EffectClip
	{
		public new record Param(Gradient TrailColor = null,Action<bool> OnComplete = null) : EffectClip.Param(OnComplete);

		[FoldoutGroup("General",Order = -5),SerializeField]
		private TrailRenderer m_trailRenderer = null;

		/// <summary>Negative duration: playable but never reaches a natural <see cref="EffectClip.Complete"/>(true).</summary>
		protected override float Duration => -1.0f;
		protected override bool IsLoop => false;

		protected override bool IsEnableDuration => false;

		protected override bool IsShowUseLoop => false;

		// TrailRenderer has no unscaled-time flag; base m_ignoreTimeScale stays false.
		protected override bool IsShowIgnoreTimeScale => false;

		/// <summary>Applies callback via base. <see cref="Param.TrailColor"/> applies only when <paramref name="effectParam"/> is <see cref="Param"/>.</summary>
		public override void Set(EffectClip.Param effectParam)
		{
			base.Set(effectParam);

			// EffectClip.Param only: callback is set; trail color is left unchanged (prefab default or previous play).
			if(effectParam is not Param param)
			{
				return;
			}

			if(!_EnsureTrailRenderer())
			{
				LogChannel.FX.W($"{gameObject.name} TrailRenderer is missing. Trail color was not applied.");

				return;
			}

			if(param.TrailColor != null)
			{
				m_trailRenderer.colorGradient = param.TrailColor;
			}
		}

		protected async override UniTask _ExecuteEffectAsync(CancellationToken token)
		{
			if(!_EnsureTrailRenderer())
			{
				LogChannel.FX.E($"{gameObject.name} TrailRenderer is missing.");

				Complete(false);

				return;
			}

			m_trailRenderer.Clear();
			m_trailRenderer.emitting = true;

			// Condition never becomes true: WaitForConditionAsync only drives SetTime until disable/ForceEnd cancels token.
			bool _WaitForEnd()
			{
				return false;
			}

			await KZExternalKit.WaitForConditionAsync(_WaitForEnd,SetTime,m_ignoreTimeScale,token).SuppressCancellationThrow();

			// Fade-before-end (not used): stop emission and wait for remaining trail vertices to expire before Complete/Release.
			// m_trailRenderer.emitting = false;
			// await UniTask.Delay(TimeSpan.FromSeconds(m_trailRenderer.time),cancellationToken : token).SuppressCancellationThrow();
		}

		/// <summary>
		/// Cancels playback via base, then clears trail immediately instead of fading.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if(!_EnsureTrailRenderer())
			{
				return;
			}

			m_trailRenderer.emitting = false;
			m_trailRenderer.Clear();

			// Fade-before-disable (not used): keep emitting false and defer Clear until trail.time elapses.
			// await UniTask.Delay(TimeSpan.FromSeconds(m_trailRenderer.time),cancellationToken : destroyCancellationToken);
			// m_trailRenderer.Clear();
		}

		protected override void _Reset()
		{
			base._Reset();

			_EnsureTrailRenderer();
		}

		private bool _EnsureTrailRenderer()
		{
			if(!m_trailRenderer)
			{
				m_trailRenderer = GetComponent<TrailRenderer>();
			}

			return m_trailRenderer;
		}
	}
}
