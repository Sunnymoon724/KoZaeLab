using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;
using Sirenix.OdinInspector;

namespace KZLib.UI
{
	/// <summary>
	/// Full-screen video playback panel with optional <see cref="SubtitlePanel"/> and <see cref="SkipPanel"/> links.
	/// Typical flow: <see cref="PrepareVideoAsync"/> → <see cref="PlayVideo"/> → <see cref="WaitUntilPlaybackEndsAsync"/>.
	/// </summary>
	public class VideoPanel : BasePanel
	{
		[SerializeField,Required]
		private RawImage m_screenImage = null;
		[SerializeField,Required]
		private VideoPlayer m_videoPlayer = null;
		[SerializeField]
		private AspectRatioFitter m_aspectRatio = null;

		public TimeSpan Duration => IsPrepared ? TimeSpan.FromSeconds(m_videoPlayer.frameCount/m_videoPlayer.frameRate) : TimeSpan.Zero;
		public float Time => IsPrepared ? m_videoPlayer.frame/m_videoPlayer.frameRate : 0.0f;

		public bool IsPlaying => m_videoPlayer != null && m_videoPlayer.isPlaying;
		public bool IsPrepared => m_videoPlayer != null && m_videoPlayer.isPrepared;

		private CancellationTokenSource m_tokenSource = null;

		private bool m_hasPrepareError = false;

		private readonly Subject<float> m_videoTimeSubject = new();

		/// <summary>
		/// Playback time in seconds for linked panels (e.g. <see cref="SubtitlePanel"/>).
		/// Emits on play, seek, and every 100 ms while playing; <c>-1</c> on <see cref="Stop"/> to clear subtitles.
		/// </summary>
		public Observable<float> OnChangedVideoTime => m_videoTimeSubject;

		public override void Open(object param)
		{
			m_hasPrepareError = false;

			base.Open(param);

			// Hide video output until PlayVideo assigns the prepared render texture.
			m_screenImage.color = Color.black;

			m_videoPlayer.errorReceived += _ReceiveError;
			m_videoPlayer.loopPointReached += _IsVideoEnd;
		}

		public override void Close()
		{
			m_hasPrepareError = false;

			KZExternalKit.KillTokenSource(ref m_tokenSource);

			if(m_videoPlayer)
			{
				m_videoPlayer.errorReceived -= _ReceiveError;
				m_videoPlayer.loopPointReached -= _IsVideoEnd;
			}

			_ResetPlayback(notifySubtitleClear : false);

			// Linked subtitle/skip panels are closed in base.Close.
			base.Close();
		}

		protected override void _Release()
		{
			// Panel is pooled; dispose the time stream when the instance is destroyed.
			m_videoTimeSubject.Dispose();

			base._Release();
		}

		private void _ReceiveError(VideoPlayer _,string message)
		{
			LogChannel.UI.E(message);

			m_hasPrepareError = true;

			Stop();
		}

		private void _IsVideoEnd(VideoPlayer _)
		{
			// Looping clips keep playing; non-looping playback ends via Stop for WaitUntilPlaybackEndsAsync.
			if(!m_videoPlayer.isLooping)
			{
				Stop();
			}
		}

		/// <summary>Loads clip or URL, opens linked panels, and waits until <see cref="VideoPlayer"/> is prepared.</summary>
		public async UniTask PrepareVideoAsync(VideoInfo videoInfo)
		{
			if(videoInfo == null)
			{
				throw new ArgumentNullException(nameof(videoInfo));
			}

			m_hasPrepareError = false;

			// Drop subtitle/skip from a previous WatchVideo session before rebinding.
			_CloseLinkedWindows();

			m_videoPlayer.isLooping = videoInfo.IsLoop;

			if(videoInfo.IsUrl)
			{
				if(videoInfo.VideoPath.IsEmpty())
				{
					throw new InvalidOperationException("Video URL is empty.");
				}

				m_videoPlayer.clip = null;
				m_videoPlayer.source = VideoSource.Url;
				m_videoPlayer.url = videoInfo.VideoPath;
			}
			else
			{
				// Clear the other source so VideoPlayer does not keep the previous mode active.
				m_videoPlayer.url = string.Empty;
				m_videoPlayer.source = VideoSource.VideoClip;

				var videoClip = ResourceManager.In.GetVideoClip(videoInfo.VideoPath);

				if(!videoClip)
				{
					throw new InvalidOperationException($"Video path is wrong. [{videoInfo.VideoPath}]");
				}

				m_videoPlayer.clip = videoClip;
			}

			if(videoInfo.IsExistSubtitle)
			{
				var subtitlePanel = UIManager.In.Open(CommonUINameTag.SubtitlePanel,new SubtitlePanel.Param(videoInfo.SubtitlePath)) as SubtitlePanel;

				if(subtitlePanel == null)
				{
					LogChannel.UI.E("SubtitlePanel is not registered.");
				}
				else
				{
					AddLink(subtitlePanel);

					subtitlePanel.LinkVideo(this);
				}
			}

			if(videoInfo.CanSkip)
			{
				_TryOpenSkipPanel();
			}

			m_videoPlayer.Prepare();

			await WaitForPreparedAsync();

			if(m_hasPrepareError)
			{
				throw new InvalidOperationException("Video prepare failed.");
			}

			if(!IsPrepared)
			{
				throw new InvalidOperationException("Video was not prepared.");
			}
		}

