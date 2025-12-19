using UnityEngine;

public class ListSlotUI : SlotUI
{
	[SerializeField] private ReuseGridLayoutGroupUI m_gridLayout = null;

	protected override bool UseImage => false;
	protected override bool UseButton => false;

	public override void SetEntry(IEntryInfo entryInfo)
	{
		var listEntry = entryInfo as ListEntryInfo;

		m_gridLayout.SetEntryInfoList(listEntry.EntryInfoList);

		base.SetEntry(entryInfo);
	}
}