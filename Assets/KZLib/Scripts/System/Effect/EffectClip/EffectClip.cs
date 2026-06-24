using Sirenix.OdinInspector;
using UnityEngine;
using System;
using KZLib.Attributes;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace KZLib.Effects
{
	/// <summary>
	/// Pooled visual-effect playback base. <see cref="EffectManager"/> calls <see cref="Set"/> then enables the instance;
	/// <see cref="OnEnable"/> starts <see cref="_ExecuteEffectAsync"/> and handles loop, completion, and cancellation.
	/// </summary>
	public abstract class EffectClip : MonoBehaviour
	{
		/// <summary>Runtime parameters passed from <see cref="EffectManager.PlayEffect"/>.</summary>
		/// <param name="OnComplete">
		/// true: playback finished naturally. false: misconfiguration, external cancellation (disable/stop), or <see cref="ForceEnd"/> with false.
		/// Cancellation via <see cref="EffectManager"/> does not destroy the instance.
		/// </param>
		public record Param(Action<bool> OnComplete = null);

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
		/// <summary>Effect length in seconds. Subclasses may override the getter (e.g. infinite = -1).</summary>
		protected virtual float Duration { get => m_duration; set => m_duration = value; }

		protected virtual bool IsEnableDuration => true;

		[VerticalGroup("General/0",Order = 0),ShowInInspector,ShowIf(nameof(IsShowUseLoop))]
		protected virtual bool IsLoop { get => m_isLoop; set => m_isLoop = value; }

		protected virtual bool IsShowUseLoop => true;

		[VerticalGroup("General/1",Order = 1),SerializeField,ShowIf(nameof(IsShowIgnoreTimeScale))]
		/// <summary>Used by subclasses when waiting/updating time. Not read by this base class.</summary>
		protected bool m_ignoreTimeScale = false;

		protected virtual bool IsShowIgnoreTimeScale => true;

		/// <summary>Duration 0 = misconfigured. Negative = infinite (manual stop via disable).</summary>
		protected bool IsInfinite => Duration < 0.0f;

		private bool IsPlayable => Duration != 0.0f;

		/// <summary>Normalized playback progress in [0, 1]. Always 0 for infinite duration.</summary>
		public float Progress => !IsInfinite && Duration > 0.0f ? m_currentTime/Duration : 0.0f;

		private Action<bool> m_onComplete = null;
		private bool m_isCompleted = false;

		private CancellationTokenSource m_tokenSource = null;

		/// <summary>Starts playback. Expect <see cref="Set"/> to have run before enable when spawned via <see cref="EffectManager"/>.</summary>
		private void OnEnable()
		{
			m_isCompleted = false;

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			_PlayEffectAsync(m_tokenSource.Token).Forget();
		}

		/// <summary>Cancels in-flight playback. Completion is reported as failure unless <see cref="Complete"/> already ran.</summary>
		protected virtual void OnDisable()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		/// <summary>Applies runtime parameters. Must be called before enable for correct <paramref name="effectParam"/> callback wiring.</summary>
		public virtual void Set(Param effectParam)
		{
			m_onComplete = effectParam?.OnComplete;
		}

		/// <summary>Validates duration, runs one or more play cycles, then completes or reports cancellation.</summary>
		protected async UniTask _PlayEffectAsync(CancellationToken token)
		{
			m_currentTime = 0.0f;

			if(!IsPlayable)
			{
				// Duration 0 is a prefab/setup error: log, invoke failure callback, and destroy (not pool return).
				LogChannel.FX.E($"{gameObject.name} duration is 0.");

				Complete(false);

				return;
			}

			var count = IsLoop ? -1 : 1;

			async UniTask _PlayTaskAsync()
			{
				m_currentTime = 0.0f;

				await _ExecuteEffectAsync(token);

				if(Duration > 0.0f)
				{
					m_currentTime = Duration;
				}
			}

			await KZExternalKit.LoopUniTaskAsync(_PlayTaskAsync,count,token).SuppressCancellationThrow();

			if(m_isCompleted)
			{
				return;
			}

			// Disable/stop path: callback only, no pool release or destroy.
			if(token.IsCancellationRequested)
			{
				_InvokeCallback(false);

				return;
			}

			Complete(true);
		}

		/// <summary>Type-specific playback. Yield until the effect finishes or <paramref name="token"/> is cancelled.</summary>
		protected abstract UniTask _ExecuteEffectAsync(CancellationToken token);

		/// <summary>Updates elapsed time for inspector display and <see cref="Progress"/>.</summary>
		protected virtual void SetTime(float time)
		{
			m_currentTime = time;
		}

		/// <summary>Invokes <see cref="Param.OnComplete"/> once and clears the stored callback.</summary>
		private void _InvokeCallback(bool isSuccess)
		{
			if(m_isCompleted)
			{
				return;
			}

			m_isCompleted = true;

			var onComplete = m_onComplete;

			m_onComplete = null;

			onComplete?.Invoke(isSuccess);
		}

		/// <summary>
		/// Ends the current play session: invokes callback, then destroys on failure or returns to <see cref="EffectManager"/> on success.
		/// </summary>
		protected virtual void Complete(bool isSuccess)
		{
			if(m_isCompleted)
			{
				return;
			}

			_InvokeCallback(isSuccess);

			if(!isSuccess)
			{
				if(EffectManager.HasInstance)
				{
					EffectManager.In.UnregisterPlayingEffect(this);
				}

				// Misconfiguration or explicit failure: destroy instead of returning to the pool.
				gameObject.DestroyObject();

				return;
			}

			if(EffectManager.HasInstance)
			{
				EffectManager.In.ReleaseEffect(this);
			}
			else
			{
				LogChannel.FX.W($"{gameObject.name} EffectManager is not available. Disabling effect manually.");

				gameObject.EnsureActive(false);
			}
		}

		/// <summary>Immediately ends playback. <see cref="OnDisable"/> from release/destroy cancels the active token.</summary>
		public void ForceEnd(bool isSuccess = true)
		{
			Complete(isSuccess);
		}

		private void Reset()
		{
			_Reset();
		}

		/// <summary>Editor/add-component hook. Subclasses sync duration, loop, and component refs from the prefab.</summary>
		protected virtual void _Reset() { }
	}
}
