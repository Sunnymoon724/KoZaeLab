
public class MenuSlotUI : SlotUI
{
	private string m_menuText = null;

	protected override bool UseGizmos => true;
	protected override string GizmosText => m_menuText ?? string.Empty;

	public override void SetEntry(IEntryInfo entryInfo)
	{
		m_menuText = entryInfo.Name;

		base.SetEntry(entryInfo);
	}
}