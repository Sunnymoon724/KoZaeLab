using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KZLib.Effects
{
	/// <summary>
	/// Infinite looping <see cref="ParticleSystem"/> trail. Does not complete naturally; stop via <see cref="EffectManager.ReleaseEffect"/> or <see cref="EffectClip.ForceEnd"/>.
	/// <see cref="Param.OnComplete"/> is true only after <see cref="EffectClip.ForceEnd"/>(true). Pool release reports false (see base <see cref="EffectClip.Param.OnComplete"/>).
	/// Disable stops emitters and clears particles immediately; fade-out before pool return is not implemented (see <see cref="OnDisable"/>).
	/// </summary>
	public class TrailParticleEffectClip : ParticleEffectClip
	{
		public new record Param(Color? StartColor = null,Action<bool> OnComplete = null) : ParticleEffectClip.Param(StartColor,OnComplete);

		/// <summary>Negative duration: playable but never reaches a natural <see cref="EffectClip.Complete"/>(true). Overrides serialized <c>m_duration</c> from <see cref="ParticleEffectClip._Reset"/>.</summary>
		protected override float Duration => -1.0f;

		// Base IsLoop follows main module; trail uses infinite wait instead of EffectClip loop cycles.
		protected override bool IsLoop => false;

		protected override bool IsEnableDuration => false;

		protected override bool IsShowUseLoop => false;

		// Inspector hidden; m_ignoreTimeScale is still synced from the main module in base._Reset.
		protected override bool IsShowIgnoreTimeScale => false;

		protected async override UniTask _ExecuteEffectAsync(CancellationToken token)
		{
			if(!_EnsureMainParticle())
			{
				LogChannel.FX.E($"{gameObject.name} main ParticleSystem is missing.");

				Complete(false);

				return;
			}

			// Child systems under the main hierarchy are cleared/played via PrepareParticlesForPlay(true) on the main emitter.
			_PrepareParticlesForPlay();

			// Condition never becomes true: WaitForConditionAsync only drives SetTime until disable/ForceEnd cancels token.
			bool _WaitForEnd()
			{
				return false;
			}

			await KZExternalKit.WaitForConditionAsync(_WaitForEnd,SetTime,m_ignoreTimeScale,token).SuppressCancellationThrow();

			// Fade-before-end (not used): stop emission and wait for remaining particles to die before Complete/Release.
			// m_mainParticle.Stop(true,ParticleSystemStopBehavior.StopEmitting);
			// await UniTask.Delay(TimeSpan.FromSeconds(m_mainParticle.main.duration),cancellationToken : token).SuppressCancellationThrow();
		}

		/// <summary>
		/// Cancels playback via base, then stops and clears particles immediately instead of fading.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if(!_EnsureMainParticle())
			{
				return;
			}

			m_mainParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);

			// Listed sub emitters that are not children of the main system are stopped separately.
			if(m_subParticleList == null)
			{
				return;
			}

			for(var i=0;i<m_subParticleList.Count;i++)
			{
				var subParticle = m_subParticleList[i];

				if(!subParticle || subParticle == m_mainParticle)
				{
					continue;
				}

				subParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
			}

			// Fade-before-disable (not used): StopEmitting only, delay, then Clear.
			// m_mainParticle.Stop(true,ParticleSystemStopBehavior.StopEmitting);
			// await UniTask.Delay(TimeSpan.FromSeconds(m_mainParticle.main.duration),cancellationToken : destroyCancellationToken);
			// m_mainParticle.Clear(true);
		}

		protected override void _Reset()
		{
			// Base syncs m_duration, m_ignoreTimeScale, and playOnAwake; Duration getter (-1) overrides playback length rules.
			base._Reset();

			if(!_EnsureMainParticle())
			{
				return;
			}

			// Trail must loop on the emitter; EffectClip loop flag stays false (see IsLoop).
			var mainModule = m_mainParticle.main;

			mainModule.loop = true;
		}
	}
}