using System;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HudPanel
{
	public class LogWindow : BaseComponentUI
	{
		#region HudLogData
		[Serializable]
		private class HudLogData
		{
			[HorizontalGroup("1",Order = 1),SerializeField,HideLabel]
			private Color m_LogColor = Color.white;

			[HorizontalGroup("0",Order = 0),SerializeField,LabelText("Toggle")]
			private Toggle m_LogToggle = null;

			[HorizontalGroup("0",Order = 0),SerializeField,LabelText("Text")]
			private TMP_Text m_LogText = null;

			private int m_LogCount = 0;

			public Color LogColor => m_LogColor;

			public bool AddCount()
			{
				m_LogCount++;

				m_LogText.SetSafeTextMeshPro(CheckCount(m_LogCount));

				return m_LogToggle.isOn;
			}

			public void ResetCount()
			{
				m_LogCount = 0;
			}

			public void Initialize(Action _onAction)
			{
				m_LogToggle.isOn = true;

				ResetCount();

				m_LogText.SetSafeTextMeshPro(CheckCount(m_LogCount));

				m_LogToggle.onValueChanged.AddAction((dummy)=> { _onAction?.Invoke(); });
			}

			private string CheckCount(int _count) => _count < 100 ? $"{_count}" : "99+";
		}
		#endregion MsgData

		private enum HudLogType { Info, Warning, Error }

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("Log Data Group"),DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		private Dictionary<HudLogType,HudLogData> m_HudLogDataDict = new();

		[VerticalGroup("1",Order = 1),SerializeField]
		private ScrollRectUI m_ScrollRect = null;
		[VerticalGroup("1",Order = 1),SerializeField]
		private TMP_InputField m_InputField = null;

		private string m_CompareText = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_InputField.text = string.Empty;

			foreach(var data in m_HudLogDataDict.Values)
			{
				data.Initialize(SetScrollRect);
			}

			m_InputField.onValueChanged.SetAction((text)=>
			{
				m_CompareText = text.IsEmpty() ? null : text;

				SetScrollRect();
			});
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			LogMgr.In.onAddLog.AddListener(OnUpdateLogScroll);

			SetScrollRect();
		}

		protected override void OnDisable()
		{
			base.OnEnable();

			LogMgr.In.onAddLog.RemoveListener(OnUpdateLogScroll);
		}

		private void SetScrollRect()
		{
			var cellList = new List<ICellData>();

			foreach(var data in m_HudLogDataDict.Values)
			{
				data.ResetCount();
			}

			foreach(var data in LogMgr.In.LogDataGroup)
			{
				var cellData = CreateLogCellData(data);

				if(cellData != null)
				{
					cellList.Add(cellData);
				}
			}

			m_ScrollRect.SetCellList(cellList,0);
		}

		private void OnUpdateLogScroll(MessageData _data)
		{
			var cellData = CreateLogCellData(_data);

			if(cellData != null)
			{
				m_ScrollRect.AddCell(cellData);
			}
		}

		private LogCellData CreateLogCellData(MessageData _data)
		{
			var head = SplitHead(_data.Head);

			if(head.Type.HasValue)
			{
				var data = m_HudLogDataDict[head.Type.Value];

				if(data.AddCount() && (m_CompareText.IsEmpty() || _data.Body.Contains(m_CompareText,StringComparison.OrdinalIgnoreCase)))
				{
					return new LogCellData(data.LogColor,head.Time,_data.Body,() =>
					{
						CommonUtility.PostWebHook_Discord("Log Window",new MessageData[] { _data });
					});
				}
			}

			return null;
		}

		private (HudLogType? Type,string Time) SplitHead(string _head)
		{
			var headArray = _head.Split(' ',2);

			if(headArray.Length == 2)
			{
				var type = headArray[0].TrimAngleBrackets().ToEnum<HudLogType>();
				var time = headArray[1];

				return (type,time);
			}

			return (null,null);
		}
	}
}