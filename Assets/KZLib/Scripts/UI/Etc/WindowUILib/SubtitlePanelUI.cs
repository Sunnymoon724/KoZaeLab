using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KZLib;
using TMPro;
using UnityEngine;

public class SubtitlePanelUI : WindowUI2D
{
	public record SubtitleParam(string SubtitlePath);

	#region Subtitle Data
	private record SubtitleData
	{
		private readonly float m_startTime = 0.0f;
		private readonly float m_finishTime = 0.0f;

		public float Length => m_finishTime-m_startTime;
		public string Text { get; }

		public SubtitleData(float startTime,float finishTime,string text)
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
	#endregion Subtitle Data

	[SerializeField] private TMP_Text m_subtitleText = null;

	public override UITag Tag => UITag.SubtitlePanelUI;

	private readonly List<SubtitleData> m_subtitleList = new();

	public override void Open(object param)
	{
		base.Open(param);

		if(param is not SubtitleParam subtitleParam)
		{
			return;
		}

		var subtitlePath = subtitleParam.SubtitlePath;

		if(subtitlePath.IsEmpty())
		{
			var textAsset = ResourceManager.In.GetTextAsset(subtitlePath);

			if(!textAsset)
			{
				LogTag.System.E($"Subtitle path is wrong. [{subtitlePath}]");

				return;
			}

			var subtitleText = textAsset.text;

			if(subtitleText.IsEmpty())
			{
				LogTag.System.E($"Subtitle is empty");

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

				m_subtitleList.Add(new SubtitleData((float)startTime.TotalSeconds,(float)finishTime.TotalSeconds,builder.ToString()));
			}

			if(m_subtitleList.IsNullOrEmpty())
			{
				gameObject.SetActiveIfDifferent(false);

				return;
			}
		}
	}

	public void SetSubtitle(float time)
	{
		if(time < 0.0f || m_subtitleList.Count == 0)
		{
			return;
		}

		var index = m_subtitleList.FindIndex(x=>x.IsIncludeTime(time));

		if(index == -1)
		{
			return;
		}

		m_subtitleText.SetSafeTextMeshPro(m_subtitleList[index].Text);
	}
}