using System;
using System.Collections.Generic;
using System.Text;
using KZLib;
using TMPro;
using UnityEngine;

namespace VideoPanel
{
	public class SubtitleUI : BaseComponentUI
	{
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

		private readonly List<SubtitleData> m_SubtitleList = new();

		public void SetPath(string _videoPath)
		{
			var dataPath = CommonUtility.ChangeExtension(_videoPath,".txt");
			var textAsset = ResMgr.In.GetTextAsset(dataPath);

			if(!textAsset)
			{
				throw new NullReferenceException(string.Format("자막 경로가 잘못 되어 있습니다. [경로 : {0}]",dataPath));
			}

			var subtitle = textAsset.text;

			if(subtitle.IsEmpty())
			{
				throw new NullReferenceException("자막이 없습니다.");
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

		public void SetSubtitle(double _time)
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
}