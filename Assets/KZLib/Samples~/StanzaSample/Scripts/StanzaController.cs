using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Samples.Develop
{
	public class StanzaController : MonoBehaviour
	{
		[InfoBox("Space -> Stanza Start / Q -> Stanza Cancel")]
		[SerializeField]
		private Stanza m_stanza = null;

		private readonly Stopwatch m_stopwatch = new();

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				PlayTestAsync().Forget();
			}

			if(Input.GetKeyDown(KeyCode.Q))
			{
				m_stanza.Cancel();
			}
		}

		private async UniTaskVoid PlayTestAsync()
		{
			LogChannel.Test.I("Schedule test start");

			m_stopwatch.Start();

			await m_stanza.PlayAsync();

			m_stopwatch.Stop();

			LogChannel.Test.I($"Schedule test stop [duration: {m_stopwatch.ElapsedMilliseconds/1000.0f}s]");
		}
	}
}