		public async UniTask WaitForPreparedAsync()
		{
			bool _IsDone()
			{
				return IsPrepared || m_hasPrepareError;
			}

			await UniTask.WaitUntil(_IsDone).SuppressCancellationThrow();
		}

		/// <summary>Waits until playback stops (natural end, skip, or <see cref="Stop"/>).</summary>
		public async UniTask WaitUntilPlaybackEndsAsync()
		{
			bool _IsPlaying()
			{
				return IsPlaying;
			}

			await UniTask.WaitWhile(_IsPlaying).SuppressCancellationThrow();
		}

		public void PlayVideo()
		{
			if(!IsPrepared || !m_videoPlayer.texture)
			{
				throw new InvalidOperationException("Video is not prepared.");
			}

			var texture = m_videoPlayer.texture;

			m_screenImage.color = Color.white;
			m_screenImage.texture = texture;

			if(m_aspectRatio)
			{
				m_aspectRatio.aspectRatio = texture.width/(float)texture.height;
			}

			m_videoPlayer.Play();

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			// Notify before the 100 ms polling loop so the first cue appears without delay.
			_NotifyVideoTime(Time);

			_UpdateVideoStateAsync(m_tokenSource.Token).Forget();
		}

		public void Stop()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);

			// Notify linked SubtitlePanel before linked windows are torn down on panel Close.
			_ResetPlayback(notifySubtitleClear : true);
		}

		/// <summary>Pushes the current or overridden playback time to <see cref="OnChangedVideoTime"/> subscribers.</summary>
		private void _NotifyVideoTime(float time)
		{
			m_videoTimeSubject.OnNext(time);
		}

		private async UniTaskVoid _UpdateVideoStateAsync(CancellationToken token)
		{
			// Poll while playing; play/seek/stop also notify immediately via _NotifyVideoTime.
			while(m_videoPlayer.isPlaying && !token.IsCancellationRequested)
			{
				_NotifyVideoTime(Time);

				await UniTask.Delay(100,cancellationToken: token).SuppressCancellationThrow();
			}
		}

		public void JumpTime(TimeSpan timeSpan)
		{
			JumpFrame((long)(timeSpan.TotalSeconds*m_videoPlayer.frameRate));
		}

		public void JumpPosition(float percent)
		{
			JumpFrame((long)(percent*m_videoPlayer.frameCount));
		}

		public void JumpFrame(long frame)
		{
			// Relative seek; frame == 0 is a no-op.
			if(!IsPrepared || frame == 0)
			{
				return;
			}

			SeekFrame(m_videoPlayer.frame+frame);
		}

		public void SeekTimeSpan(TimeSpan timeSpan)
		{
			SeekFrame((long)(timeSpan.TotalSeconds*m_videoPlayer.frameRate));
		}

		public void SeekPosition(float percent)
		{
			SeekFrame((long)(percent*m_videoPlayer.frameCount));
		}

		public void SeekFrame(long frame)
		{
			if(!IsPrepared)
			{
				return;
			}

			m_videoPlayer.frame = Math.Clamp(frame,0L,(long)m_videoPlayer.frameCount);

			// Keep subtitles in sync when seeking outside the playback polling loop.
			_NotifyVideoTime(Time);
		}

		/// <summary>
		/// Stops playback and clears clip/url. Optionally emits <c>-1</c> for <see cref="SubtitlePanel"/>.
		/// </summary>
		/// <param name="notifySubtitleClear"><c>false</c> on <see cref="Close"/> because linked panels close separately.</param>
		private void _ResetPlayback(bool notifySubtitleClear)
		{
			if(m_videoPlayer)
			{
				m_videoPlayer.Stop();
				m_videoPlayer.clip = null;
				m_videoPlayer.url = string.Empty;
			}

			if(m_screenImage)
			{
				m_screenImage.texture = null;
				m_screenImage.color = Color.black;
			}

			if(notifySubtitleClear)
			{
				// Negative value is a clear signal for SubtitlePanel, not a playback timestamp.
				_NotifyVideoTime(-1f);
			}
		}

		/// <summary>Opens skip when available; logs and continues when SkipPanel is busy or misconfigured.</summary>
		private void _TryOpenSkipPanel()
		{
			if(UIManager.In.IsOpened(CommonUINameTag.SkipPanel))
			{
				// Cut-scene skip owns the shared SkipPanel; video plays without skip UI.
				LogChannel.UI.W("SkipPanel is already in use. Video skip was not opened.");

				return;
			}

			var skipPanel = UIManager.In.Register(CommonUINameTag.SkipPanel) as SkipPanel;

			if(skipPanel == null)
			{
				LogChannel.UI.E("SkipPanel is not registered.");

				return;
			}

			if(!skipPanel.IsSkipConfigured)
			{
				LogChannel.UI.E("SkipPanel is not configured for skip. Set a non-zero show duration and hide duration at least 0.02s.");

				return;
			}

			UIManager.In.Open(CommonUINameTag.SkipPanel,new SkipPanel.Param(Stop));

			AddLink(skipPanel);
		}
	}
}
