using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

public class HudTestCon : MonoBehaviour
{
	private void Start()
	{
		UIManager.In.Open<HudPanelUI>(UITag.HudPanelUI);

		LoopTextAsync().Forget();
	}

	private async UniTaskVoid LoopTextAsync()
	{
		while(true)
		{
			LogTag.Network.I("Network Info");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.Editor.W("Editor Warning");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.System.E("System Error");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		}
	}
}