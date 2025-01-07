using System.Collections.Generic;
using UnityEngine;

public class ListSlotUI : SlotUI
{
	[SerializeField] private ReuseGridLayoutGroupUI m_gridLayout = null;

	protected override bool UseImage => false;
	protected override bool UseButton => false;

	public override void SetCell(ICellData cellData)
	{
		var listCellData = cellData as ListCellData;

		m_gridLayout.SetCellList(listCellData.CellDataList);

		base.SetCell(cellData);
	}
}

public record ListCellData : CellData
{
	public List<ICellData> CellDataList { get; }

	public ListCellData(string name,List<ICellData> cellDataList) : base(name,null,null,null,null)
	{
		CellDataList = new List<ICellData>(cellDataList);
	}
}