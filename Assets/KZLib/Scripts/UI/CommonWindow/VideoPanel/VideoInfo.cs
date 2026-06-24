using System;

namespace KZLib.UI
{
	/// <summary>
	/// Playback descriptor passed to <see cref="KZLib.UI.VideoPanel.PrepareVideoAsync"/> via <see cref="KZLib.UIManager.WatchVideoAsync"/>.
	/// </summary>
	public record VideoInfo
	{
		/// <summary>Resource path or absolute URL, depending on <see cref="IsUrl"/>.</summary>
		public string VideoPath { get; }

		/// <summary>Optional subtitle resource path; empty when <see cref="IsExistSubtitle"/> is <c>false</c>.</summary>
		public string SubtitlePath { get; }

		/// <summary>When <c>true</c>, <see cref="KZLib.UI.VideoPanel"/> tries to open <see cref="KZLib.UI.SkipPanel"/>.</summary>
		public bool CanSkip { get; }

		/// <summary>Passed to <see cref="UnityEngine.Video.VideoPlayer.isLooping"/>.</summary>
		public bool IsLoop { get; }

		/// <summary><c>true</c> when <see cref="VideoPath"/> is an absolute URI; otherwise treated as a project video clip path.</summary>
		public bool IsUrl { get; }

		public VideoInfo(string videoPath,string subtitlePath,bool canSkip,bool isLoop)
		{
			IsUrl = Uri.IsWellFormedUriString(videoPath,UriKind.Absolute);
			VideoPath = videoPath;

			SubtitlePath = subtitlePath;

			CanSkip = canSkip;
			IsLoop = isLoop;
		}

		/// <summary>Creates video info without a subtitle path.</summary>
		public VideoInfo(string videoPath,bool canSkip,bool isLoop) : this(videoPath,null,canSkip,isLoop) { }

		public bool IsExistSubtitle => !SubtitlePath.IsEmpty();
	}
}