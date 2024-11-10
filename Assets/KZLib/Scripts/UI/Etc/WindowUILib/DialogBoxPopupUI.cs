using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxPopupUI : WindowUI2D
{
	public record DialogParam(string Message,params DialogCellData[] DataArray);

	public override UITag Tag => UITag.DialogBoxPopupUI;

	[SerializeField]
	private TMP_Text m_MessageText = null;

	[SerializeField]
	private GridLayoutGroupUI m_GridLayout = null;

	public override void Open(object _param)
	{
		base.Open(_param);

		if(_param is not DialogParam param)
		{
			return;
		}

		m_MessageText.SetLocalizeText(param.Message);

		var cellList = new List<ICellData>();

		foreach(var data in param.DataArray)
		{
			if(data.OnClicked == null)
			{
				LogTag.UI.E($"{data.Title} is null");

				return;
			}

			cellList.Add(data);
		}

		if(cellList.IsNullOrEmpty())
		{
			return;
		}

		m_GridLayout.SetCellList(cellList);
	}
}

public record DialogCellData(string Title,Action OnClicked) : CellData(Title,null,null,null,OnClicked);