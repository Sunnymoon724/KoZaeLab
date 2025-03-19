using System;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HudPanel
{
	public class LogViewUI : BaseComponentUI
	{
		#region HudLogData
		[Serializable]
		private struct HudLogData
		{
			[HorizontalGroup("1",Order = 1),SerializeField,HideLabel]
			private Color m_color;

			[HorizontalGroup("0",Order = 0),SerializeField]
			private Toggle m_toggle;

			[HorizontalGroup("0",Order = 0),SerializeField]
			private TMP_Text m_text;

			private int m_count;

			public Color Color => m_color;
			public bool IsToggleOn => m_toggle.isOn;

			public void AddCount()
			{
				m_count++;

				m_text.SetSafeTextMeshPro(CheckCount(m_count));
			}

			public void ResetCount()
			{
				m_count = 0;
			}

			public void Initialize(Action onAction)
			{
				m_toggle.isOn = true;

				ResetCount();

				m_text.SetSafeTextMeshPro(CheckCount(m_count));

				m_toggle.onValueChanged.AddAction((dummy)=> { onAction?.Invoke(); });
			}

			private string CheckCount(int count) => count < 100 ? $"{count}" : "99+";
		}
		#endregion MsgData

		private enum HudLogType { Info, Warning, Error }

		[VerticalGroup("0",Order = 0),SerializeField,DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		private Dictionary<HudLogType,HudLogData> m_hudLogDataDict = new();

		[VerticalGroup("1",Order = 1),SerializeField]
		private ReuseScrollRectUI m_scrollRectUI = null;
		[VerticalGroup("1",Order = 1),SerializeField]
		private TMP_InputField m_inputField = null;

		private string m_compareText = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_inputField.text = string.Empty;

			foreach(var data in m_hudLogDataDict.Values)
			{
				data.Initialize(SetScrollRect);
			}

			m_inputField.onValueChanged.SetAction((text)=>
			{
				m_compareText = text.IsEmpty() ? null : text;

				SetScrollRect();
			});
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			LogMgr.In.OnLogDisplay += OnUpdateLogScroll;

			SetScrollRect();
		}

		protected override void OnDisable()
		{
			base.OnEnable();

			LogMgr.In.OnLogDisplay -= OnUpdateLogScroll;
		}

		private void SetScrollRect()
		{
			var cellDataList = new List<ICellData>();

			foreach(var cellData in m_hudLogDataDict.Values)
			{
				cellData.ResetCount();
			}

			foreach(var message in LogMgr.In.LogDataGroup)
			{
				var cellData = CreateLogCellData(message);

				if(cellData != null)
				{
					cellDataList.Add(cellData);
				}
			}

			m_scrollRectUI.SetCellList(cellDataList,0);
		}

		private void OnUpdateLogScroll(MessageData messageData)
		{
			var cellData = CreateLogCellData(messageData);

			if(cellData != null)
			{
				m_scrollRectUI.AddCell(cellData);
			}
		}

		private LogCellData CreateLogCellData(MessageData messageData)
		{
			var (Type,Time) = SplitHeader(messageData.Header);

			if(Type.HasValue)
			{
				var logData = m_hudLogDataDict[Type.Value];

				logData.AddCount();

				if(logData.IsToggleOn && IsContainsText(messageData.Body))
				{
					return new LogCellData(logData.Color,Time,messageData.Body,() =>
					{
						NetworkMgr.In.PostDiscordWebHook("Log Window",new MessageData[] { messageData });
					});
				}
			}

			return null;
		}

		private bool IsContainsText(string message)
		{
			return m_compareText.IsEmpty() || message.Contains(m_compareText,StringComparison.OrdinalIgnoreCase);
		}

		private (HudLogType? Type,string Time) SplitHeader(string header)
		{
			var headerArray = header.Split(' ',2);

			if(headerArray.Length == 2)
			{
				var type = headerArray[0].TrimAngleBrackets().ToEnum<HudLogType>();
				var time = headerArray[1];

				return (type,time);
			}

			return (null,null);
		}
	}
}