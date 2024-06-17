using System;
using System.Collections.Generic;
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

		protected override void Awake()
		{
			base.Awake();

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

		private void OnEnable()
		{
			LogMgr.In.OnAddLog += OnUpdateLogScroll;

			SetScrollRect();
		}

		private void OnDisable()
		{
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
				if(CheckLogData(data,out var color))
				{
					cellList.Add(CreateCellData(data,color));
				}
			}

			m_ScrollRect.SetCellList(cellList,0);

			foreach(var data in m_MsgDataDict.Values)
			{
				data.SetCount();
			}
		}

		private void OnUpdateLogScroll(LogData _data)
		{
			if(CheckLogData(_data,out var color))
			{
				m_ScrollRect.AddCell(CreateCellData(_data,color));
			}

			foreach(var data in m_MsgDataDict.Values)
			{
				data.SetCount();
			}
		}

		private bool CheckLogData(LogData _data,out Color _color)
		{
			var type = GetMsgType(_data.DataType);

			if(m_MsgDataDict.TryGetValue(type,out var data))
			{
				_color = data.MsgColor;

				return data.AddCount() && (m_CompareText.IsEmpty() || _data.Body.Contains(m_CompareText,StringComparison.OrdinalIgnoreCase));
			}

			_color = Color.white;

			return false;
		}

		private LogCellData CreateCellData(LogData _data,Color _color)
		{
			return new LogCellData(_color,_data.Time,_data.Body,() =>
			{
				CommonUtility.SendReportOnlyDiscord("Log Window",new MessageData[] { _data },null);
			});
		}

		private MsgType GetMsgType(LogType _logType)
		{
			return _logType switch
			{
				LogType.Warning => MsgType.Warning,
				LogType.Error or LogType.Exception => MsgType.Error,
				_ => MsgType.Info,
			};
		}
	}
}