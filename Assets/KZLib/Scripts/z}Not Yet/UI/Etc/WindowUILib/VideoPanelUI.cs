using System;
using KZLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;

public class VideoPanelUI : WindowUI2D
{
	public record VideoParam(string VideoPath,SubtitlePanelUI.SubtitleParam SubtitleParam,bool UseSkip,bool IsLoop);

	[SerializeField] private RawImage m_Screen = null;
	[SerializeField] private VideoPlayer m_VideoPlayer = null;
	[SerializeField] private AspectRatioFitter m_Aspect = null;

	public override UITag Tag => UITag.VideoPanelUI;

	public TimeSpan Duration => TimeSpan.FromSeconds(m_VideoPlayer.frameCount/m_VideoPlayer.frameRate);

	public double Time => m_VideoPlayer.frame/m_VideoPlayer.frameRate;

	public bool IsPlaying => m_VideoPlayer != null && m_VideoPlayer.isPlaying;
	public bool IsPrepared => m_VideoPlayer != null && m_VideoPlayer.isPrepared;

	protected override void Initialize()
	{
		base.Initialize();

		m_VideoPlayer.errorReceived += (player,message) => { throw new Exception(message); };

		m_VideoPlayer.loopPointReached += (player) =>
		{
			if(!m_VideoPlayer.isLooping)
			{
				Stop();
			}
		};
	}

	public override void Open(object _param)
	{
		base.Open(_param);

		if(_param is not VideoParam param)
		{
			return;
		}

		m_Screen.color = Color.black;
		m_VideoPlayer.targetCamera = m_canvas.worldCamera;

		m_VideoPlayer.isLooping = param.IsLoop;

		if(Uri.IsWellFormedUriString(param.VideoPath,UriKind.Absolute))
		{
			m_VideoPlayer.source = VideoSource.Url;
			m_VideoPlayer.url = param.VideoPath;
		}
		else
		{
			m_VideoPlayer.source = VideoSource.VideoClip;

			var clip = ResMgr.In.GetVideoClip(param.VideoPath);

			if(!clip)
			{
				LogTag.System.E($"Video path is wrong. [{param.VideoPath}]");

				return;
			}

			m_VideoPlayer.clip = clip;
		}

		if(param.SubtitleParam != null)
		{
			var panel = UIMgr.In.Open<SubtitlePanelUI>(UITag.SubtitlePanelUI,param.SubtitleParam);

			AddLink(panel);

			panel.SetVideoPanelUI(this);
		}

		if(param.UseSkip)
		{
			var panel = UIMgr.In.Open<SkipPanelUI>(UITag.SkipPanelUI,new SkipPanelUI.SkipParam(Stop));

			AddLink(panel);
		}

		m_VideoPlayer.Prepare();
	}

	public override void Close()
	{
		base.Close();

		m_VideoPlayer.clip = null;
		m_Screen.color = Color.black;
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
		var texture = m_VideoPlayer.texture;

		m_Screen.color = Color.white;
		m_Screen.texture = texture;

		m_Aspect.aspectRatio = texture.width/(float)texture.height;

		m_VideoPlayer.Play();
	}

	public void Stop()
	{
		m_VideoPlayer.Stop();
	}

	public void JumpTime(TimeSpan _time)
	{
		JumpFrame((long) (_time.Seconds*m_VideoPlayer.frameRate));
	}
	
	public void JumpPosition(float _percent)
	{
		JumpFrame((long) (_percent*m_VideoPlayer.frameCount));
	}

	public void JumpFrame(long _frame)
	{
		if(!m_VideoPlayer.texture || _frame == 0)
		{
			return;
		}

		SeekFrame(m_VideoPlayer.frame+_frame);
	}

	public void SeekTimeSpan(TimeSpan _time)
	{
		SeekFrame((long) (_time.TotalSeconds * m_VideoPlayer.frameRate));
	}

	public void SeekPosition(float _percent)
	{
		SeekFrame((long) (_percent*m_VideoPlayer.frameCount));
	}

	public void SeekFrame(long _frame)
	{
		m_VideoPlayer.frame = CommonUtility.Clamp(_frame,0L,(long)m_VideoPlayer.frameCount);
	}
}