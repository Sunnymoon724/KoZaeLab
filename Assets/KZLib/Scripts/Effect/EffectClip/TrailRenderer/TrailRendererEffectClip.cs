using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererEffectClip : EffectClip
{
	public record TrailRendererEffectParam(Gradient TrailColor = null,Action<bool> OnComplete = null) : EffectParam(OnComplete);

	[FoldoutGroup("General",Order = -5),LabelText("Trail Renderer"),SerializeField]
	private TrailRenderer m_TrailRenderer = null;

	protected override float Duration => -1.0f;
	protected override bool IsLoop => false;

	protected override bool IsEnableDuration => false;

	protected override bool IsShowUseLoop => false;

	protected override bool IsShowIgnoreTimeScale => false;

	protected override void Reset()
	{
		base.Reset();

		if(!m_TrailRenderer)
		{
			m_TrailRenderer = GetComponent<TrailRenderer>();
		}
	}

	public override void SetEffect(EffectParam _param)
	{
		base.SetEffect(_param);

		if(_param is not TrailRendererEffectParam param)
		{
			return;
		}

		if(param.TrailColor != null)
		{
			m_TrailRenderer.colorGradient = param.TrailColor;
		}
	}

	protected async override UniTask PlayTaskAsync()
	{
		await UniTaskUtility.WaitForConditionAsync(()=>false,SetTime,m_IgnoreTimeScale,m_TokenSource.Token);
	}
}