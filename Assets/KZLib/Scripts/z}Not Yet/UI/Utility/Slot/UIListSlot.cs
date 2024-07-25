// using System.Collections.Generic;
// using UnityEngine;

// public class UIListSlot : SlotUI
// {	
// 	[SerializeField] private UIFlexibleGrid m_Grid = null;

// 	protected override bool UseImage => false;
// 	protected override bool UseButton => false;

// 	protected override void SetCellData(CellData _data)
// 	{
// 		var data = _data as ListCellData;

// 		m_Grid.SetCellList(data.CellList,0);

// 		base.SetCellData(_data);
// 	}
// }

// public class ListCellData : CellData
// {
// 	public List<ICellData> CellList { get; }

// 	public ListCellData(string _name,List<ICellData> _cellList) : base(_name,string.Empty,string.Empty,string.Empty)
// 	{
// 		CellList = new List<ICellData>(_cellList.Count);
// 		CellList.AddRange(_cellList);
// 	}
// }