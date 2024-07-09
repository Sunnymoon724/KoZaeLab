using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxPopupUI : WindowUI2D
{
	public record DialogParam(string Message,params (string,Action)[] ButtonList);

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

		foreach(var data in param.ButtonList)
		{
			if(data.Item2 == null)
			{
				throw new NullReferenceException(string.Format("{0}의 실행 함수가 없습니다.",data.Item1));
			}

			cellList.Add(new CellData(data.Item1,null,null,null,()=> { data.Item2(); }));
		}

		if(cellList.IsNullOrEmpty())
		{
			return;
		}

		m_GridLayout.SetCellList(cellList);
	}
}