using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;

namespace KZLib.UI
{
	/// <summary>
	/// Parses SRT-style subtitle files and displays cues synchronized with <see cref="VideoPanel"/> playback time.
	/// </summary>
	public class SubtitlePanel : BasePanel
	{
		public record Param(string SubtitlePath);

		#region Subtitle Info
		private record SubtitleInfo
		{
			private readonly float m_startTime = 0.0f;
			private readonly float m_finishTime = 0.0f;

			public string Text { get; }

			public SubtitleInfo(float startTime,float finishTime,string text)
			{
				m_startTime = startTime;
				m_finishTime = finishTime;

				Text = text;
			}

			public bool IsIncludeTime(float time)
			{
				// Inclusive on both ends; adjacent cues sharing a boundary resolve to the earlier entry via FindIndex.
				return m_startTime <= time && time <= m_finishTime;
			}
		}
		#endregion Subtitle Info

		[SerializeField,Required]
		private TMP_Text m_subtitleText = null;

		private readonly List<SubtitleInfo> m_subtitleList = new();

		private IDisposable m_subscription = null;

		public override void Open(object param)
		{
			if(!_TryGetOpenParam(param,out Param subtitleParam))
			{
				return;
			}

			var subtitlePath = subtitleParam.SubtitlePath;

			if(subtitlePath.IsEmpty())
			{
				_FailOpen("Subtitle path is empty.");

				return;
			}

			var textAsset = ResourceManager.In.GetTextAsset(subtitlePath);

			if(!textAsset)
			{
				_FailOpen($"Subtitle path is wrong. [{subtitlePath}]");

				return;
			}

			var subtitleText = textAsset.text;

			if(subtitleText.IsEmpty())
			{
				_FailOpen("Subtitle is empty.");

				return;
			}

			m_subtitleList.Clear();

			if(!_TryParseSubtitleText(subtitleText,out var parseError))
			{
				_FailOpen(parseError);

				return;
			}

			base.Open(param);
		}

		public override void Close()
		{
			base.Close();

			m_subscription?.Dispose();
			m_subscription = null;

			m_subtitleList.Clear();

			if(m_subtitleText)
			{
				m_subtitleText.SetSafeTextMeshPro(string.Empty);
			}
		}

		/// <summary>
		/// Subscribes to <paramref name="videoPanel"/>.<see cref="VideoPanel.OnChangedVideoTime"/>.
		/// Must be called after <see cref="Open"/>; typically invoked by <see cref="VideoPanel.PrepareVideoAsync"/>.
		/// </summary>
		public void LinkVideo(VideoPanel videoPanel)
		{
			if(!videoPanel)
			{
				return;
			}

			m_subscription?.Dispose();
			m_subscription = videoPanel.OnChangedVideoTime.Subscribe(_SetSubtitle);

			// Reflect current playback position immediately (e.g. after seek while paused).
			_SetSubtitle(videoPanel.Time);
		}

		/// <summary>
		/// Parses a subset of SRT: optional index line, <c>HH:MM:SS,mmm --&gt; HH:MM:SS,mmm</c> timing, then one or more text lines.
		/// </summary>
		private bool _TryParseSubtitleText(string subtitleText,out string errorMessage)
		{
			var lines = subtitleText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			var builder = new StringBuilder();

			for(var lineIndex = 0;lineIndex < lines.Length;)
			{
				while(lineIndex < lines.Length && lines[lineIndex].IsEmpty())
				{
					lineIndex++;
				}

				if(lineIndex >= lines.Length)
				{
					break;
				}

				// Optional cue index line (e.g. "1").
				if(int.TryParse(lines[lineIndex].Trim(),out _))
				{
					lineIndex++;

					if(lineIndex >= lines.Length)
					{
						break;
					}
				}

				var timingLine = lines[lineIndex];

				if(!timingLine.Contains("-->"))
				{
					errorMessage = $"Subtitle timing line is invalid. [{timingLine}]";

					return false;
				}

				// SRT uses comma for milliseconds; TimeSpan.TryParse expects a dot.
				var timeArray = timingLine.Replace(',', '.').Split(new[] { "-->" },StringSplitOptions.RemoveEmptyEntries);

				if(timeArray.Length < 2 || !TimeSpan.TryParse(timeArray[0].Trim(),out var startTime) || !TimeSpan.TryParse(timeArray[1].Trim(),out var finishTime))
				{
					errorMessage = $"Subtitle timing line is invalid. [{timingLine}]";

					return false;
				}

				var startSeconds = (float)startTime.TotalSeconds;
				var finishSeconds = (float)finishTime.TotalSeconds;

				if(startSeconds > finishSeconds)
				{
					errorMessage = $"Subtitle start time is after finish time. [{timingLine}]";

					return false;
				}

				lineIndex++;

				builder.Clear();

				while(lineIndex < lines.Length && !lines[lineIndex].IsEmpty())
				{
					if(builder.Length > 0)
					{
						builder.Append('\n');
					}

					builder.Append(lines[lineIndex]);
					lineIndex++;
				}

				if(builder.Length == 0)
				{
					continue;
				}

				m_subtitleList.Add(new SubtitleInfo(startSeconds,finishSeconds,builder.ToString()));
			}

			if(m_subtitleList.IsNullOrEmpty())
			{
				errorMessage = "Subtitle has no valid entries.";

				return false;
			}

			errorMessage = null;

			return true;
		}

		/// <summary>
		/// Updates displayed cue for <paramref name="time"/> in seconds.
		/// A negative value clears the subtitle (sent by <see cref="VideoPanel.Stop"/>).
		/// </summary>
		private void _SetSubtitle(float time)
		{
			if(time < 0.0f)
			{
				// Clear signal from VideoPanel.Stop — not a playback timestamp.
				m_subtitleText.SetSafeTextMeshPro(string.Empty);

				return;
			}

			if(m_subtitleList.Count == 0)
			{
				return;
			}

			var index = m_subtitleList.FindIndex(subtitleInfo => subtitleInfo.IsIncludeTime(time));

			if(index == Global.InvalidIndex)
			{
				m_subtitleText.SetSafeTextMeshPro(string.Empty);

				return;
			}

			m_subtitleText.SetSafeTextMeshPro(m_subtitleList[index].Text);
		}
	}
}