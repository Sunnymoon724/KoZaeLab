﻿using KZLib;
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
	private float m_currentTime = 0.0f;

	[SerializeField,HideInInspector]
	private float m_duration = 0.0f;
	[SerializeField,HideInInspector]
	private bool m_isLoop = false;

	[VerticalGroup("Time",Order = 0),ShowInInspector,KZRichText]
	public string CurrentTime => $"{m_currentTime:F3}s";

	[FoldoutGroup("General",Order = 1)]
	[VerticalGroup("General/0",Order = 0),ShowInInspector,EnableIf(nameof(IsEnableDuration))]
	protected virtual float Duration { get => m_duration; set => m_duration = value; }

	protected virtual bool IsEnableDuration => true;

	[VerticalGroup("General/0",Order = 0),ShowInInspector,ShowIf(nameof(IsShowUseLoop))]
	protected virtual bool IsLoop { get => m_isLoop; set => m_isLoop = value; }

	protected virtual bool IsShowUseLoop => true;

	[VerticalGroup("General/1",Order = 1),SerializeField,ShowIf(nameof(IsShowIgnoreTimeScale))]
	protected bool m_ignoreTimeScale = false;

	protected virtual bool IsShowIgnoreTimeScale => true;

	private bool IsPlayable => m_duration != 0.0f;

	public float Progress => IsPlayable && Duration > 0.0f ? m_currentTime/Duration : 0.0f;

	private Action<bool> m_onComplete = null;

	protected CancellationTokenSource m_tokenSource = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		PlayEffectAsync().Forget();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	public virtual void SetEffect(EffectParam effectParam)
	{
		if(effectParam != null)
		{
			m_onComplete = effectParam.OnComplete;
		}
	}

	protected async UniTask PlayEffectAsync()
	{
		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		m_currentTime = 0.0f;

		if(!IsPlayable)
		{
			LogSvc.FX.W($"{gameObject.name} duration is 0.");

			EndEffect(false);

			return;
		}

		var count = IsLoop ? -1 : 1;

		await CommonUtility.LoopUniTaskAsync(async ()=>
		{
			m_currentTime = 0.0f;

			await PlayTaskAsync();

			m_currentTime = Duration;
		},count,m_tokenSource.Token);

		EndEffect(true);

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	protected abstract UniTask PlayTaskAsync();

	protected virtual void SetTime(float time)
	{
		m_currentTime = time;
	}

	protected virtual void EndEffect(bool showResult)
	{
		m_onComplete?.Invoke(showResult);

		// fail -> destroy
		if(!showResult)
		{
			gameObject.DestroyObject();

			return;
		}

		if(EffectMgr.HasInstance)
		{
			EffectMgr.In.ReleaseEffect(this);
		}
	}

	public void ForceEndEffect(bool isDestroy = false)
	{
		EndEffect(!isDestroy);
	}
}