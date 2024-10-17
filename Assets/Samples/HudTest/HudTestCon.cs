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
			LogTag.Server.I("Server Info");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.Editor.W("Editor Warning");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.Sound.E("Sound Error");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		}
	}
}