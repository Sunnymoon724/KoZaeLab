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
			LogTag.Server.I("서버 인포");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.Editor.W("에디터 워닝");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

			LogTag.Sound.E("사운드 에러");

			await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		}
	}
}