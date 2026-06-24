using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;
using MessagePipe;
using R3;

namespace KZLib
{
	/// <summary>
	/// Polls screen display metrics while detection is active and publishes
	/// <see cref="CommonNoticeTag.ChangedDeviceResolution"/> when resolution or safe area changes.
	/// Polling is used instead of platform callbacks for consistent cross-platform behavior.
	/// </summary>
	public class ResolutionMonitor
	{
		/// <summary>Interval between display checks while the app is in the foreground.</summary>
		private const float c_pollIntervalSeconds = 0.1f;

		private Vector2Int m_resolution = Vector2Int.zero;
		private Rect m_safeArea = Rect.zero;
		private CancellationTokenSource m_tokenSource = null;
		private readonly Action<Vector2Int> m_onResolutionChanged = null;

		public ResolutionMonitor(Action<Vector2Int> onResolutionChanged = null)
		{
			m_onResolutionChanged = onResolutionChanged;

			_SyncDisplayState(notify: false);
		}

		/// <summary>Starts or restarts foreground display polling.</summary>
		public void StartResolutionDetection()
		{
			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			// Notify when display changed while detection was stopped (e.g. rotation in background).
			_SyncDisplayState(notify: _HasDisplayChanged());

			_CheckResolutionDetectionAsync(m_tokenSource.Token).Forget();
		}

		/// <summary>Stops foreground display polling.</summary>
		public void StopResolutionDetection()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		private bool _HasDisplayChanged()
		{
			return m_resolution.x != Screen.width || m_resolution.y != Screen.height || m_safeArea != Screen.safeArea;
		}

		private void _SyncDisplayState(bool notify)
		{
			m_resolution = new Vector2Int(Screen.width,Screen.height);
			m_safeArea = Screen.safeArea;

			if(notify)
			{
				m_onResolutionChanged?.Invoke(m_resolution);

				GlobalMessagePipe.GetPublisher<CommonNoticeTag,Unit>().Publish(CommonNoticeTag.ChangedDeviceResolution,Unit.Default);
			}
		}

		private async UniTaskVoid _CheckResolutionDetectionAsync(CancellationToken token)
		{
			while(!token.IsCancellationRequested)
			{
				// Read after the frame ends so layout/orientation has settled.
				await UniTask.WaitForEndOfFrame(token).SuppressCancellationThrow();

				if(token.IsCancellationRequested)
				{
					break;
				}

				if(_HasDisplayChanged())
				{
					_SyncDisplayState(notify: true);
				}

				// Realtime so detection still works when Time.timeScale is zero.
				await UniTask.Delay(TimeSpan.FromSeconds(c_pollIntervalSeconds),DelayType.Realtime,cancellationToken: token).SuppressCancellationThrow();
			}
		}
	}
}
