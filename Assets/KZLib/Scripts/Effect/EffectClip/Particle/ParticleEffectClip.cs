using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectClip : EffectClip
{
	public record ParticleEffectParam(Color? StartColor = null,Action<bool> OnComplete = null) : EffectParam(OnComplete);

	[VerticalGroup("General/2",Order = 2),SerializeField]
	protected ParticleSystem m_mainParticle = null;
	[VerticalGroup("General/2",Order = 2),SerializeField]
	protected List<ParticleSystem> m_subParticleList = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_mainParticle)
		{
			m_mainParticle = GetComponent<ParticleSystem>();
		}

		if(!m_mainParticle)
		{
			return;
		}

		var mainModule = m_mainParticle.main;

		Duration = mainModule.duration;
		IsLoop = mainModule.loop;
		m_ignoreTimeScale = mainModule.useUnscaledTime;

		mainModule.playOnAwake = true;
	}

	public override void SetEffect(EffectParam effectParam)
	{
		base.SetEffect(effectParam);

		if(effectParam is not ParticleEffectParam param)
		{
			return;
		}

		if(param.StartColor.HasValue)
		{
			_SetColor(m_mainParticle.main,param.StartColor.Value);

			foreach(var particle in m_subParticleList)
			{
				_SetColor(particle.main,param.StartColor.Value);
			}
		}
	}

	protected async override UniTask PlayTaskAsync()
	{
		await CommonUtility.WaitForConditionAsync(()=>!m_mainParticle.isPlaying,SetTime,m_ignoreTimeScale,m_tokenSource.Token);
	}

	private void _SetColor(ParticleSystem.MainModule mainModule,Color color)
	{
		mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
	}
}