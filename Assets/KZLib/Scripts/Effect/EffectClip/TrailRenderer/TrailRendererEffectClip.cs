using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererEffectClip : EffectClip
{
	public record TrailRendererEffectParam(Gradient TrailColor = null,Action<bool> OnComplete = null) : EffectParam(OnComplete);

	[FoldoutGroup("General",Order = -5),LabelText("Trail Renderer"),SerializeField]
	private TrailRenderer m_trailRenderer = null;

	protected override float Duration => -1.0f;
	protected override bool IsLoop => false;

	protected override bool IsEnableDuration => false;

	protected override bool IsShowUseLoop => false;

	protected override bool IsShowIgnoreTimeScale => false;

	protected override void Reset()
	{
		base.Reset();

		if(!m_trailRenderer)
		{
			m_trailRenderer = GetComponent<TrailRenderer>();
		}
	}

	public override void SetEffect(EffectParam effectParam)
	{
		base.SetEffect(effectParam);

		if(effectParam is not TrailRendererEffectParam param)
		{
			return;
		}

		if(param.TrailColor != null)
		{
			m_trailRenderer.colorGradient = param.TrailColor;
		}
	}

	protected async override UniTask PlayTaskAsync()
	{
		await CommonUtility.WaitForConditionAsync(()=>false,SetTime,m_ignoreTimeScale,m_tokenSource.Token);
	}
}