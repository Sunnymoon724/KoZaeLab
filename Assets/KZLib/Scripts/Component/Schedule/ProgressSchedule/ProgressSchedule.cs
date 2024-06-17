using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ProgressSchedule : Schedule
{
	public record ProgressParam(float? Duration = null,bool IsReverse = false) : ScheduleParam;

	[SerializeField,HideInInspector]
	private float m_Duration = 0.0f;

	[SerializeField,HideInInspector]
	private float m_Progress = 0.0f;

	protected virtual bool DurationLock => false;

	[FoldoutGroup("기본 옵션",Order = 5),ShowInInspector,LabelText("진행 시간"),DisableIf(nameof(DurationLock))]
	public float Duration { get => m_Duration; protected set => m_Duration = value; }

	public bool IsPlayable => m_Duration > 0.0f;

	[VerticalGroup("프로그래스",Order = 99),ShowInInspector,PropertyRange(0.0f,1.0f),LabelText("진행 상황")]
	public virtual float Progress
	{
		get => m_Progress;
		set
		{
			m_Progress = Mathf.Clamp01(value);

			SetProgress(value);
		}
	}

	[FoldoutGroup("기본 옵션",Order = 5),SerializeField,LabelText("반복 횟수"),PropertyTooltip("-1은 무한/0은 작동 안함")]
	protected int m_LoopCount = 1;

	[FoldoutGroup("기본 옵션",Order = 5),SerializeField,LabelText("시간 무시")]
	protected bool m_IgnoreTimeScale = false;

	private Action<float> m_OnProgress = null;

	public event Action<float> OnProgress
	{
		add { m_OnProgress -= value; m_OnProgress += value; }
		remove { m_OnProgress -= value; }
	}

	protected abstract void SetProgress(float _progress);

	public override void ResetSchedule()
	{
		Progress = 0.0f;
	}

	protected async override UniTask DoPlayScheduleAsync(ScheduleParam _param)
	{
		var param = _param == null ? new ProgressParam() : _param as ProgressParam;

		var duration = param.Duration ?? Duration;

		if(duration <= 0.0f)
		{
			Log.System.W("진행 시간이 0 미만인 애니메이션은 실행할 수 없습니다.");

			return;
		}

		var start = param.IsReverse ? 1.0f : 0.0f;
		var finish = param.IsReverse ? 0.0f : 1.0f;

		await CommonUtility.LoopUniTaskAsync(async ()=>
		{
			await CommonUtility.ExecuteOverTimeAsync(start,finish,duration,(progress)=>
			{
				m_OnProgress?.Invoke(progress);
				Progress = progress;
			},m_IgnoreTimeScale,null,m_Source.Token);
		},m_LoopCount,m_Source.Token);
	}
}