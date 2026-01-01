using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectClip : EffectClip
{
	public new record Param(Color? StartColor = null,Action<bool> OnComplete = null) : EffectClip.Param(OnComplete);

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

	public override void SetEffect(EffectClip.Param effectParam)
	{
		base.SetEffect(effectParam);

		if(effectParam is not Param param)
		{
			return;
		}

		if(param.StartColor.HasValue)
		{
			_SetColor(m_mainParticle.main,param.StartColor.Value);

			for(var i=0;i<m_subParticleList.Count;i++)
			{
				_SetColor(m_subParticleList[i].main,param.StartColor.Value);
			}
		}
	}

	protected async override UniTask _ExecuteEffectAsync()
	{
		bool _WaitForParticle()
		{
			return !m_mainParticle.isPlaying;
		}

		await CommonUtility.WaitForConditionAsync(_WaitForParticle,SetTime,m_ignoreTimeScale,m_tokenSource.Token).SuppressCancellationThrow();
	}

	private void _SetColor(ParticleSystem.MainModule mainModule,Color color)
	{
		mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
	}
}