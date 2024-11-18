using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib;
using TMPro;
using UnityEngine;

public class SubtitlePanelUI : WindowUI2D
{
	public record SubtitleParam(string SubtitlePath,TMP_FontAsset FontAsset);

	#region Subtitle Data
	private record SubtitleData
	{
		private readonly double m_StartTime;
		private readonly double m_FinishTime;

		public double Length => m_FinishTime-m_StartTime;

		public string Text { get; }

		public SubtitleData(double _start,double _finish,string _text)
		{
			m_StartTime = _start;
			m_FinishTime = _finish;

			Text = _text;
		}

		public bool IsIncludeTime(double _time)
		{
			return m_StartTime <= _time && _time <= m_FinishTime;
		}
	}
	#endregion Subtitle Data

	[SerializeField] private TMP_Text m_SubtitleText = null;

	public override UITag Tag => UITag.SubtitlePanelUI;

	private readonly List<SubtitleData> m_SubtitleList = new();

	private VideoPanelUI m_VideoPanelUI = null;

	private CancellationTokenSource m_TokenSource = null;

	public override void Open(object _param)
	{
		base.Open(_param);

		if(_param is not SubtitleParam param)
		{
			return;
		}

		if(param.SubtitlePath.IsEmpty())
		{
			var textAsset = ResMgr.In.GetTextAsset(param.SubtitlePath);

			if(!textAsset)
			{
				LogTag.System.E($"Subtitle path is wrong. [{param.SubtitlePath}]");

				return;
			}

			var subtitle = textAsset.text;

			if(subtitle.IsEmpty())
			{
				LogTag.System.E($"Subtitle is empty");

				return;
			}

			m_SubtitleList.Clear();

			var builder = new StringBuilder();
			var textArray = subtitle.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			var pivot = 0;

			while(pivot <= textArray.Length)
			{
				var timeArray = textArray[pivot++].Replace(',', '.').Split(new[] { "-->" },StringSplitOptions.RemoveEmptyEntries);
				var start = TimeSpan.Parse(timeArray[0]);
				var finish = TimeSpan.Parse(timeArray[1]);

				builder.Clear();

				while(true)
				{
					if(textArray[pivot].IsEmpty() || pivot == textArray.Length-1)
					{
						break;
					}

					builder.AppendFormat("{0}\n",textArray[pivot++]);
				}

				m_SubtitleList.Add(new SubtitleData(start.TotalSeconds,finish.TotalSeconds,builder.ToString()));
			}

			if(m_SubtitleList.IsNullOrEmpty())
			{
				gameObject.SetActiveSelf(false);

				return;
			}
		}

		if(param.FontAsset)
		{
			m_SubtitleText.font = param.FontAsset;
		}
	}

	public override void Close()
	{
		base.Close();

		CommonUtility.KillTokenSource(ref m_TokenSource);
	}

	public void SetVideoPanelUI(VideoPanelUI _videoPanel)
	{
		m_VideoPanelUI = _videoPanel;

		CommonUtility.RecycleTokenSource(ref m_TokenSource);

		UpdateSubTitleAsync().Forget();
	}

	private async UniTask UpdateSubTitleAsync()
	{
		while(true)
		{
			if(m_VideoPanelUI.IsPlaying)
			{
				SetSubtitle(m_VideoPanelUI.Time);
			}

			await UniTask.Yield();
		}
	}

	private void SetSubtitle(double _time)
	{
		if(_time < 0.0d || m_SubtitleList.Count == 0)
		{
			return;
		}

		var index = m_SubtitleList.FindIndex(x=>x.IsIncludeTime(_time));

		if(index == -1)
		{
			return;
		}

		m_SubtitleText.SetSafeTextMeshPro(m_SubtitleList[index].Text);
	}
}