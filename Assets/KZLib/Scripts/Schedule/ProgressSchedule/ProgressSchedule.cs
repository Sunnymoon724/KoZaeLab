using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public abstract class ProgressSchedule : Schedule
	{
		public record ProgressParam(float? Duration = null,bool IsReverse = false) : ScheduleParam;

		[SerializeField,HideInInspector]
		private float m_Duration = 0.0f;

		[SerializeField,HideInInspector]
		private float m_Progress = 0.0f;

		protected virtual bool DurationLock => false;

		[FoldoutGroup("General Option",Order = 5),ShowInInspector,LabelText("Duration"),DisableIf(nameof(DurationLock))]
		public float Duration { get => m_Duration; protected set => m_Duration = value; }

		public bool IsPlayable => m_Duration > 0.0f;

		[BoxGroup("Progress",Order = 99,ShowLabel = false),ShowInInspector,PropertyRange(0.0f,1.0f),LabelText("Progress")]
		public virtual float Progress
		{
			get => m_Progress;
			set
			{
				m_Progress = Mathf.Clamp01(value);

				SetProgress(value);
			}
		}

		[FoldoutGroup("General Option",Order = 5),SerializeField,LabelText("Loop Count"),PropertyTooltip("-1 is infinite loop / 0 is not working")]
		protected int m_LoopCount = 1;

		[FoldoutGroup("General Option",Order = 5),SerializeField,LabelText("Ignore TimeScale")]
		protected bool m_IgnoreTimeScale = false;

		public NewAction<float> onProgress = new();

		protected abstract void SetProgress(float _progress);

		public override void ResetSchedule()
		{
			Progress = 0.0f;
		}

		protected async override UniTask DoPlayScheduleAsync(ScheduleParam _param)
		{
			var param = _param as ProgressParam ?? new ProgressParam();
			var duration = param.Duration ?? Duration;

			if(duration <= 0.0f)
			{
				LogTag.System.W("Duration must be greater than 0.0f.");

				return;
			}

			var start = param.IsReverse ? 1.0f : 0.0f;
			var finish = param.IsReverse ? 0.0f : 1.0f;

			await CommonUtility.LoopUniTaskAsync(async ()=>
			{
				await CommonUtility.ExecuteOverTimeAsync(start,finish,duration,(progress)=>
				{
					onProgress?.Invoke(progress);
					Progress = progress;
				},m_IgnoreTimeScale,null,m_TokenSource.Token);
			},m_LoopCount,m_TokenSource.Token);
		}
	}
}