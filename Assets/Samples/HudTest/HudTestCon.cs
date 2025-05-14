using System;
using Cysharp.Threading.Tasks;
using KZLib;
using UnityEngine;

public class HudTestCon : MonoBehaviour
{
	private void Start()
	{
		UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);

		LoopTextAsync().Forget();
	}

	private async UniTaskVoid LoopTextAsync()
	{
		while(true)
		{
			KZLogType.Network.I("Network Info");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			KZLogType.Editor.W("Editor Warning");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			KZLogType.System.E("System Error");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		}
	}
}