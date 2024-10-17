// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class TrailRendererEffectClip : EffectClip
// {
// 	public record TrailParam(Vector3 EndPosition,float Duration,Gradient TrailColor = null,AnimationCurve Curve = null,Action<bool> OnComplete = null) : EffectParam(Curve,OnComplete);

// 	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),LabelText("트레일 렌더러"),SerializeField]
// 	private TrailRenderer m_TrailRenderer = null;

// 	private Vector3 m_StartPosition = Vector3.zero;
// 	private Vector3 m_EndPosition = Vector3.zero;

// 	protected override void Reset()
// 	{
// 		base.Reset();

// 		if(!m_TrailRenderer)
// 		{
// 			m_TrailRenderer = GetComponent<TrailRenderer>();
// 		}

// 		if(m_Duration == 0.0f)
// 		{
// 			m_Duration = m_TrailRenderer.time;
// 		}
// 	}

// 	public override void Initialize(EffectParam _param)
// 	{
// 		base.Initialize(_param);

// 		if(_param is not TrailParam param)
// 		{
// 			return;
// 		}

// 		if(param.TrailColor != null)
// 		{
// 			m_TrailRenderer.colorGradient = param.TrailColor;
// 		}

// 		m_StartPosition = transform.position;
// 		m_EndPosition = param.EndPosition;
// 	}

// 	protected override void PlayProgress(float _progress)
// 	{
// 		base.PlayProgress(_progress);

// 		transform.position = Vector3.Lerp(m_StartPosition,m_EndPosition,_progress);
// 	}
// }