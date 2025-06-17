using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

public class HudTestCon : MonoBehaviour
{
	private void Start()
	{
		UIMgr.In.Open<HudPanelUI>(Global.HUD_PANEL_UI);

		LoopTextAsync().Forget();
	}

	private async UniTaskVoid LoopTextAsync()
	{
		while(true)
		{
			Logger.Network.I("Network Info");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			Logger.Editor.W("Editor Warning");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			Logger.System.E("System Error");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		}
	}
}