using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KZLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class LogWindow : BaseComponentUI
	{
		#region MsgData
		[Serializable]
		private class MsgData
		{
			[HorizontalGroup("1",Order = 1),SerializeField,HideLabel]
			private Color m_MsgColor;

			[HorizontalGroup("0",Order = 0),SerializeField,LabelText("토글")]
			private GraphicToggleUI m_MsgToggle;

			[HorizontalGroup("0",Order = 0),SerializeField,LabelText("텍스트")]
			private TMP_Text m_MsgText;

			private int m_MsgCount;

			public Color MsgColor => m_MsgColor;

			public bool AddCount()
			{
				m_MsgCount++;

				return m_MsgToggle.IsOn;
			}

			public void ResetCount()
			{
				m_MsgCount = 0;
			}

			public void SetCount()
			{
				m_MsgText.SetSafeTextMeshPro(CheckCount(m_MsgCount));
			}

			public void Initialize(Action _onChanged)
			{
				m_MsgToggle.SetToggle(true,true);

				m_MsgToggle.OnChanged += (toggle) =>
				{
					_onChanged();
				};

				ResetCount();
			}

			private string CheckCount(int _count) => _count < 100 ? string.Format("{0}", _count) : "99+";
		}
		#endregion MsgData

		private enum MsgType { Info,Warning,Error }

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("메세지 타입 딕셔너리"),DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		private Dictionary<MsgType,MsgData> m_MsgDataDict = new();

		[VerticalGroup("1",Order = 1),SerializeField]
		private ScrollRectUI m_ScrollRect = null;
		[VerticalGroup("1",Order = 1),SerializeField]
		private TMP_InputField m_InputField = null;

		private string m_CompareText = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_InputField.text = string.Empty;

			foreach(var data in m_MsgDataDict.Values)
			{
				data.Initialize(SetScrollRect);
			}

			m_InputField.onValueChanged.AddListener((text)=>
			{
				m_CompareText = text.IsEmpty() ? null : text;

				SetScrollRect();
			});
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			LogMgr.In.OnAddLog += OnUpdateLogScroll;

			SetScrollRect();
		}

		protected override void OnDisable()
		{
			base.OnEnable();

			LogMgr.In.OnAddLog -= OnUpdateLogScroll;
		}

		private void SetScrollRect()
		{
			var cellList = new List<ICellData>();

			foreach(var data in m_MsgDataDict.Values)
			{
				data.ResetCount();
			}

			foreach(var data in LogMgr.In.LogDataGroup)
			{
				var log = GetLogData(data);

				if(log.Item2 != null)
				{
					cellList.Add(CreateCellData(data,log.Item1,log.Item2));
				}
			}

			m_ScrollRect.SetCellList(cellList,0);

			foreach(var data in m_MsgDataDict.Values)
			{
				data.SetCount();
			}
		}

		private void OnUpdateLogScroll(MessageData _data)
		{
			var log = GetLogData(_data);

			if(log.Item2 != null)
			{
				m_ScrollRect.AddCell(CreateCellData(_data,log.Item1,log.Item2));
			}

			foreach(var data in m_MsgDataDict.Values)
			{
				data.SetCount();
			}
		}

		private (Color,string) GetLogData(MessageData _data)
		{
			var head = SplitHead(_data.Head);

			if(head.Type.HasValue)
			{
				var data = m_MsgDataDict[head.Type.Value];

				if(data.AddCount() && (m_CompareText.IsEmpty() || _data.Body.Contains(m_CompareText,StringComparison.OrdinalIgnoreCase)))
				{
					return (data.MsgColor,_data.Body);
				}
			}

			return (Color.white,null);
		}

		private LogCellData CreateCellData(MessageData _data,Color _color,string _time)
		{
			return new LogCellData(_color,_time,_data.Body,() =>
			{
				CommonUtility.SendReportOnlyDiscord("Log Window",new MessageData[] { _data },null);
			});
		}

		private (MsgType? Type,string Time) SplitHead(string _head)
		{
			var match = Regex.Match(_head,@"<(.+?)> \[(.+?)\]");

			if(match.Success)
			{
				var type = match.Groups[1].Value;
				var time = match.Groups[2].Value;

				return (type.ToEnum<MsgType>(),time);
			}

			return (null,null);
		}
	}
}