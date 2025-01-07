using System;

namespace VideoPanel
{
	public record VideoData
	{
		public string VideoPath { get; }
		public string SubtitlePath { get; }
		public bool CanSkip { get; }
		public bool IsLoop { get; }
		public bool IsUrl { get; }

		public VideoData(string videoPath,string subtitlePath,bool canSkip,bool isLoop)
		{
			IsUrl = Uri.IsWellFormedUriString(videoPath,UriKind.Absolute);
			VideoPath = videoPath;

			SubtitlePath = subtitlePath;

			CanSkip = canSkip;
			IsLoop = isLoop;
		}

		public VideoData(string videoPath,bool canSkip,bool isLoop) : this(videoPath,null,canSkip,isLoop) { }

		public bool IsExistSubtitle => !SubtitlePath.IsEmpty();
	}
}