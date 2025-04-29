using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxPopupUI : WindowUI2D
{
	public record DialogParam(string Title,string Message,params DialogCellData[] CellDataArray);

	public override UITag Tag => UITag.DialogBoxPopupUI;

	[SerializeField]
	private TMP_Text m_titleText = null;

	[SerializeField]
	private TMP_Text m_messageText = null;

	[SerializeField]
	private ReuseGridLayoutGroupUI m_gridLayout = null;

	public override void Open(object param)
	{
		base.Open(param);

		if(param is not DialogParam dialogParam)
		{
			return;
		}

		if(m_titleText)
		{
			m_titleText.SetSafeTextMeshPro(dialogParam.Title);
		}

		m_messageText.SetSafeTextMeshPro(dialogParam.Message);

		var cellDataList = new List<ICellData>();

		foreach(var cellData in dialogParam.CellDataArray)
		{
			if(cellData.OnClicked == null)
			{
				LogTag.UI.E($"{cellData.Name} is null");

				return;
			}

			cellDataList.Add(cellData);
		}

		if(cellDataList.IsNullOrEmpty())
		{
			return;
		}

		m_gridLayout.SetCellList(cellDataList);
	}
}

public record DialogCellData(string Name,Action OnClicked) : CellData(Name,null,null,null,OnClicked);