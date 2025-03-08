using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZTest
{
	public class ScheduleTest : BaseComponent
	{
		[InfoBox("Space -> 스케쥴 시작 / Q -> 캔슬")]
		[SerializeField,LabelText("시작할 스케쥴")]
		private Schedule m_Schedule = null;

		private readonly Stopwatch m_Stopwatch = new();

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				PlayTestAsync().Forget();
			}

			if(Input.GetKeyDown(KeyCode.Q))
			{
				m_Schedule.gameObject.EnsureActive(false);
			}
		}

		private async UniTaskVoid PlayTestAsync()
		{
			m_Schedule.gameObject.EnsureActive(true);

			LogTag.Test.I("스케쥴 테스트 시작");

			m_Stopwatch.Start();

			await m_Schedule.PlayScheduleAsync();

			m_Stopwatch.Stop();

			LogTag.Test.I("스케쥴 테스트 종료 [경과 시간: {0}초]",m_Stopwatch.ElapsedMilliseconds/1000.0f);
		}
	}
}