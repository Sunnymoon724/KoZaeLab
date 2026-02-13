using System;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI.Widgets.Debug
{
	public class LogSlot : Slot
	{
		private Color m_tagColor = Color.white;

		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			var logEntry = entryInfo as LogEntryInfo;

			m_tagColor = logEntry.TagColor;

			base.SetEntryInfo(entryInfo);
		}

		protected override void _SetIcon(Sprite sprite)
		{
			m_image.color = m_tagColor;
		}
	}
}

public record LogEntryInfo(Color TagColor,string Name,string Description,Action<IEntryInfo> OnClicked) : EntryInfo(Name,Description,OnClicked);