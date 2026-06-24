namespace KZLib.UI
{
	/// <summary>
	/// <see cref="Slot"/> for toolbar menu cells. Scene gizmos display the menu entry name.
	/// </summary>
	/// <remarks>
	/// Used with <see cref="MenuToolBarUI"/>. <see cref="EntryInfo.Name"/> is a localization key
	/// (<c>UIMenu_{tag}</c>). Resolves via <see cref="LocalizeTextUI"/> when present, otherwise
	/// <see cref="string.ToLocalize"/> on bind. Gizmo text follows <see cref="Slot.CurrentEntryInfo"/>.
	/// </remarks>
	public class MenuSlot : Slot
	{
		/// <inheritdoc/>
		protected override string GizmosText => CurrentEntryInfo?.Name ?? string.Empty;

		/// <inheritdoc/>
		protected override void _SetName(string text)
		{
			if(!m_nameText)
			{
				return;
			}

			if(m_nameText.GetComponent<LocalizeTextUI>())
			{
				m_nameText.SetLocalizeText(text);
			}
			else
			{
				m_nameText.SetSafeTextMeshPro(text?.ToLocalize());
			}
		}
	}
}