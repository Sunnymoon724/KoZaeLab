using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample.Develop
{
	public class TaskSequenceController : BaseComponent
	{
		[InfoBox("Space -> TaskSequence Start / Q -> TaskSequence Cancel")]
		[SerializeField]
		private TaskSequence m_taskSequence = null;

		private readonly Stopwatch m_stopwatch = new();

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				PlayTestAsync().Forget();
			}

			if(Input.GetKeyDown(KeyCode.Q))
			{
				m_taskSequence.CancelSequence();
			}
		}

		private async UniTaskVoid PlayTestAsync()
		{
			LogSvc.Test.I("Schedule test start");

			m_stopwatch.Start();

			await m_taskSequence.PlaySequenceAsync();

			m_stopwatch.Stop();

			LogSvc.Test.I($"Schedule test stop [duration: {m_stopwatch.ElapsedMilliseconds/1000.0f}s]");
		}
	}
}