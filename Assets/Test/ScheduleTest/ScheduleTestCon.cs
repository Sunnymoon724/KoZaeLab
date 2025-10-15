// using System.Diagnostics;
// using Cysharp.Threading.Tasks;
// using Sirenix.OdinInspector;
// using UnityEngine;

// namespace KZLib.KZTest
// {
// 	public class ScheduleTest : BaseComponent
// 	{
// 		[InfoBox("Space -> Schedule Start / Q -> Schedule Cancel")]
// 		[SerializeField]
// 		private Schedule m_Schedule = null;

// 		private readonly Stopwatch m_Stopwatch = new();

// 		private void Update()
// 		{
// 			if(Input.GetKeyDown(KeyCode.Space))
// 			{
// 				PlayTestAsync().Forget();
// 			}

// 			if(Input.GetKeyDown(KeyCode.Q))
// 			{
// 				m_Schedule.gameObject.EnsureActive(false);
// 			}
// 		}

// 		private async UniTaskVoid PlayTestAsync()
// 		{
// 			m_Schedule.gameObject.EnsureActive(true);

// 			Logger.Test.I("Schedule test start");

// 			m_Stopwatch.Start();

// 			await m_Schedule.PlayScheduleAsync();

// 			m_Stopwatch.Stop();

// 			Logger.Test.I("Schedule test stop [duration: {0}s]",m_Stopwatch.ElapsedMilliseconds/1000.0f);
// 		}
// 	}
// }