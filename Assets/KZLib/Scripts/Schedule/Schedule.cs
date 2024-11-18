using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public abstract class Schedule : BaseComponent
	{
		public record ScheduleParam();

		[FoldoutGroup("General Option",Order = 5),SerializeField,LabelText("Auto Play")]
		protected bool m_AutoPlay = false;

		protected CancellationTokenSource m_TokenSource = null;

		public bool IsPlaying => m_TokenSource != null;

		public NewAction onStart = new();

		public NewAction onComplete = new();

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

			onStart.RemoveAllListeners();
			onComplete.RemoveAllListeners();
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

		protected virtual void StartSchedule() { onStart?.Invoke(); }
		protected virtual void CompleteSchedule() { onComplete?.Invoke(); }

		protected abstract UniTask DoPlayScheduleAsync(ScheduleParam _param);
	}
}