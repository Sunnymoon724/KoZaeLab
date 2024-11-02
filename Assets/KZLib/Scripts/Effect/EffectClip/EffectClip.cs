using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using KZLib.KZAttribute;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class EffectClip : BaseComponent
{
	public record EffectParam(Action<bool> OnComplete = null);

	[SerializeField,HideInInspector]
	private float m_CurrentTime = 0.0f;

	[SerializeField,HideInInspector]
	private float m_Duration = 0.0f;
	[SerializeField,HideInInspector]
	private bool m_IsLoop = false;

	[VerticalGroup("Time",Order = 0),ShowInInspector,KZRichText]
	public string CurrentTime => $"{m_CurrentTime:F3}s";

	[FoldoutGroup("General",Order = 1)]
	[VerticalGroup("General/0",Order = 0),ShowInInspector,EnableIf(nameof(IsEnableDuration))]
	protected virtual float Duration { get => m_Duration; set => m_Duration = value; }

	protected virtual bool IsEnableDuration => true;

	[VerticalGroup("General/0",Order = 0),ShowInInspector,ShowIf(nameof(IsShowUseLoop))]
	protected virtual bool IsLoop { get => m_IsLoop; set => m_IsLoop = value; }

	protected virtual bool IsShowUseLoop => true;

	[VerticalGroup("General/1",Order = 1),SerializeField,ShowIf(nameof(IsShowIgnoreTimeScale))]
	protected bool m_IgnoreTimeScale = false;

	protected virtual bool IsShowIgnoreTimeScale => true;

	private bool IsPlayable => m_Duration != 0.0f;

	public float Progress => IsPlayable && Duration > 0.0f ? m_CurrentTime/Duration : 0.0f;

	private Action<bool> m_OnComplete = null;

    protected CancellationTokenSource m_TokenSource = null;

	protected override void OnEnable()
	{
        base.OnEnable();

		PlayEffectAsync().Forget();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

	public virtual void SetEffect(EffectParam _param)
	{
		if(_param != null)
		{
			m_OnComplete = _param.OnComplete;
		}
	}

	protected async UniTask PlayEffectAsync()
	{
		UniTaskUtility.RecycleTokenSource(ref m_TokenSource);

		m_CurrentTime = 0.0f;

		if(!IsPlayable)
		{
			LogTag.FX.W($"{gameObject.name} duration is 0.");

			EndEffect(false);

			return;
		}

		var count = IsLoop ? -1 : 1;

		await UniTaskUtility.LoopUniTaskAsync(async ()=>
		{
			m_CurrentTime = 0.0f;

			await PlayTaskAsync();

			m_CurrentTime = Duration;
		},count,m_TokenSource.Token);

		EndEffect(true);

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

	protected abstract UniTask PlayTaskAsync();

	protected void SetTime(float _time)
	{
		m_CurrentTime = _time;

		PlayProgress(_time/Duration);
	}

	protected virtual void PlayProgress(float _progress) { }

	protected virtual void EndEffect(bool _result)
	{
		m_OnComplete?.Invoke(_result);

		// fail -> destroy
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