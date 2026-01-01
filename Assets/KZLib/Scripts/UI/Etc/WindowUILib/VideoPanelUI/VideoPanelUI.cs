using System;
using KZLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using VideoPanel;
using System.Threading;
using KZLib.KZData;
using R3;

public class VideoPanelUI : BasePanelUI
{
	[SerializeField]
	private RawImage m_screenImage = null;
	[SerializeField]
	private VideoPlayer m_videoPlayer = null;
	[SerializeField]
	private AspectRatioFitter m_aspectRatio = null;

	public TimeSpan Duration => IsPrepared ? TimeSpan.FromSeconds(m_videoPlayer.frameCount/m_videoPlayer.frameRate) : TimeSpan.Zero;
	public float Time => IsPrepared ? m_videoPlayer.frame/m_videoPlayer.frameRate : 0.0f;

	public bool IsPlaying => m_videoPlayer != null && m_videoPlayer.isPlaying;
	public bool IsPrepared => m_videoPlayer != null && m_videoPlayer.isPrepared;

	private CancellationTokenSource m_tokenSource = null;

	private readonly Subject<float> m_videoTimeSubject = new();
	public Observable<float> OnChangedVideoTime => m_videoTimeSubject;

	public override void Open(object param)
	{
		base.Open(param);

		m_screenImage.color = Color.black;

		m_videoPlayer.errorReceived += _ReceiveError;
		m_videoPlayer.loopPointReached += _IsVideoEnd;
	}

	public override void Close()
	{
		m_videoPlayer.clip = null;
		m_screenImage.color = Color.black;

		CommonUtility.KillTokenSource(ref m_tokenSource);

		m_videoPlayer.errorReceived -= _ReceiveError;
		m_videoPlayer.loopPointReached -= _IsVideoEnd;

		base.Close();
	}

	private void _ReceiveError(VideoPlayer _,string message)
	{
		throw new Exception(message);
	}

	private void _IsVideoEnd(VideoPlayer _)
	{
		if(!m_videoPlayer.isLooping)
		{
			Stop();
		}
	}

	public async UniTask PrepareVideoAsync(VideoInfo videoInfo)
	{
		m_videoPlayer.isLooping = videoInfo.IsLoop;

		if(videoInfo.IsUrl)
		{
			m_videoPlayer.source = VideoSource.Url;
			m_videoPlayer.url = videoInfo.VideoPath;
		}
		else
		{
			m_videoPlayer.source = VideoSource.VideoClip;

			var videoClip = ResourceManager.In.GetVideoClip(videoInfo.VideoPath);

			if(!videoClip)
			{
				LogSvc.System.E($"Video path is wrong. [{videoInfo.VideoPath}]");

				return;
			}

			m_videoPlayer.clip = videoClip;
		}

		if(videoInfo.IsExistSubtitle)
		{
			var subtitlePanel = UIManager.In.Open(UINameType.SubtitlePanelUI,new SubtitlePanelUI.Param(videoInfo.SubtitlePath)) as SubtitlePanelUI;

			AddLink(subtitlePanel);

			subtitlePanel.LinkVideo(this);
		}

		if(videoInfo.CanSkip)
		{
			var skipPanel = UIManager.In.Open(UINameType.SkipPanelUI,new SkipPanelUI.Param(Stop)) as SkipPanelUI;

			AddLink(skipPanel);
		}

		m_videoPlayer.Prepare();

		await WaitForPreparedAsync();
	}

	public async UniTask WaitForPreparedAsync()
	{
		bool _IsPrepared()
		{
			return IsPrepared;
		}

		await UniTask.WaitUntil(_IsPrepared);
	}

	public async UniTask WaitForPlayingAsync()
	{
		bool _IsPlaying()
		{
			return IsPlaying;
		}

		await UniTask.WaitWhile(_IsPlaying);
	}

	public void PlayVideo()
	{
		var texture = m_videoPlayer.texture;

		m_screenImage.color = Color.white;
		m_screenImage.texture = texture;

		m_aspectRatio.aspectRatio = texture.width/(float)texture.height;

		m_videoPlayer.Play();

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		_UpdateVideoStateAsync(m_tokenSource.Token).Forget();
	}

	public void Stop()
	{
		m_videoPlayer.Stop();

		m_screenImage.texture = null;
		m_screenImage.color = Color.black;

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	private async UniTaskVoid _UpdateVideoStateAsync(CancellationToken token)
	{
		while(m_videoPlayer.isPlaying && !token.IsCancellationRequested)
		{
			m_videoTimeSubject.OnNext(Time);

			await UniTask.Delay(100,cancellationToken: token).SuppressCancellationThrow();
		}
	}

	public void JumpTime(TimeSpan timeSpan)
	{
		JumpFrame((long) (timeSpan.Seconds*m_videoPlayer.frameRate));
	}
	
	public void JumpPosition(float percent)
	{
		JumpFrame((long) (percent*m_videoPlayer.frameCount));
	}

	public void JumpFrame(long frame)
	{
		if(!m_videoPlayer.texture || frame == 0)
		{
			return;
		}

		SeekFrame(m_videoPlayer.frame+frame);
	}

	public void SeekTimeSpan(TimeSpan timeSpan)
	{
		SeekFrame((long) (timeSpan.TotalSeconds * m_videoPlayer.frameRate));
	}

	public void SeekPosition(float percent)
	{
		SeekFrame((long) (percent*m_videoPlayer.frameCount));
	}

	public void SeekFrame(long frame)
	{
		m_videoPlayer.frame = CommonUtility.Clamp(frame,0L,(long)m_videoPlayer.frameCount);
	}
}