using System;

namespace KZLib.KZData.Video
{
	public record VideoInfo
	{
		public string VideoPath { get; }
		public string SubtitlePath { get; }
		public bool CanSkip { get; }
		public bool IsLoop { get; }
		public bool IsUrl { get; }

		public VideoInfo(string videoPath,string subtitlePath,bool canSkip,bool isLoop)
		{
			IsUrl = Uri.IsWellFormedUriString(videoPath,UriKind.Absolute);
			VideoPath = videoPath;

			SubtitlePath = subtitlePath;

			CanSkip = canSkip;
			IsLoop = isLoop;
		}

		public VideoInfo(string videoPath,bool canSkip,bool isLoop) : this(videoPath,null,canSkip,isLoop) { }

		public bool IsExistSubtitle => !SubtitlePath.IsEmpty();
	}
}