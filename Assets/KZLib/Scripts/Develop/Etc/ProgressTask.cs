using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public abstract class ProgressTask : SerializedMonoBehaviour
	{
		public record ProgressParam(bool IsReverse = false);

		[SerializeField,HideInInspector]
		private float m_duration = 0.0f;

		[SerializeField,HideInInspector]
		private float m_progress = 0.0f;

		[BoxGroup("General Option",Order = 5),SerializeField]
		protected bool m_autoPlay = false;

		protected virtual bool IsDurationLock => false;

		[BoxGroup("General Option",Order = 5),ShowInInspector,DisableIf(nameof(IsDurationLock))]
		public float Duration { get => m_duration; protected set => m_duration = value; }

		public bool IsPlayable => m_duration > 0.0f;

		[BoxGroup("Progress",Order = 99,ShowLabel = false),ShowInInspector,PropertyRange(0.0f,1.0f)]
		public virtual float Progress
		{
			get => m_progress;
			set
			{
				m_progress = Mathf.Clamp01(value);

				SetProgress(value);
			}
		}

		[BoxGroup("General Option",Order = 5),SerializeField,PropertyTooltip("-1 is infinite loop / 0 is not working")]
		protected int m_loopCount = 1;

		[BoxGroup("General Option",Order = 5),SerializeField]
		protected bool m_ignoreTimeScale = false;

		private bool m_isReverse = false;

		public event Action<float> OnProgressUpdate = null;
		public event Action OnProgressStart = null;
		public event Action OnProgressComplete = null;

		protected CancellationTokenSource m_tokenSource = null;

		public bool IsPlaying => m_tokenSource != null;

		protected abstract void SetProgress(float progress);

		protected virtual void OnEnable()
		{
			if(m_autoPlay)
			{
				PlayProgress();
			}
		}

		protected virtual void OnDisable()
		{
			_KillProgress();
		}

		protected virtual void OnDestroy()
		{
			_KillProgress();
		}

		public void ResetProgress()
		{
			Progress = 0.0f;
		}

		private void _KillProgress()
		{
			CommonUtility.KillTokenSource(ref m_tokenSource);

			OnProgressStart = null;
			OnProgressComplete = null;
		}

		public void PlayProgress(ProgressParam progressParam = null)
		{
			PlayProgressAsync(progressParam).Forget();
		}

		public async UniTask PlayProgressAsync(ProgressParam progressParam = null)
		{
			SetParamData(progressParam);

			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			StartSchedule();

			await _PlayProgressAsync();

			CompleteSchedule();

			CommonUtility.KillTokenSource(ref m_tokenSource);
		}

		protected virtual void SetParamData(ProgressParam progressParam)
		{
			var param = progressParam ?? new ProgressParam();

			m_isReverse = param.IsReverse;
		}

		private async UniTask _PlayProgressAsync()
		{
			if(Duration <= 0.0f)
			{
				LogSvc.System.W("Duration must be greater than 0.0f.");

				return;
			}

			if(m_loopCount == 0)
			{
				LogSvc.System.W("loop count is zero.");

				return;
			}

			var start = m_isReverse ? 1.0f : 0.0f;
			var finish = m_isReverse ? 0.0f : 1.0f;

			await CommonUtility.LoopUniTaskAsync(async ()=>
			{
				await CommonUtility.ExecuteProgressAsync(start,finish,Duration,(progress)=>
				{
					OnProgressUpdate?.Invoke(progress);
					Progress = progress;
				},m_ignoreTimeScale,null,m_tokenSource.Token);
			},m_loopCount,m_tokenSource.Token);
		}

		public virtual void ResetSchedule() { }

		protected virtual void StartSchedule() { OnProgressStart?.Invoke(); }
		protected virtual void CompleteSchedule() { OnProgressComplete?.Invoke(); }
	}
}