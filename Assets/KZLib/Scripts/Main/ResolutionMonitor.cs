using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;
using MessagePipe;
using R3;

public class ResolutionMonitor
{
	private Vector2Int m_resolution;
	private CancellationTokenSource m_tokenSource = null;

	public ResolutionMonitor()
	{
		_SetScreen(false);
	}

	public void StartResolutionDetection()
	{
		KZExternalKit.RecycleTokenSource(ref m_tokenSource);

		_SetScreen(false);

		_CheckResolutionDetectionAsync(m_tokenSource.Token).Forget();
	}

	public void StopResolutionDetection()
	{
		KZExternalKit.KillTokenSource(ref m_tokenSource);
	}

	private void _SetScreen(bool notify)
	{
		m_resolution = new Vector2Int(Screen.width,Screen.height);

		if(notify)
		{
			GlobalMessagePipe.GetPublisher<CommonNoticeTag,Unit>().Publish(CommonNoticeTag.ChangedDeviceResolution,Unit.Default);
		}
	}

	private async UniTaskVoid _CheckResolutionDetectionAsync(CancellationToken token)
	{
		while(!token.IsCancellationRequested)
		{
			await UniTask.WaitForEndOfFrame(token).SuppressCancellationThrow();

			if(token.IsCancellationRequested)
			{
				break;
			}

			if(m_resolution.x != Screen.width || m_resolution.y != Screen.height)
			{
				_SetScreen(true);
			}

			await UniTask.Delay(TimeSpan.FromSeconds(0.1f),DelayType.Realtime,cancellationToken: token).SuppressCancellationThrow();
		}
	}
}