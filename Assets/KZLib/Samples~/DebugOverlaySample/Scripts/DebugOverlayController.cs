using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KZLib.KZSample.Develop
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
				LogSvc.Network.I("Network Info");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogSvc.Editor.W("Editor Warning");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

				LogSvc.System.E("System Error");

				await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
			}
		}
	}
}