using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public abstract class Schedule : BaseComponent
	{
		public record ScheduleParam();

		[FoldoutGroup("기본 옵션",Order = 5),SerializeField,LabelText("자동 재생")]
		protected bool m_AutoPlay = false;

		protected CancellationTokenSource m_TokenSource = null;

		public bool IsPlaying => m_TokenSource != null;

		private Action m_OnStart = null;
		public event Action OnStart
		{
			add { m_OnStart -= value; m_OnStart += value; }
			remove { m_OnStart -= value; }
		}

		private Action m_OnComplete = null;
		public event Action OnComplete
		{
			add { m_OnComplete -= value; m_OnComplete += value; }
			remove { m_OnComplete -= value; }
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if(m_AutoPlay)
			{
				PlaySchedule();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			KillSchedule();
		}

		protected override void Release()
		{
			base.Release();

			KillSchedule();
		}

		private void KillSchedule()
		{
			CommonUtility.KillTokenSource(ref m_TokenSource);

			m_OnStart = null;
			m_OnComplete = null;
		}

		public void PlaySchedule(ScheduleParam _param = null)
		{
			PlayScheduleAsync(_param).Forget();
		}

		public async virtual UniTask PlayScheduleAsync(ScheduleParam _param = null)
		{
			CommonUtility.RecycleTokenSource(ref m_TokenSource);

			StartSchedule();

			await DoPlayScheduleAsync(_param);

			CompleteSchedule();

			CommonUtility.KillTokenSource(ref m_TokenSource);
		}

		public void Skip()
		{
			gameObject.SetActiveSelf(false);
		}

		public virtual void ResetSchedule() { }

		protected virtual void StartSchedule() { m_OnStart?.Invoke(); }
		protected virtual void CompleteSchedule() { m_OnComplete?.Invoke(); }

		protected abstract UniTask DoPlayScheduleAsync(ScheduleParam _param);
	}
}