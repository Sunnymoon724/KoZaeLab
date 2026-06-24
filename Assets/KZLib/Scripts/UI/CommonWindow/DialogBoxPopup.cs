using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public record DialogEntryInfo(string Name,Action<IEntryInfo> OnClicked) : EntryInfo(Name,OnClicked);

namespace KZLib.UI
{
	/// <summary>
	/// Message dialog with one or more action buttons backed by <see cref="ReuseGridLayoutGroup"/>.
	/// Entry <see cref="DialogEntryInfo.OnClicked"/> runs on button press, then the popup closes.
	/// The optional close button and back dismiss invoke <see cref="Param.OnDismissed"/> when provided.
	/// </summary>
	public class DialogBoxPopup : BasePopup
	{
		/// <summary>
		/// Dialog content and button entries. <see cref="OnDismissed"/> runs on close-button / back dismiss, not on entry clicks.
		/// </summary>
		/// <param name="OnDismissed">Optional; invoked when closed without pressing an entry button.</param>
		public record Param(string Title,string Message,DialogEntryInfo[] EntryInfoArray,Action OnDismissed = null)
		{
			/// <summary>Convenience ctor for callers that only pass button entries via <c>params</c>.</summary>
			public Param(string title,string message,params DialogEntryInfo[] entryInfoArray) : this(title,message,entryInfoArray,null) { }
		}

		[SerializeField]
		private TMP_Text m_titleText = null;

		[SerializeField,Required]
		private TMP_Text m_messageText = null;

		[SerializeField,Required]
		private ReuseGridLayoutGroup m_gridLayout = null;

		private Action m_onDismissed = null;

		/// <summary><c>true</c> after an entry button click so <see cref="Close"/> skips <see cref="m_onDismissed"/>.</summary>
		private bool m_closedViaEntry = false;

		public override void Open(object param)
		{
			m_closedViaEntry = false;
			m_onDismissed = null;

			if(!_TryGetOpenParam(param,out Param dialogParam))
			{
				return;
			}

			if(dialogParam.Title.IsEmpty() && dialogParam.Message.IsEmpty())
			{
				// Either field may be empty individually; SetSafeTextMeshPro hides the corresponding text object.
				_FailOpen($"{NameTag} requires a non-empty title or message.");

				return;
			}

			var entryInfoArray = dialogParam.EntryInfoArray;

			if(entryInfoArray.IsNullOrEmpty())
			{
				_FailOpen($"{NameTag} has no valid entries.");

				return;
			}

			var entryInfoList = new List<IEntryInfo>(entryInfoArray.Length);

			for(var i=0;i<entryInfoArray.Length;i++)
			{
				var entryInfo = entryInfoArray[i];

				if(entryInfo == null)
				{
					_FailOpen($"{NameTag} entry at index {i} is null.");

					return;
				}

				if(entryInfo.Name.IsEmpty())
				{
					_FailOpen($"{NameTag} entry at index {i} has an empty name.");

					return;
				}

				if(entryInfo.OnClicked == null)
				{
					_FailOpen($"{entryInfo.Name} {nameof(DialogEntryInfo.OnClicked)} is null.");

					return;
				}

				entryInfoList.Add(_WrapEntryInfo(entryInfo));
			}

			m_onDismissed = dialogParam.OnDismissed;

			base.Open(param);

			if(m_titleText)
			{
				m_titleText.SetSafeTextMeshPro(dialogParam.Title);
			}

			m_messageText.SetSafeTextMeshPro(dialogParam.Message);

			m_gridLayout.SetEntryInfoList(entryInfoList);
		}

		public override void Close()
		{
			// X button, back press, and UIManager.Close reach here without m_closedViaEntry.
			if(!m_closedViaEntry)
			{
				m_onDismissed?.Invoke();
			}

			m_onDismissed = null;
			m_closedViaEntry = false;

			base.Close();
		}

		/// <summary>Wraps the caller callback to close via <see cref="Window._SelfClose"/> after the entry action.</summary>
		private DialogEntryInfo _WrapEntryInfo(DialogEntryInfo entryInfo)
		{
			void _OnClicked(IEntryInfo clickedInfo)
			{
				// Suppress OnDismissed when the user chose an explicit entry.
				m_closedViaEntry = true;

				entryInfo.OnClicked.Invoke(clickedInfo);

				_SelfClose();
			}

			return new DialogEntryInfo(entryInfo.Name,_OnClicked);
		}
	}
}