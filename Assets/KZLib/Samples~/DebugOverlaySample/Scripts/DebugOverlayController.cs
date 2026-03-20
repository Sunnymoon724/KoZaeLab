using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KZLib.Samples.Develop
{
    public class DebugOverlayController : MonoBehaviour
	{
		private void Start()
		{
			DebugOverlayManager.In.ShowOverlay();

			LoopTextAsync().Forget();
		}

		private async UniTaskVoid LoopTextAsync()
		{
			while(true)
			{
				LogChannel.Test.I("Test Info");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogChannel.Test.W("Test Warning");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogChannel.Test.E("Test Error");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
			}
		}
	}
}