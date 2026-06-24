using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Utilities
{
	/// <summary>
	/// Base MonoBehaviour for a single presentation step (stanza).
	/// Handles play lifecycle, loop count, skip button, cancellation, and start/finish observables.
	/// Subclasses implement <see cref="_DoPlayAsync"/> for the actual sequence.
	/// </summary>
	public abstract class Stanza : MonoBehaviour
	{
		/// <summary>Optional parameter bag passed into <see cref="PlayAsync"/>.</summary>
		public record Param();

		[FoldoutGroup("General",Order = 1)]
		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected bool m_autoPlay = false;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected Button m_skipButton = null;

		/// <summary>-1 = infinite loop, 0 = disabled.</summary>
		[VerticalGroup("General/0",Order = 0),SerializeField,PropertyTooltip("-1 is infinite loop / 0 is not working")]
		protected int m_loopCount = 1;

		/// <summary>True while <see cref="PlayAsync"/> is running.</summary>
		public bool IsPlaying => m_isPlaying;

		private readonly Subject<Unit> m_startSubject = new();
		/// <summary>Emitted when playback starts (after loop guard checks).</summary>
		public Observable<Unit> OnStarted => m_startSubject;

		private readonly Subject<Unit> m_finishSubject = new();
		/// <summary>Emitted when playback completes without cancellation.</summary>
		public Observable<Unit> OnFinished => m_finishSubject;

		private bool m_isPlaying = false;
		protected CancellationTokenSource m_tokenSource = null;

		private void Awake()
		{
			if(m_skipButton)
			{
				void _ClickButton()
				{
					gameObject.EnsureActive(false);
				}

				m_skipButton.onClick.SetAction(_ClickButton);
			}

			_Initialize();
		}

		/// <summary>Called once from <see cref="Awake"/>; override for setup before play.</summary>
		protected virtual void _Initialize() { }

		private void OnEnable()
		{
			if(m_autoPlay)
			{
				Play();
			}

			_OnEnable();
		}

		/// <summary>Called from <see cref="OnEnable"/> after optional auto-play.</summary>
		protected virtual void _OnEnable() { }

		private void OnDisable()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);

			_OnDisable();
		}
		
		/// <summary>Called from <see cref="OnDisable"/> after token cleanup.</summary>
		protected virtual void _OnDisable() { }

		/// <summary>Fire-and-forget wrapper around <see cref="PlayAsync"/>.</summary>
		public void Play(Param param = null)
		{
			PlayAsync(param).Forget();
		}

		/// <summary>
		/// Runs <see cref="_DoPlayAsync"/> up to <see cref="m_loopCount"/> times.
		/// Ignores overlapping calls while already playing.
		/// </summary>
		public async virtual UniTask PlayAsync(Param param = null)
		{
			if(m_loopCount == 0)
			{
				LogChannel.Develop.W("loop count is zero.");

				return;
			}

			if(m_isPlaying)
			{
				LogChannel.Develop.W($"{gameObject.name} is already playing.");

				return;
			}

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			m_isPlaying = true;

			_Start();

			var token = m_tokenSource.Token;

			async UniTask _PlayAsync()
			{
				await _DoPlayAsync(param);
			}

			var isCanceled = await KZExternalKit.LoopUniTaskAsync(_PlayAsync,m_loopCount,token).SuppressCancellationThrow();

			m_isPlaying = false;

			KZExternalKit.KillTokenSource(ref m_tokenSource);

			if(!isCanceled && !token.IsCancellationRequested)
			{
				_Finish();
			}
		}

		/// <summary>Cancels the current playback via the internal token source.</summary>
		public void Cancel()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		/// <summary>Resets subclass state; override in derived types.</summary>
		public virtual void ResetAll() { }

		protected virtual void _Start() { m_startSubject.OnNext(Unit.Default); }
		protected virtual void _Finish() { m_finishSubject.OnNext(Unit.Default); }

		/// <summary>Core sequence logic implemented by each stanza type.</summary>
		protected abstract UniTask _DoPlayAsync(Param param);
	}
}
