using System;
using System.Collections.Generic;
using KZLib.KZNetwork;
using MessagePipe;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;

namespace HudPanel
{
	public class LogViewUI : BaseComponentUI
	{
		#region HudLog Info
		[Serializable]
		private record HudLogInfo
		{
			[HorizontalGroup("1",Order = 1),SerializeField,HideLabel]
			private Color m_color;

			[HorizontalGroup("0",Order = 0),SerializeField]
			private Toggle m_toggle;

			[HorizontalGroup("0",Order = 0),SerializeField]
			private TMP_Text m_countText;

			[SerializeField,HideInInspector]
			private int m_count;

			public Color Color => m_color;
			public bool IsToggleOn => m_toggle.isOn;

			public void AddCount()
			{
				m_count++;

				_SetCountText();
			}

			public void ResetCount()
			{
				m_count = 0;
			}

			public void Initialize(Action onAction)
			{
				m_toggle.isOn = true;

				ResetCount();

				_SetCountText();

				void _ChangeValue(bool _)
				{
					onAction?.Invoke();
				}

				m_toggle.onValueChanged.SetAction(_ChangeValue);
			}

			private void _SetCountText()
			{
				m_countText.SetSafeTextMeshPro(m_count < 100 ? $"{m_count}" : "99+");
			}
		}
		#endregion HudLog Info

		private enum HudLogType { Info, Warning, Error }

		[VerticalGroup("0",Order = 0),SerializeField,DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
		private Dictionary<HudLogType,HudLogInfo> m_hudLogInfoDict = new();

		[VerticalGroup("1",Order = 1),SerializeField]
		private ReuseScrollRectUI m_scrollRectUI = null;
		[VerticalGroup("1",Order = 1),SerializeField]
		private TMP_InputField m_inputField = null;

		private string m_compareText = null;

		private IDisposable m_subscription = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_inputField.text = string.Empty;

			foreach(var pair in m_hudLogInfoDict)
			{
				pair.Value.Initialize(_SetScrollRect);
			}

			void _ChangeValue(string text)
			{
				m_compareText = text.IsEmpty() ? null : text;

				_SetScrollRect();
			}

			m_inputField.onValueChanged.SetAction(_ChangeValue);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			void _RefreshLogScroll(MessageInfo messageInfo)
			{
				var entryInfo = _CreateLogEntryInfo(messageInfo);

				if(entryInfo != null)
				{
					m_scrollRectUI.AddEntry(entryInfo);
				}
			}

			m_subscription = GlobalMessagePipe.GetSubscriber<CommonNoticeTag,MessageInfo>().Subscribe(CommonNoticeTag.DisplayLog,_RefreshLogScroll);

			_SetScrollRect();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_subscription?.Dispose();
		}

		private void _SetScrollRect()
		{
			var entryInfoList = new List<IEntryInfo>();

			foreach(var pair in m_hudLogInfoDict)
			{
				pair.Value.ResetCount();
			}

			foreach(var message in LogChannel.LogMessageInfoGroup)
			{
				var entryInfo = _CreateLogEntryInfo(message);

				if(entryInfo != null)
				{
					entryInfoList.Add(entryInfo);
				}
			}

			m_scrollRectUI.SetEntryInfoList(entryInfoList,0);
		}

		private LogEntryInfo _CreateLogEntryInfo(MessageInfo messageInfo)
		{
			var headerInfo = _SplitHeader(messageInfo.Header);

			if(headerInfo != null)
			{
				var logInfo = m_hudLogInfoDict[headerInfo.LogType];

				logInfo.AddCount();

				if(logInfo.IsToggleOn && _IsContainsText(messageInfo.Body))
				{
					void _ClickSlot(IEntryInfo _)
					{
						WebRequestManager.In.PostDiscordWebHook("Log Window",new MessageInfo[] { messageInfo });
					}

					return new LogEntryInfo(logInfo.Color,headerInfo.Time,messageInfo.Body,_ClickSlot);
				}
			}

			return null;
		}

		private bool _IsContainsText(string message)
		{
			return m_compareText.IsEmpty() || message.Contains(m_compareText,StringComparison.OrdinalIgnoreCase);
		}

		private HeaderInfo _SplitHeader(string header)
		{
			var headerArray = header.Split(' ',2);

			if(headerArray.Length == 2)
			{
				var type = headerArray[0].TrimAngleBrackets().ToEnum<HudLogType>();
				var time = headerArray[1];

				return new HeaderInfo(type,time);
			}

			return null;
		}

		private record HeaderInfo(HudLogType LogType,string Time);
	}
}