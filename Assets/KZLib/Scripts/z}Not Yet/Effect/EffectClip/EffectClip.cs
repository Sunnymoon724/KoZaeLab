using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using KZLib.KZAttribute;
using Cysharp.Threading.Tasks;

public abstract class EffectClip : BaseComponentTask
{
	public record EffectParam(AnimationCurve Curve = null,Action<bool> OnComplete = null);

	[SerializeField,HideInInspector]
	protected float m_CurrentTime = 0.0f;

	[VerticalGroup("이펙트 시간",Order = -10),LabelText("현재 시간"),ShowInInspector,KZRichText]
	public string CurrentTime => string.Format("{0} 초",m_CurrentTime);

	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),LabelText("실행 시간"),SerializeField,ValidateInput(nameof(IsPlayable),"지속시간이 0으로 설정되어 있습니다.",InfoMessageType.Error)]
	protected float m_Duration = 0.0f;
	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),LabelText("반복 사용"),SerializeField]
	protected bool m_UseLoop = false;

	[BoxGroup("이펙트 설정",ShowLabel = false,Order = -5),SerializeField,LabelText("타임스케일 무시")]
	protected bool m_IgnoreTimeScale = false;

	private bool IsPlayable => m_Duration > 0.0f;

	public float Duration => m_Duration;
	public float Progress => IsPlayable ? m_CurrentTime/m_Duration : 0.0f;

	private Action<bool> m_OnComplete = null;

	private AnimationCurve m_AnimationCurve = null;

	protected virtual void OnEnable()
	{
		PlayEffectAsync().Forget();
	}

	public virtual void Initialize(EffectParam _param)
	{
		if(_param != null)
		{
			m_OnComplete = _param.OnComplete;
			m_AnimationCurve = _param.Curve;
		}
	}

	protected async virtual UniTask PlayEffectAsync()
	{
		InitializeTokenSource();

		m_CurrentTime = 0.0f;

		if(!IsPlayable)
		{
			LogTag.Effect.W("{0}의 지속시간이 0으로 설정되어 있습니다.",gameObject.name);

			EndEffect(false);

			return;
		}

		var count = m_UseLoop ? -1 : 1;

		await CommonUtility.LoopUniTaskAsync(async ()=>
		{
			m_CurrentTime = 0.0f;

			await PlayTaskAsync();

			m_CurrentTime = Duration;
		},count,m_Source.Token);

		EndEffect(true);

		ReleaseTokenSource();
	}

	protected async virtual UniTask PlayTaskAsync()
	{
		await CommonUtility.ExecuteOverTimeAsync(0.0f,Duration,Duration,(time)=>
		{
			m_CurrentTime = time;

			PlayProgress(time/Duration);
		},m_IgnoreTimeScale,m_AnimationCurve,m_Source.Token);
	}

	protected virtual void PlayProgress(float _progress) { }

	protected virtual void EndEffect(bool _result)
	{
		m_OnComplete?.Invoke(_result);

		// 실패시 삭제
		if(!_result)
		{
			CommonUtility.DestroyObject(gameObject);

			return;
		}

		if(EffectMgr.HasInstance)
		{
			EffectMgr.In.ReleaseEffect(this);
		}
	}

	public void ForceEndEffect(bool _destroy = false)
	{
		EndEffect(!_destroy);
	}
}