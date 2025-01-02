using System.Collections.Generic;
using UnityEngine;

public class ListSlotUI : SlotUI
{
	[SerializeField] private ReuseGridLayoutGroupUI m_gridLayout = null;

	protected override bool UseImage => false;
	protected override bool UseButton => false;

	public override void SetCell(ICellData _cellData)
	{
		var data = _cellData as ListCellData;

		m_gridLayout.SetCellList(data.CellList);

		base.SetCell(_cellData);
	}
}

public record ListCellData : CellData
{
	public List<ICellData> CellList { get; }

	public ListCellData(string _name,List<ICellData> _cellList) : base(_name,null,null,null,null)
	{
		CellList = new List<ICellData>(_cellList);
	}
}