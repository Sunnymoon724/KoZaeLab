using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Effects
{
	/// <summary>
	/// Particle-system effect for pooled FX prefabs. Duration/loop/time scale come from <see cref="m_mainParticle"/> (see <see cref="_Reset"/>).
	/// Playback ends when the main and all listed sub emitters have stopped.
	/// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleEffectClip : EffectClip
	{
		public new record Param(Color? StartColor = null,Action<bool> OnComplete = null) : EffectClip.Param(OnComplete);

		[VerticalGroup("General/2",Order = 2),SerializeField]
		protected ParticleSystem m_mainParticle = null;
		[VerticalGroup("General/2",Order = 2),SerializeField]
		protected List<ParticleSystem> m_subParticleList = null;

		/// <summary>Applies callback via base. <see cref="Param.StartColor"/> applies only when <paramref name="effectParam"/> is <see cref="Param"/>.</summary>
		public override void Set(EffectClip.Param effectParam)
		{
			base.Set(effectParam);

			if(effectParam is not Param param)
			{
				return;
			}

			if(!param.StartColor.HasValue)
			{
				return;
			}

			if(!_EnsureMainParticle())
			{
				LogChannel.FX.W($"{gameObject.name} main ParticleSystem is missing. Start color was not applied.");

				return;
			}

			_SetColor(m_mainParticle.main,param.StartColor.Value);

			if(m_subParticleList == null)
			{
				return;
			}

			for(var i=0;i<m_subParticleList.Count;i++)
			{
				var subParticle = m_subParticleList[i];

				if(!subParticle)
				{
					continue;
				}

				_SetColor(subParticle.main,param.StartColor.Value);
			}
		}

		protected async override UniTask _ExecuteEffectAsync(CancellationToken token)
		{
			if(!_EnsureMainParticle())
			{
				LogChannel.FX.E($"{gameObject.name} main ParticleSystem is missing.");

				Complete(false);

				return;
			}

			_PrepareParticlesForPlay();

			bool _WaitForAllParticles()
			{
				return _IsAllParticlesStopped();
			}

			await KZExternalKit.WaitForConditionAsync(_WaitForAllParticles,SetTime,m_ignoreTimeScale,token).SuppressCancellationThrow();
		}

		private void _SetColor(ParticleSystem.MainModule mainModule,Color color)
		{
			mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
		}

		/// <summary>Editor/add-component hook. Syncs duration and time scale from the main module.</summary>
		protected override void _Reset()
		{
			base._Reset();

			if(!_EnsureMainParticle())
			{
				return;
			}

			var mainModule = m_mainParticle.main;

			Duration = mainModule.duration;
			// Emitter loop is not EffectClip play-cycle loop; looping emitters run until disable/ForceEnd.
			IsLoop = false;
			m_ignoreTimeScale = mainModule.useUnscaledTime;

			if(mainModule.loop)
			{
				LogChannel.FX.W($"{gameObject.name} main particle loop is enabled. Effect runs until disable/ForceEnd unless overridden by a subclass.");
			}

			mainModule.playOnAwake = true;
		}

		/// <summary>Clears stale particles from pool reuse and restarts emitters for this play session.</summary>
		protected virtual void _PrepareParticlesForPlay()
		{
			m_mainParticle.Clear(true);
			m_mainParticle.Play(true);

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

				subParticle.Clear(true);
				subParticle.Play(true);
			}
		}

		protected bool _IsAllParticlesStopped()
		{
			if(m_mainParticle.isPlaying)
			{
				return false;
			}

			if(m_subParticleList == null)
			{
				return true;
			}

			for(var i=0;i<m_subParticleList.Count;i++)
			{
				var subParticle = m_subParticleList[i];

				if(subParticle && subParticle.isPlaying)
				{
					return false;
				}
			}

			return true;
		}

		protected bool _EnsureMainParticle()
		{
			if(!m_mainParticle)
			{
				m_mainParticle = GetComponent<ParticleSystem>();
			}

			return m_mainParticle;
		}
	}
}
