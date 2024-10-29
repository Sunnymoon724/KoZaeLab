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
	protected ParticleSystem m_MainParticle = null;
	[VerticalGroup("General/2",Order = 2),SerializeField]
	protected List<ParticleSystem> m_SubParticleList = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_MainParticle)
		{
			m_MainParticle = GetComponent<ParticleSystem>();
		}

		if(!m_MainParticle)
		{
			return;
		}

		var mainModule = m_MainParticle.main;

		Duration = mainModule.duration;
		IsLoop = mainModule.loop;
		m_IgnoreTimeScale = mainModule.useUnscaledTime;

		mainModule.playOnAwake = true;
	}

	public override void SetEffect(EffectParam _param)
	{
		base.SetEffect(_param);

		if(_param is not ParticleEffectParam param)
		{
			return;
		}

		if(param.StartColor.HasValue)
		{
			SetColor(m_MainParticle.main,param.StartColor.Value);

			foreach(var particle in m_SubParticleList)
			{
				SetColor(particle.main,param.StartColor.Value);
			}
		}
	}

	protected async override UniTask PlayTaskAsync()
	{
		await UniTaskUtility.WaitForConditionAsync(()=>!m_MainParticle.isPlaying,SetTime,m_IgnoreTimeScale,m_TokenSource.Token);
	}

	private void SetColor(ParticleSystem.MainModule _module,Color _color)
	{
		_module.startColor = new ParticleSystem.MinMaxGradient(_color);
	}
}