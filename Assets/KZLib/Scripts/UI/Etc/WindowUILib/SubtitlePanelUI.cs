using System;
using System.Collections.Generic;
using System.Text;
using KZLib;
using TMPro;
using UnityEngine;
using R3;

public class SubtitlePanelUI : WindowUI2D
{
	public record Param(string SubtitlePath);

	#region Subtitle Info
	private record SubtitleInfo
	{
		private readonly float m_startTime = 0.0f;
		private readonly float m_finishTime = 0.0f;

		public float Length => m_finishTime-m_startTime;
		public string Text { get; }

		public SubtitleInfo(float startTime,float finishTime,string text)
		{
			m_startTime = startTime;
			m_finishTime = finishTime;

			Text = text;
		}

		public bool IsIncludeTime(float time)
		{
			return m_startTime <= time && time <= m_finishTime;
		}
	}
	#endregion Subtitle Info

	[SerializeField] private TMP_Text m_subtitleText = null;

	private readonly List<SubtitleInfo> m_subtitleList = new();

	public override void Open(object param)
	{
		base.Open(param);

		if(param is not Param subtitleParam)
		{
			return;
		}

		var subtitlePath = subtitleParam.SubtitlePath;

		if(subtitlePath.IsEmpty())
		{
			var textAsset = ResourceManager.In.GetTextAsset(subtitlePath);

			if(!textAsset)
			{
				LogSvc.System.E($"Subtitle path is wrong. [{subtitlePath}]");

				return;
			}

			var subtitleText = textAsset.text;

			if(subtitleText.IsEmpty())
			{
				LogSvc.System.E($"Subtitle is empty");

				return;
			}

			m_subtitleList.Clear();

			var builder = new StringBuilder();
			var textArray = subtitleText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			var pivot = 0;

			while(pivot <= textArray.Length)
			{
				var timeArray = textArray[pivot++].Replace(',', '.').Split(new[] { "-->" },StringSplitOptions.RemoveEmptyEntries);
				var startTime = TimeSpan.Parse(timeArray[0]);
				var finishTime = TimeSpan.Parse(timeArray[1]);

				builder.Clear();

				while(true)
				{
					if(textArray[pivot].IsEmpty() || pivot == textArray.Length-1)
					{
						break;
					}

					builder.AppendFormat("{0}\n",textArray[pivot++]);
				}

				m_subtitleList.Add(new SubtitleInfo((float)startTime.TotalSeconds,(float)finishTime.TotalSeconds,builder.ToString()));
			}

			if(m_subtitleList.IsNullOrEmpty())
			{
				gameObject.EnsureActive(false);

				return;
			}
		}
	}

	public void LinkVideo(VideoPanelUI videoPanel)
	{
		videoPanel.OnChangedVideoTime.Subscribe(_SetSubtitle).RegisterTo(destroyCancellationToken);
	}

	private void _SetSubtitle(float time)
	{
		if(time < 0.0f || m_subtitleList.Count == 0)
		{
			return;
		}

		bool _FindIndex(SubtitleInfo subtitleInfo)
		{
			return subtitleInfo.IsIncludeTime(time);
		}

		var index = m_subtitleList.FindIndex(_FindIndex);

		if(index == Global.INVALID_INDEX)
		{
			return;
		}

		m_subtitleText.SetSafeTextMeshPro(m_subtitleList[index].Text);
	}
}