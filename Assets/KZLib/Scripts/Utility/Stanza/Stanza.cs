using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Utilities
{
	public abstract class Stanza : MonoBehaviour
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

		private readonly Subject<Unit> m_startSubject = new();
		public Observable<Unit> OnStarted => m_startSubject;

		private readonly Subject<Unit> m_finishSubject = new();
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

		protected virtual void _Initialize() { }

		private void OnEnable()
		{
			if(m_autoPlay)
			{
				Play();
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

		public void Play(Param param = null)
		{
			PlayAsync(param).Forget();
		}

		public async virtual UniTask PlayAsync(Param param = null)
		{
			if(m_loopCount == 0)
			{
				LogChannel.Develop.W("loop count is zero.");

				return;
			}

			if(m_isPlaying)
			{
				return;
			}

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			m_isPlaying = true;

			_Start();

			async UniTask _PlayAsync()
			{
				await _DoPlayAsync(param);
			}

			var isCanceled = await KZExternalKit.LoopUniTaskAsync(_PlayAsync,m_loopCount,m_tokenSource.Token).SuppressCancellationThrow();

			m_isPlaying = false;

			KZExternalKit.KillTokenSource(ref m_tokenSource);

			if(!isCanceled)
			{
				_Finish();
			}
		}

		public void Cancel()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		public virtual void ResetAll() { }

		protected virtual void _Start() { m_startSubject.OnNext(Unit.Default); }
		protected virtual void _Finish() { m_finishSubject.OnNext(Unit.Default); }

		protected abstract UniTask _DoPlayAsync(Param param);
	}
}