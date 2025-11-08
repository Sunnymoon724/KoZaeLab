using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

namespace KZLib.KZSample
{
    public class HudTestController : MonoBehaviour
	{
		private void Start()
		{
			UIManager.In.Open<HudPanelUI>(Global.HUD_PANEL_UI);

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