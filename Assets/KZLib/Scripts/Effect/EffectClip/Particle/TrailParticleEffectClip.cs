using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrailParticleEffectClip : ParticleEffectClip
{
	public record TrailParticleEffectParam(Color? StartColor = null,Action<bool> OnComplete = null) : ParticleEffectParam(StartColor,OnComplete);

	protected override float Duration => -1.0f;
	protected override bool IsLoop => false;

	protected override bool IsEnableDuration => false;

	protected override bool IsShowUseLoop => false;

	protected override bool IsShowIgnoreTimeScale => false;

	protected override void Reset()
	{
		base.Reset();

		if(!m_mainParticle)
		{
			return;
		}

		var mainModule = m_mainParticle.main;

		mainModule.loop = true;
	}

	protected async override UniTask PlayTaskAsync()
	{
		await CommonUtility.WaitForConditionAsync(()=>false,SetTime,m_ignoreTimeScale,m_tokenSource.Token);
	}
}