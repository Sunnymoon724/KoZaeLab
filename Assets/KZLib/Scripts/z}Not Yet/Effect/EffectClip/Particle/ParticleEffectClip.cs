// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class ParticleEffectClip : EffectClip
// {
// 	public record ParticleParam(Color? StartColor = null,AnimationCurve Curve = null,Action<bool> OnComplete = null) : EffectParam(Curve,OnComplete);

// 	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),LabelText("메인 파티클"),SerializeField]
// 	protected ParticleSystem m_MainParticle = null;
// 	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),LabelText("서브 파티클들"),SerializeField]
// 	protected List<ParticleSystem> m_SubParticleList = null;

// 	protected override void Reset()
// 	{
// 		base.Reset();

// 		if(!m_MainParticle)
// 		{
// 			m_MainParticle = GetComponent<ParticleSystem>();
// 		}

// 		m_Duration = m_MainParticle.main.duration;
// 	}

// 	public override void Initialize(EffectParam _param)
// 	{
// 		base.Initialize(_param);

// 		if(_param is not ParticleParam param)
// 		{
// 			return;
// 		}

// 		if(param.StartColor.HasValue)
// 		{
// 			SetColor(m_MainParticle.main,param.StartColor.Value);

// 			foreach(var particle in m_SubParticleList)
// 			{
// 				SetColor(particle.main,param.StartColor.Value);
// 			}
// 		}
// 	}

// 	private void SetColor(ParticleSystem.MainModule _module,Color _color)
// 	{
// 		_module.startColor = new ParticleSystem.MinMaxGradient(_color);
// 	}
// }