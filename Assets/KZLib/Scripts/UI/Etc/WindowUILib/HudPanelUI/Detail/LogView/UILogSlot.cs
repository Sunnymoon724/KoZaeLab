using System;
using UnityEngine;

namespace HudPanel
{
	public class UILogSlot : SlotUI
	{
		private Color m_tagColor = Color.white;

		public override void SetEntry(IEntryInfo entryInfo)
		{
			var logEntry = entryInfo as LogEntryInfo;

			m_tagColor = logEntry.TagColor;

			base.SetEntry(entryInfo);
		}

		protected override void _SetIcon(Sprite sprite)
		{
			m_image.color = m_tagColor;
		}
	}

	public record LogEntryInfo(Color TagColor,string Name,string Description,Action<IEntryInfo> OnClicked) : EntryInfo(Name,Description,OnClicked);
}