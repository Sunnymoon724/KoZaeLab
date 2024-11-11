
public class MenuSlotUI : SlotUI
{
	private string m_MenuText = null;

	protected override bool UseGizmos => true;
	protected override string GizmosText => m_MenuText ?? string.Empty;

	public override void SetCell(ICellData _cell)
	{
		m_MenuText = _cell.CellName;

		base.SetCell(_cell);
	}
}