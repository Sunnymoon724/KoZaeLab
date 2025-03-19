using System;
using KZLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using VideoPanel;
using System.Threading;

public class VideoPanelUI : WindowUI2D
{
	[SerializeField] private RawImage m_screenImage = null;
	[SerializeField] private VideoPlayer m_videoPlayer = null;
	[SerializeField] private AspectRatioFitter m_aspectRatio = null;

	public override UITag Tag => UITag.VideoPanelUI;

	public TimeSpan Duration => TimeSpan.FromSeconds(m_videoPlayer.frameCount/m_videoPlayer.frameRate);

	public float Time => m_videoPlayer.frame/m_videoPlayer.frameRate;

	public bool IsPlaying => m_videoPlayer != null && m_videoPlayer.isPlaying;
	public bool IsPrepared => m_videoPlayer != null && m_videoPlayer.isPrepared;

	private CancellationTokenSource m_tokenSource = null;

	public event Action<float> OnVideoTimeUpdate;

	protected override void Initialize()
	{
		base.Initialize();

		m_videoPlayer.errorReceived += (player,message) => { throw new Exception(message); };

		m_videoPlayer.loopPointReached += (player) =>
		{
			if(!m_videoPlayer.isLooping)
			{
				Stop();
			}
		};
	}

	public override void Open(object param)
	{
		base.Open(param);

		m_screenImage.color = Color.black;

		m_videoPlayer.targetCamera = m_canvas.worldCamera;
	}

	public override void Close()
	{
		base.Close();

		m_videoPlayer.clip = null;
		m_screenImage.color = Color.black;

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	public async UniTask PrepareVideoAsync(VideoData videoData)
	{
		m_videoPlayer.isLooping = videoData.IsLoop;

		if(videoData.IsUrl)
		{
			m_videoPlayer.source = VideoSource.Url;
			m_videoPlayer.url = videoData.VideoPath;
		}
		else
		{
			m_videoPlayer.source = VideoSource.VideoClip;

			var videoClip = ResourceMgr.In.GetVideoClip(videoData.VideoPath);

			if(!videoClip)
			{
				LogTag.System.E($"Video path is wrong. [{videoData.VideoPath}]");

				return;
			}

			m_videoPlayer.clip = videoClip;
		}

		if(videoData.IsExistSubtitle)
		{
			var subtitlePanel = UIMgr.In.Open<SubtitlePanelUI>(UITag.SubtitlePanelUI,new SubtitlePanelUI.SubtitleParam(videoData.SubtitlePath));

			AddLink(subtitlePanel);

			OnVideoTimeUpdate -= subtitlePanel.SetSubtitle;
			OnVideoTimeUpdate += subtitlePanel.SetSubtitle;
		}

		if(videoData.CanSkip)
		{
			var skipPanel = UIMgr.In.Open<SkipPanelUI>(UITag.SkipPanelUI,new SkipPanelUI.SkipParam(Stop));

			AddLink(skipPanel);
		}

		m_videoPlayer.Prepare();

		await UniTask.WaitUntil(() => IsPrepared);
	}

	public async UniTask WaitForPreparedAsync()
	{
		await UniTask.WaitUntil(() => IsPrepared);
	}

	public async UniTask WaitForPlayingAsync()
	{
		await UniTask.WaitWhile(() => IsPlaying);
	}

	public void PlayVideo()
	{
		var texture = m_videoPlayer.texture;

		m_screenImage.color = Color.white;
		m_screenImage.texture = texture;

		m_aspectRatio.aspectRatio = texture.width/(float)texture.height;

		m_videoPlayer.Play();

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		UpdateVideoStateAsync().Forget();
	}

	public void Stop()
	{
		m_videoPlayer.Stop();

		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	private async UniTask UpdateVideoStateAsync()
	{
		while(m_videoPlayer.isPlaying)
		{
			m_tokenSource.Token.ThrowIfCancellationRequested();

			OnVideoTimeUpdate?.Invoke(Time);

			await UniTask.Delay(100,cancellationToken: m_tokenSource.Token);
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