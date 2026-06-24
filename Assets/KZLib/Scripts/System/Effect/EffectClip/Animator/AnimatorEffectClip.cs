using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using System.Threading;

namespace KZLib.Effects
{
	/// <summary>
	/// Animator state playback for pooled effects. <see cref="Param.Name"/> is an animator <b>state</b> name (not necessarily a clip name).
	/// Duration resolves from clip name first, then editor state motion length, then runtime <see cref="AnimatorStateInfo.length"/> after play starts.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimatorEffectClip : EffectClip
	{
		private const float c_animationStartTimeout = 0.5f;

		[SerializeField,HideInInspector]
		private string m_animationName = null;

		public new record Param(string Name,Action<bool> OnComplete = null) : EffectClip.Param(OnComplete);

		protected override bool IsEnableDuration => false;

		[VerticalGroup("General/2",Order = 2),SerializeField]
		private Animator m_animator = null;

		[VerticalGroup("General/3",Order = 3),ShowInInspector,ValueDropdown(nameof(StateNameGroup))]
		private string AnimationName
		{
			get => m_animationName;
			set
			{
				if(value.IsEmpty())
				{
					m_animationName = null;
					Duration = 0.0f;

					return;
				}

				_ApplyAnimationName(value);
			}
		}

		/// <summary>Applies callback via base. <see cref="Param.Name"/> applies only when <paramref name="effectParam"/> is <see cref="Param"/>.</summary>
		public override void Set(EffectClip.Param effectParam)
		{
			base.Set(effectParam);

			// EffectClip.Param only: callback is set; animation name is left unchanged (prefab default or previous play).
			if(effectParam is not Param param)
			{
				return;
			}

			AnimationName = param.Name;
		}

		protected async override UniTask _ExecuteEffectAsync(CancellationToken token)
		{
			if(!_EnsureAnimator())
			{
				LogChannel.FX.E($"{gameObject.name} Animator is missing.");

				Complete(false);

				return;
			}

			if(AnimationName.IsEmpty())
			{
				LogChannel.FX.E($"{gameObject.name} animation name is empty.");

				Complete(false);

				return;
			}

			m_animator.Play(AnimationName);

			await UniTask.Yield(token);

			if(!await _WaitForAnimationStartAsync(token))
			{
				LogChannel.FX.E($"{gameObject.name} animation [{AnimationName}] did not start.");

				Complete(false);

				return;
			}

			_SyncDurationFromState();

			bool _WaitForAnimation()
			{
				return m_animator.IsAnimationFinished(AnimationName);
			}

			await KZExternalKit.WaitForConditionAsync(_WaitForAnimation,SetTime,m_ignoreTimeScale,token).SuppressCancellationThrow();
		}

		protected override void _Reset()
		{
			base._Reset();

			if(!_EnsureAnimator())
			{
				return;
			}

			AnimationName = null;

			m_ignoreTimeScale = m_animator.updateMode == AnimatorUpdateMode.UnscaledTime;
		}

#if UNITY_EDITOR
		private readonly List<string> m_stateNameList = new();

		private IEnumerable<string> StateNameGroup
		{
			get
			{
				if(m_animator && m_stateNameList.IsNullOrEmpty())
				{
					m_stateNameList.AddRange(m_animator.FindStateNameGroup());
				}

				return m_stateNameList;
			}
		}
#endif

		private void _ApplyAnimationName(string stateName)
		{
			if(!_EnsureAnimator())
			{
				m_animationName = stateName;
				Duration = 0.0f;

				return;
			}

			m_animationName = stateName;

			var duration = _ResolveDuration(stateName);

			if(duration <= 0.0f)
			{
				LogChannel.FX.W($"{gameObject.name} animation [{stateName}] length was not found. Duration syncs after play starts when possible.");

				if(Duration <= 0.0f)
				{
					Duration = 0.0f;
				}

				return;
			}

			Duration = duration;
		}

		private float _ResolveDuration(string stateName)
		{
			var duration = m_animator.FindAnimationClipLength(stateName);

			if(duration > 0.0f)
			{
				return duration;
			}

#if UNITY_EDITOR
			duration = m_animator.FindAnimatorStateMotionLength(stateName);

			if(duration > 0.0f)
			{
				return duration;
			}
#endif

			return -1.0f;
		}

		private void _SyncDurationFromState()
		{
			var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

			if(stateInfo.shortNameHash != Animator.StringToHash(AnimationName))
			{
				return;
			}

			if(stateInfo.length > 0.0f)
			{
				Duration = stateInfo.length;
			}
		}

		private async UniTask<bool> _WaitForAnimationStartAsync(CancellationToken token)
		{
			if(m_animator.HasAnimationStarted(AnimationName))
			{
				return true;
			}

			var elapsed = 0.0f;

			while(elapsed < c_animationStartTimeout)
			{
				if(token.IsCancellationRequested)
				{
					return false;
				}

				if(m_animator.HasAnimationStarted(AnimationName))
				{
					return true;
				}

				await UniTask.Yield(token);

				elapsed += m_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
			}

			return m_animator.HasAnimationStarted(AnimationName);
		}

		private bool _EnsureAnimator()
		{
			if(!m_animator)
			{
				m_animator = GetComponent<Animator>();
			}

			return m_animator;
		}
	}
}
