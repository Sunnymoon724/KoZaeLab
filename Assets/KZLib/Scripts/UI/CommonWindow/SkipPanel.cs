using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib.Sounds;

namespace KZLib.UI
{
	/// <summary>
	/// Delayed skip UI for skippable playback (video, cut scene).
	/// Show duration &lt; 0 keeps the skip button visible; otherwise it auto-hides and is re-shown via the trigger button.
	/// </summary>
	public class SkipPanel : BasePanel
	{
		/// <summary>Minimum auto-hide duration; values below this break the show/hide timer UX.</summary>
		private const float MinHideDuration = 0.02f;

		/// <summary>Invoked when the skip button is pressed; typically <see cref="VideoPanel.Stop"/> or cut-scene stop.</summary>
		public record Param(Action OnClicked);

		[SerializeField,ValidateInput(nameof(IsValidShowDuration),"0 is not defined.",InfoMessageType.Error),PropertyTooltip("Negative numbers represent infinity, and zero doesn't work.")]
		private float m_skipShowDuration = 0.0f;
		[SerializeField,MinValue(MinHideDuration)]
		private float m_skipHideDuration = 0.0f;

		[SerializeField]
		private AudioClip m_clickSoundClip = null;

		[SerializeField]
		private Button m_triggerButton = null;

		[SerializeField]
		private Button m_skipButton = null;

		private bool IsValidShowDuration => m_skipShowDuration != 0.0f;

		/// <summary>
		/// <c>true</c> when skip UI can be shown; <c>false</c> when <see cref="Open"/> would <see cref="_FailOpen"/>.
		/// Hide duration is only required when <see cref="m_skipShowDuration"/> is positive (timed show/hide cycle).
		/// </summary>
		public bool IsSkipConfigured => IsValidShowDuration && (m_skipShowDuration < 0.0f || m_skipHideDuration >= MinHideDuration);

		private Action m_onClickedTrigger = null;

		private IDisposable m_showDelaySubscription = null;
		private IDisposable m_hideDelaySubscription = null;

		public override void Open(object param)
		{
			if(!_TryGetOpenParam(param,out Param skipParam,isRequired:false))
			{
				return;
			}

			if(!IsSkipConfigured)
			{
				_FailOpen($"{NameTag} skip is not configured.");

				return;
			}

			if(skipParam?.OnClicked == null)
			{
				_FailOpen($"{NameTag} requires {nameof(Param.OnClicked)}.");

				return;
			}

			_CancelDelaySubscriptions();

			base.Open(param);

			m_onClickedTrigger = skipParam.OnClicked;

			if(m_skipShowDuration < 0.0f)
			{
				// Always-visible skip; no show/hide timer cycle.
				_SetButtonsState(isSkipActive : true,isTriggerActive : false);

				return;
			}

			// Both hidden until show delay elapses — reduces accidental skips.
			_SetButtonsState(isSkipActive : false,isTriggerActive : false);
			_ShowSkipButton(m_skipShowDuration);
		}

		public override void Close()
		{
			_CancelDelaySubscriptions();

			m_onClickedTrigger = null;

			base.Close();
		}

		protected override void _OnEnable()
		{
			base._OnEnable();

			// Open validates at runtime; OnEnable may run earlier (editor, prefab) with unassigned buttons.
			if(m_triggerButton)
			{
				m_triggerButton.onClick.AddAction(_OnClickedTrigger);
			}

			if(m_skipButton)
			{
				m_skipButton.onClick.AddAction(_OnClickedSkip);
			}
		}

		protected override void _OnDisable()
		{
			base._OnDisable();

			if(m_triggerButton)
			{
				m_triggerButton.onClick.RemoveAction(_OnClickedTrigger);
			}

			if(m_skipButton)
			{
				m_skipButton.onClick.RemoveAction(_OnClickedSkip);
			}

			_CancelDelaySubscriptions();
		}

		private void _SetButtonsState(bool isSkipActive,bool isTriggerActive)
		{
			m_skipButton.gameObject.EnsureActive(isSkipActive);
			m_triggerButton.gameObject.EnsureActive(isTriggerActive);
		}

		private void _OnClickedTrigger()
		{
			// Re-show skip immediately after the user taps the trigger zone.
			_ShowSkipButton(0.0f);
		}

		private void _OnClickedSkip()
		{
			m_onClickedTrigger?.Invoke();

			if(m_clickSoundClip)
			{
				SoundManager.In.PlayEffect2D(m_clickSoundClip);
			}

			// Close via UIManager so the owning session (video / cut scene) stays consistent.
			_SelfClose();
		}

		/// <summary>
		/// Schedules skip visibility after <paramref name="delayTime"/> seconds, then auto-hides after <see cref="m_skipHideDuration"/>.
		/// </summary>
		private void _ShowSkipButton(float delayTime)
		{
			_CancelDelaySubscriptions();

			void _PlayAction()
			{
				_SetButtonsState(isSkipActive : true,isTriggerActive : false);

				m_hideDelaySubscription = KZExternalKit.DelayAction(_HideSkipButton,m_skipHideDuration);
			}

			m_showDelaySubscription = KZExternalKit.DelayAction(_PlayAction,delayTime);
		}

		private void _HideSkipButton()
		{
			KZExternalKit.KillSubscription(ref m_hideDelaySubscription);

			// Leave the trigger zone active so the user can bring skip back.
			_SetButtonsState(isSkipActive : false,isTriggerActive : true);
		}

		/// <summary>Cancels pending show/hide timers; safe to call from Open, Close, and Disable.</summary>
		private void _CancelDelaySubscriptions()
		{
			KZExternalKit.KillSubscription(ref m_showDelaySubscription);
			KZExternalKit.KillSubscription(ref m_hideDelaySubscription);
		}
	}
}