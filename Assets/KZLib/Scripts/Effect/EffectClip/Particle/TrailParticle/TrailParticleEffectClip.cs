using System;
using UnityEngine;

public class TrailParticleEffectClip : ParticleEffectClip
{
	public record TrailParam(Vector3 EndPosition,float Duration,Color? StartColor = null,AnimationCurve Curve = null,Action<bool> OnComplete = null) : ParticleParam(StartColor,Curve,OnComplete);

	private Vector3 m_StartPosition = Vector3.zero;
	private Vector3 m_EndPosition = Vector3.zero;

	public override void Initialize(EffectParam _param)
	{
		base.Initialize(_param);

		if(_param is not TrailParam param)
		{
			return;
		}

		m_Duration = param.Duration;
		m_StartPosition = transform.position;
		m_EndPosition = param.EndPosition;
	}

	protected override void PlayProgress(float _progress)
	{
		base.PlayProgress(_progress);

		transform.position = Vector3.Lerp(m_StartPosition,m_EndPosition,_progress);
	}
}