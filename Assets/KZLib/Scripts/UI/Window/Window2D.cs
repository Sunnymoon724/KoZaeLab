using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace KZLib.UI
{
	/// <summary>
	/// Screen-space 2D window with optional DOTween open/close sequences.
	/// <see cref="Hide"/> and <see cref="BlockUI"/> are tracked separately and merged in <see cref="_ApplyCanvasGroupState"/>.
	/// </summary>
	public abstract class Window2D : Window
	{
		public override bool IsBlocked => m_blocked || m_hidden;

		[BoxGroup("UI",Order = -10)]
		[VerticalGroup("UI/General",Order = 0),SerializeField]
		private bool m_pooling = true;
		public override bool IsPooling => m_pooling;

		[VerticalGroup("UI/General",Order = 0),SerializeField]
		private WindowPriorityType m_priorityType = WindowPriorityType.Middle;
		public override WindowPriorityType PriorityType => m_priorityType;

		public override bool Is3D => false;

		protected Sequence m_openSequence = null;
		protected Sequence m_closeSequence = null;

		private bool m_blocked = false;
		private bool m_hidden = false;

		public override void Open(object param)
		{
			m_hidden = false;

			base.Open(param);

			_ApplyCanvasGroupState();

			m_openSequence?.Restart();
		}

		/// <summary>Plays the close sequence when assigned; otherwise closes immediately.</summary>
		public override void Close()
		{
			if(m_closeSequence != null)
			{
				m_closeSequence.Restart();
			}
			else
			{
				base.Close();
			}
		}

		/// <summary>
		/// Builds or extends a paused DOTween sequence. <paramref name="onComplete"/> is registered only on first creation.
		/// </summary>
		protected void AddSequence(ref Sequence sequence,Tween tween,TweenCallback onComplete = null)
		{
			if(sequence == null)
			{
				sequence = DOTween.Sequence().SetAutoKill(false);
				sequence.Append(tween);
				sequence.SetUpdate(true);
				sequence.Pause();

				if(onComplete != null)
				{
					sequence.OnComplete(onComplete);
				}
			}
			else
			{
				sequence.Join(tween);
			}
		}

		protected virtual void _OnOpenSequenceComplete() { }

		protected virtual void _OnCloseSequenceComplete()
		{
			base.Close();
		}

		public override void Hide(bool isHidden)
		{
			if(m_hidden == isHidden)
			{
				return;
			}

			m_hidden = isHidden;

			if(isHidden)
			{
				LogChannel.UI.I($"{NameTag} is hidden");
			}
			else
			{
				LogChannel.UI.I($"{NameTag} is shown");
			}

			_ApplyCanvasGroupState();
		}

		/// <summary>Default back-button handler for 2D windows.</summary>
		public virtual void PressBackButton()
		{
			_SelfClose();
		}

		public override void BlockUI(bool isBlocked)
		{
			if(m_blocked == isBlocked)
			{
				return;
			}

			m_blocked = isBlocked;

			if(isBlocked)
			{
				LogChannel.UI.I($"{NameTag} input is blocked");
			}
			else
			{
				LogChannel.UI.I($"{NameTag} input is allowed");
			}

			_ApplyCanvasGroupState();
		}

		// hidden > blocked > normal
		private void _ApplyCanvasGroupState()
		{
			if(m_hidden)
			{
				_SetCanvasGroupState(0,false,false);
			}
			else if(m_blocked)
			{
				_SetCanvasGroupState(1,false,false);
			}
			else
			{
				_SetCanvasGroupState(1,true,true);
			}
		}
	}
}