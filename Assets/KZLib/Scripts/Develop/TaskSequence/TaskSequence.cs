using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Development
{
	public abstract class TaskSequence : MonoBehaviour
	{
		public record Param();

		[FoldoutGroup("General",Order = 1)]
		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected bool m_autoPlay = false;

		[VerticalGroup("General/0",Order = 0),SerializeField]
		protected Button m_skipButton = null;

		[VerticalGroup("General/0",Order = 0),SerializeField,PropertyTooltip("-1 is infinite loop / 0 is not working")]
		protected int m_loopCount = 1;

		public bool IsPlaying => m_isPlaying;

		private readonly Subject<Unit> m_startTaskSubject = new();
		public Observable<Unit> OnStartedTask => m_startTaskSubject;

		private readonly Subject<Unit> m_finishTaskSubject = new();
		public Observable<Unit> OnFinishedTask => m_finishTaskSubject;

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

		protected virtual void _Initialize() { }

		private void OnEnable()
		{
			if(m_autoPlay)
			{
				PlaySequence();
			}

			_OnEnable();
		}

		protected virtual void _OnEnable() { }

		private void OnDisable()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);

			_OnDisable();
		}
		
		protected virtual void _OnDisable() { }

		public void PlaySequence(Param sequenceParam = null)
		{
			PlaySequenceAsync(sequenceParam).Forget();
		}

		public async virtual UniTask PlaySequenceAsync(Param sequenceParam = null)
		{
			if(m_loopCount == 0)
			{
				LogChannel.Develop.W("loop count is zero.");

				return;
			}

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			m_isPlaying = true;

			_StartSequence();

			async UniTask _PlayAsync()
			{
				await _DoPlaySequenceAsync(sequenceParam);
			}

			await KZExternalKit.LoopUniTaskAsync(_PlayAsync,m_loopCount,m_tokenSource.Token).SuppressCancellationThrow();

			_FinishSequence();

			m_isPlaying = false;

			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		public void CancelSequence()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		public virtual void ResetSequence() { }

		protected virtual void _StartSequence() { m_startTaskSubject.OnNext(Unit.Default); }
		protected virtual void _FinishSequence() { m_finishTaskSubject.OnNext(Unit.Default); }

		protected abstract UniTask _DoPlaySequenceAsync(Param sequenceParam);
	}
}