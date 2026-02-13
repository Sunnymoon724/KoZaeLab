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
				LogChannel.Network.I("Network Info");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogChannel.Editor.W("Editor Warning");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogChannel.System.E("System Error");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
			}
		}
	}
}